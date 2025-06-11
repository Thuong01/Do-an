namespace Datas.Extensions.Responses
{
    public class ResponseMessage
    {
        public ResponseMessage()
        {
        }

        public ResponseMessage(bool success, object result)
        {
            Success = success;
            Result = result;
        }

        public bool Success { get; set; }
        public object Result { get; set; }
    }
}
