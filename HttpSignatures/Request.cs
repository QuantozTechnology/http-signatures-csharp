using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace HttpSignatures
{
    public class Request : IRequest
    {
        public Request()
        {
            Headers = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        }

        public HttpMethod Method { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string Body { get; set; }

        public string GetHeader(string header)
        {
            return Headers[header];
        }

        public void SetHeader(string header, string value)
        {
            Headers[header] = value;
        }

        public string Path { get; set; }
    }
}