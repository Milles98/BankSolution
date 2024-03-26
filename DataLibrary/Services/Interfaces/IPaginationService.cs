namespace DataLibrary.Services.Interfaces
{
    public interface IPaginationService<T>
    {
        IQueryable<T> GetPage(IQueryable<T> query, int page, int itemsPerPage);
    }
}
