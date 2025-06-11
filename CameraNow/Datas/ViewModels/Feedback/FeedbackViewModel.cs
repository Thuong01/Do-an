using Datas.Extensions;
using Datas.ViewModels.Image;

namespace Datas.ViewModels.Feedback
{
    public class Stars
    {
        public string star { get; set; }
        public double Percent { get; set; }
        public int SumStar { get; set; }
    }

    public class Data
    {
        public Guid ID { get; set; }

        public Guid? Parent_ID { get; set; }

        public Guid Product_ID { get; set; }

        public string User_ID { get; set; }

        public string User_Name { get; set; }

        public string User_Avatar { get; set; }

        public string Subject { get; set; }

        public string? Message { get; set; }

        public DateTime Date { get; set; }

        public int Rating { get; set; } = 0;

        public int Likes { get; set; } = 0;

        public IEnumerable<FeedbackImageViewModel>? Images { get; set; }

        public string? Videos { get; set; }

        public IEnumerable<Data> FeedbackChildrents { get; set; }

        public Data FeedbackParent { get; set; }
    }

    public class FeedbackViewModel
    {
        public double Rating_Average { get; set; }

        public int Feedbacks_Count { get; set; }

        public IEnumerable<Stars> Stars { get; set; }

        public PaginationSet<Data> Data { get; set; }
    }
}
