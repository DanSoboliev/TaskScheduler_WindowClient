using System;
using System.Net.Http;

namespace TaskShedulerDesktopClient.Data.Errors {
    public class ServerException : Exception {
        public HttpResponseMessage ResponseMessage { get; set; }
        public ServerException(HttpResponseMessage responseMessage) {
            ResponseMessage = responseMessage;
        }
    }
}
