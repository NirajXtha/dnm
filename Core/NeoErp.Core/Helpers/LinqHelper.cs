using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace NeoErp.Core.Helpers
{
   
    
    public class FilterModel
    {
        public string FilterField { get; set; }
        public string FilterValue { get; set; }
        public Op Operation { get; set; }
    }

    public enum Op
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    public static class ExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo startsWithMethod =
        typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod =
        typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });


        public static Expression<Func<T,bool>> GetExpression<T>(IList<FilterModel> filters)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]);
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    if (exp == null)
                        exp = GetExpression<T>(param, filters[0], filters[1]);
                    else
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(ParameterExpression param, FilterModel filter)
        {
            MemberExpression member = Expression.Property(param, filter.FilterField);
            ConstantExpression constant = Expression.Constant(filter.FilterValue);

            switch (filter.Operation)
            {
                case Op.Equals:
                    return Expression.Equal(member, constant);

                case Op.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Op.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Op.LessThan:
                    return Expression.LessThan(member, constant);

                case Op.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case Op.Contains:
                    return GetCaseInsensitiveСompartion(containsMethod, member, constant); // Expression.Call(member, containsMethod, constant);

                case Op.StartsWith:
                    return GetCaseInsensitiveСompartion(startsWithMethod, member, constant); // return Expression.Call(member, startsWithMethod, constant);

                case Op.EndsWith:
                    return GetCaseInsensitiveСompartion(endsWithMethod, member, constant);  //return Expression.Call(member, endsWithMethod, constant);
            }

            return null;
        }

        private static Expression GetCaseInsensitiveСompartion(MethodInfo methodName, Expression leftExpr, Expression rightExpr)
        {
            Type targetType = TargetType;
            //case insensitive compartion:
            MethodInfo miUpper = targetType.GetMethod("ToUpper", new Type[] { });
            MethodCallExpression upperValueExpr = Expression.Call(rightExpr, miUpper);
            MethodCallExpression upperFirstExpr = Expression.Call(leftExpr, miUpper);

            //if (!string.IsNullOrEmpty(methodName))
            //{
            //    MethodInfo mi = targetType.GetMethod(methodName, new[] { typeof(string) });
            //    if (mi == null)
            //        throw new MissingMethodException("There is no method - " + methodName);
            //    return Expression.Call(upperFirstExpr, mi, upperValueExpr);
            //}
            return Expression.Call(upperFirstExpr, methodName, upperValueExpr);

            //return Expression.Equal(upperFirstExpr, upperValueExpr);
        }

        public static  Type TargetType
        {
            get { return typeof(String); }
        }

        private static BinaryExpression GetExpression<T>
        (ParameterExpression param, FilterModel filter1, FilterModel filter2)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }

        /// <summary>
        /// Dynamically runs an aggregate function on the IQueryable.
        /// </summary>
        /// <param name="source">The IQueryable data source.</param>
        /// <param name="function">The name of the function to run. Can be Sum, Average, Min, Max.</param>
        /// <param name="member">The name of the property to aggregate over.</param>
        /// <returns>The value of the aggregate function run over the specified property.</returns>
        public static object GetAggregateResult(this IQueryable source, string function, string member)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (member == null) throw new ArgumentNullException("member");

            // Properties
            PropertyInfo property = source.ElementType.GetProperty(member);
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "s");
            Expression selector = Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter);
            // We've tried to find an expression of the type Expression<Func<TSource, TAcc>>,
            // which is expressed as ( (TSource s) => s.Price );

           var methods = typeof(Queryable).GetMethods().Where(x => x.Name == function);

            // Method
            MethodInfo aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
            m => m.Name == function
            && m.ReturnType == property.PropertyType // should match the type of the property
                        && m.IsGenericMethod);

            if((property.PropertyType == typeof(int) || property.PropertyType == typeof(int?)) && function == "Average")
            {
                aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
                m => m.Name == function
                && (m.ReturnType == typeof(double) || m.ReturnType == typeof(double?)) // should match the type of the property
                            && m.IsGenericMethod);
            }

            // Sum, Average
            if (aggregateMethod != null)
            {
                return source.Provider.Execute(
                Expression.Call(
                null,
                aggregateMethod.MakeGenericMethod(new[] { source.ElementType }),
                new[] { source.Expression, Expression.Quote(selector) }));
            }
            // Min, Max
            else
            {
                aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
                m => m.Name == function
                && m.GetGenericArguments().Length == 2
                && m.IsGenericMethod);

                return source.Provider.Execute(
                Expression.Call(
                null,
                aggregateMethod.MakeGenericMethod(new[] { source.ElementType, property.PropertyType }),
                new[] { source.Expression, Expression.Quote(selector) }));
            }
        }

        
    }
}