namespace DataLibrary.Services.Interfaces
{
    public interface IPaginationService<T>
    {
        IQueryable<T> GetPage(IQueryable<T> query, int page, int itemsPerPage);
        int GetTotalPages(IQueryable<T> query, int itemsPerPage);
    }
}
