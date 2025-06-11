namespace Datas.ViewModels.Notification
{
    public class CreateNotificationViewModel
    {
        public string Title { get; set; }
        public string User_ID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string Role { get; set; }
    }
}
