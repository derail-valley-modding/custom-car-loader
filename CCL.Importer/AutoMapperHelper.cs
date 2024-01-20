using System;
using System.Linq.Expressions;
using System.Reflection;

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
    }
}
