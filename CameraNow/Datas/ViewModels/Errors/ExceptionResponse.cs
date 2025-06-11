using System.Text.Json;

namespace Datas.ViewModels.Errors
{
    public class ExceptionResponse
    {
        public ExceptionResponse()
        {
        }

        public ExceptionResponse(int? statusCode, object? message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public int? StatusCode { get; set; }
        public object? Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
