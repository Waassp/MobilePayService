using System.Net;

namespace MobilePayService.RestAPI
{

    public class ApiResponse
    {
        public string Version { get { return "1.2.3"; } }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public object Result { get; set; }

        public ApiResponse(HttpStatusCode statusCode, object result = null, string errorMessage = null)
        {
            StatusCode = (int)statusCode;
            Result = result;
            ErrorMessage = errorMessage;
        }

    }
}