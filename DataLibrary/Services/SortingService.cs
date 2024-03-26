using DataLibrary.Services.Interfaces;
using System.Linq.Expressions;

namespace DataLibrary.Services
{
    public class SortingService<T> : ISortingService<T>
    {
        public IQueryable<T> Sort(IQueryable<T> query, string sortColumn, string sortOrder, Dictionary<string, Expression<Func<T, object>>> sortExpressions)
        {
            sortColumn = sortColumn ?? "DefaultName";

            if (sortExpressions.ContainsKey(sortColumn))
            {
                var sortExpression = sortExpressions[sortColumn];

                if (sortOrder == "asc")
                {
                    query = query.OrderBy(sortExpression);
                }
                else
                {
                    query = query.OrderByDescending(sortExpression);
                }
            }

            return query;
        }
    }
}
