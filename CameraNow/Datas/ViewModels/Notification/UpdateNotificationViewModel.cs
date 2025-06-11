namespace Datas.ViewModels.Notification
{
    public class UpdateNotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string User_ID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }
}
