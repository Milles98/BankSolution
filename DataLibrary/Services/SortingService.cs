using DataLibrary.Services.Interfaces;
using System.Linq.Expressions;

namespace DataLibrary.Services
{
    public class SortingService<T> : ISortingService<T>
    {
        public IQueryable<T> Sort(IQueryable<T> query, string sortColumn, string sortOrder, Dictionary<string, LambdaExpression> sortExpressions)
        {
            sortColumn = sortColumn ?? "DefaultName";

            if (sortExpressions.ContainsKey(sortColumn))
            {
                var sortExpression = sortExpressions[sortColumn];

                if (sortOrder == "asc")
                {
                    query = CallOrderBy(query, sortExpression, "OrderBy");
                }
                else
                {
                    query = CallOrderBy(query, sortExpression, "OrderByDescending");
                }
            }

            return query;
        }

        private static IQueryable<T> CallOrderBy(IQueryable<T> query, LambdaExpression sortExpression, string methodName)
        {
            var expression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), sortExpression.ReturnType },
                query.Expression,
                sortExpression);

            return query.Provider.CreateQuery<T>(expression);
        }
    }

}
