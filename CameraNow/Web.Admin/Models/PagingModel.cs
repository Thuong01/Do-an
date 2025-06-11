namespace Web.Admin.Models
{
    public class PagingModel
    {
        public PagingModel()
        {
        }

        public PagingModel(int currentPage, int countPages, Func<int?, string> generateUrl)
        {
            CurrentPage = currentPage;
            CountPages = countPages;
            GenerateUrl = generateUrl;
        }

        public int CurrentPage { get; set; }
        public int CountPages { get; set; }
        public Func<int?, string> GenerateUrl { get; set; }
    }
}
