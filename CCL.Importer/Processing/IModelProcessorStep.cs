using System.Linq;
using System;

namespace CCL.Importer.Processing
{
    internal interface IModelProcessorStep : IComparable<IModelProcessorStep>
    {
        Type[] RequiredSteps { get; }

        void ExecuteStep(ModelProcessor context);
    }

    internal abstract class ModelProcessorStep : IModelProcessorStep
    {
        public virtual Type[] RequiredSteps => new Type[0];

        public int CompareTo(IModelProcessorStep other)
        {
            if (RequiredSteps.Contains(other.GetType()))
            {
                return 1;
            }
            else if (other.RequiredSteps.Contains(GetType()))
            {
                return -1;
            }
            return 0;
        }

        public abstract void ExecuteStep(ModelProcessor context);
    }
}
