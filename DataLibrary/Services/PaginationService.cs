using BankWeb.Services.Interfaces;

namespace BankWeb.Services
{
    public class PaginationService<T> : IPaginationService<T>
    {
        public IQueryable<T> GetPage(IQueryable<T> query, int page, int itemsPerPage)
        {
            return query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage);
        }
    }
}
