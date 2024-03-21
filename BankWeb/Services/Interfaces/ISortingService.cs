using System.Linq.Expressions;

namespace BankWeb.Services.Interfaces
{
    public interface ISortingService<T>
    {
        IQueryable<T> Sort(IQueryable<T> query, string sortColumn, string sortOrder, Dictionary<string, Expression<Func<T, object>>> sortExpressions);
    }
}
