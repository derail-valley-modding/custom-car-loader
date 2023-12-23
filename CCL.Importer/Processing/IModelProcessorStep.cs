using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CCL.Importer.Processing
{
    /// <summary>
    /// Don't implement this directly, inherit from <see cref="ModelProcessorStep"/> instead
    /// </summary>
    internal interface IModelProcessorStep : IComparable<IModelProcessorStep>
    {
        void ExecuteStep(ModelProcessor context);
    }

    /// <summary>
    /// Used to specify a <see cref="ModelProcessorStep"/> that must be executed before executing before this one.
    /// <para>Use <see cref="ModelProcessor.GetCompletedStep{T}"/> to obtain the result of the required step, if desired.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class RequiresStepAttribute : Attribute
    {
        public readonly Type RequiredStepType;

        public RequiresStepAttribute(Type requiredStepType)
        {
            if (!typeof(ModelProcessorStep).IsAssignableFrom(requiredStepType))
            {
                throw new ArgumentOutOfRangeException(nameof(requiredStepType), "Requirement must be a ModelProcessorStep");
            }
            RequiredStepType = requiredStepType;
        }
    }

    /// <summary>
    /// Base class for all transformations applied to a custom car/livery.
    /// Inherit from this and apply attribute [Export(typeof(<see cref="IModelProcessorStep"/>)].
    /// <para>See also <see cref="RequiresStepAttribute"/> for specifying execution order</para>
    /// </summary>
    internal abstract class ModelProcessorStep : IModelProcessorStep
    {
        public int CompareTo(IModelProcessorStep other)
        {
            Type thisType = GetType();
            Type otherType = other.GetType();

            var reqs = GetRequirements(GetType());
            var otherReqs = GetRequirements(otherType);

            if (ContainsRequirement(reqs, otherType))
            {
                return 1;
            }
            else if (ContainsRequirement(otherReqs, thisType))
            {
                return -1;
            }

            return reqs.Count.CompareTo(otherReqs.Count);
        }

        private static readonly Dictionary<Type, List<Type>> _requirementCache = new();

        private static bool ContainsRequirement(IEnumerable<Type> reqList, Type potentialReq)
        {
            return reqList.Any(t => t.IsAssignableFrom(potentialReq));
        }

        private static List<Type> GetRequirements(Type type)
        {
            if (!_requirementCache.TryGetValue(type, out List<Type> result))
            {
                result = new List<Type>();

                foreach (var attr in type.GetCustomAttributes<RequiresStepAttribute>())
                {
                    result.Add(attr.RequiredStepType);
                    result.AddRange(GetRequirements(attr.RequiredStepType));
                }

                _requirementCache[type] = result;
            }

            return result;
        }

        public abstract void ExecuteStep(ModelProcessor context);
    }
}
