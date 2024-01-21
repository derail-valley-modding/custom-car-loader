using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer
{
    internal static class AutoMapperHelper
    {
        public static Expression<Func<T, object>> CreateMemberLambda<T>(string parameterName)
        {
            var type = typeof(T);
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField;

            var parameter = type.GetField(parameterName, bindFlags);
            var classExpression = Expression.Parameter(type, type.Name);
            var memberAccessExpression = Expression.MakeMemberAccess(classExpression, parameter);

            var lambda = Expression.Lambda<Func<T, object>>(memberAccessExpression, classExpression);

            return lambda;
        }

        /// <summary>
        /// Can be used when private variables also need to be mapped.
        /// By using this method instead of checking if it's private, it
        /// avoids mapping private unity fields that should not be mapped.
        /// </summary>
        /// <param name="f">The field to check.</param>
        /// <returns><see cref="true"/> if the field is public, or is tagged with [<see cref="SerializeField"/>].</returns>
        public static bool IsPublicOrSerialized(FieldInfo f)
        {
            return f.IsPublic || Attribute.IsDefined(f, typeof(SerializeField));
        }
    }
}
