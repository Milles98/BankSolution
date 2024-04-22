using System.Linq.Expressions;

namespace DataLibrary.Services.Interfaces
{
    public interface ISortingService<T>
    {
        IQueryable<T> Sort(IQueryable<T> query, string sortColumn, string sortOrder, Dictionary<string, LambdaExpression> sortExpressions);
    }
}
