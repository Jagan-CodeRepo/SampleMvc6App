using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MVCEFSample.Entities.Common;
using System.Reflection;
using MVCEFSample.Entities.EF;

namespace MVCEFSample.DAL.Repository
{
    public class GenericFilter
    {
        public static IEnumerable<T> ApplyFilterRules<T>(IEnumerable<T> dataContainer) where T : class
        {
            Expression filterExpression = null;
            MethodCallExpression methodCallExpression = null;
            BinaryExpression binaryExpression = null;
            var parameter = Expression.Parameter(typeof(T));
            IEnumerable<CustomFilter> filters = new List<CustomFilter>()
            {
                new CustomFilter(){
                    ColumnName="FirstName",
                    ColumnValue="ja",
                    LogicalOperator="and",
                    RelationalOperator="like"
                },
                new CustomFilter(){
                    ColumnName="TotalChildren",
                    ColumnValue="0",
                    LogicalOperator="and",
                    RelationalOperator="!="
                },
                new CustomFilter(){
                    ColumnName="Gender",
                    ColumnValue="M",
                    LogicalOperator="and",
                    RelationalOperator="="
                },new CustomFilter(){
                    ColumnName="LastName",
                    ColumnValue="ang",
                    LogicalOperator="or",
                    RelationalOperator="like"
                }                
            };
            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.ColumnName);
                if (property != null)
                {
                    var propertyType = property.PropertyType;
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    switch (filter.RelationalOperator)
                    {
                        case "=":
                            binaryExpression = GetEqualBinaryExpression(propertyAccess, filter.ColumnValue);
                            filterExpression = GetLogicalExpression(filter.LogicalOperator, filterExpression, binaryExpression);
                            break;
                        case "!=":
                            binaryExpression = GetNotEqualBinaryExpression(propertyAccess, filter.ColumnValue);
                            filterExpression = GetLogicalExpression(filter.LogicalOperator, filterExpression, binaryExpression);
                            break;
                        case "like":
                            methodCallExpression = GetLikeExpression(propertyAccess, filter.ColumnValue);
                            filterExpression = GetMethodCallExpression(filter.LogicalOperator, filterExpression, methodCallExpression);
                            break;
                    }
                }
            }
            if (filterExpression != null)
            {
                using (AdventureWorksEntities dbContext = new AdventureWorksEntities())
                {
                    Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                    Func<T, bool> compiled = predicate.Compile();
                    return dataContainer.Where(compiled).ToList();
                }
            }
            return dataContainer.ToList();
        }
        static BinaryExpression GetEqualBinaryExpression(MemberExpression propertyAccess, string columnValue)
        {
            return Expression.Equal(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
        }
        static BinaryExpression GetNotEqualBinaryExpression(MemberExpression propertyAccess, string columnValue)
        {
            return Expression.NotEqual(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
        }
        static MethodCallExpression GetLowerCasePropertyAccess(MemberExpression propertyAccess)
        {
            return Expression.Call(Expression.Call(propertyAccess, "ToString", new Type[0]), typeof(string).GetMethod("ToLower", new Type[0]));
        }
        static MethodCallExpression GetLikeExpression(MemberExpression propertyAccess, string columnValue)
        {
            MethodCallExpression methodCallExpression = Expression.Call(GetLowerCasePropertyAccess(propertyAccess), ContainsMethod, Expression.Constant(columnValue.ToLower()));
            return methodCallExpression;
        }
        static Expression GetMethodCallExpression(string logicalOperator, Expression filterExpression, MethodCallExpression methodCallExpression)
        {
            switch (logicalOperator.ToLower())
            {
                case "and":
                    if (filterExpression == null)
                        filterExpression = methodCallExpression;
                    else
                        filterExpression = Expression.And(filterExpression, methodCallExpression);
                    break;
                case "or":
                    if (filterExpression == null)
                        filterExpression = methodCallExpression;
                    else
                        filterExpression = Expression.Or(filterExpression, methodCallExpression);
                    break;
                default:
                    if (filterExpression == null)
                        filterExpression = methodCallExpression;
                    else
                        filterExpression = Expression.And(filterExpression, methodCallExpression);
                    break;
            }
            return filterExpression;
        }
        static Expression GetLogicalExpression(string logicalOperator, Expression filterExpression, BinaryExpression binaryExpression)
        {
            switch (logicalOperator.ToLower())
            {
                case "and":
                    filterExpression = filterExpression == null ? binaryExpression : Expression.And(filterExpression, binaryExpression);
                    break;
                case "or":
                    filterExpression = filterExpression == null ? binaryExpression : Expression.Or(filterExpression, binaryExpression);
                    break;
                default:
                    filterExpression = filterExpression == null ? binaryExpression : Expression.And(filterExpression, binaryExpression);
                    break;
            }
            return filterExpression;
        } 
        private static readonly MethodInfo ContainsMethod = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });
        private static readonly MethodInfo StartsWithMethod = typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) });
        private static readonly MethodInfo EndsWithMethod = typeof(String).GetMethod("EndsWith", new Type[] { typeof(String) });
    }
}
