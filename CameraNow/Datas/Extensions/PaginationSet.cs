namespace Datas.Extensions
{
    public class PaginationSet<T>
    {
        public PaginationSet()
        {
        }

        public PaginationSet(int page)
        {
            Page = page;
        }

        public PaginationSet(int page, int totalPage, int totalCount, IEnumerable<T> data)
        {
            Page = page;
            TotalPage = totalPage;
            TotalCount = totalCount;
            Data = data;
        }

        public int Page { get; set; }
        public int Count { get => (Data != null) ? Data.Count() : 0; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}