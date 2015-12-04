namespace DancingGoat.Models.Pager
{
    public interface IPagedDataSource
    {
        int PageIndex
        {
            get;
        }

        int PageSize
        {
            get;
        }

        int TotalItemCount
        {
            get;
        }
    }
}