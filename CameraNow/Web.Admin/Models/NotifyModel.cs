namespace Web.Admin.Models
{
    public class NotifyModel
    {
        public NotifyModel(string message, bool success = false, bool warning = false, bool error = false, bool info = false)
        {
            Success = success;
            Warning = warning;
            Error = error;
            Info = info;
            Message = message;
        }

        public bool Success { get; set; }
        public bool Warning { get; set; }
        public bool Error { get; set; }
        public bool Info { get; set; }
        public string Message { get; set; }
    }
}
