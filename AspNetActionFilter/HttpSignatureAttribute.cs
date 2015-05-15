using HttpSignatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace AspNetActionFilter
{
    public static class WebApiRequestConverter
    {
        public static IRequest FromHttpRequest(HttpRequestMessage r)
        {
            var req = new Request()
            {
                Method = new HttpMethod(r.Method.ToString()),
                Path = r.RequestUri.PathAndQuery
            };

            foreach (var h in r.Headers)
            {
                req.Headers.Add(h.Key.ToLowerInvariant(), h.Value.First());
            }

            return req;
        }
    }

    public interface IKeyStoreService
    {
        IDictionary<string, string> GetKeyStore();
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpSignatureAttribute : ActionFilterAttribute
    {
        private string[] headers;

        public HttpSignatureAttribute(params string[] headers)
        {
            this.headers = headers;
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext context)
        {
            var keyService = (IKeyStoreService)context
                          .ControllerContext.Configuration.DependencyResolver
                          .GetService(typeof(IKeyStoreService));

            var keys = keyService.GetKeyStore();

            var request = context.Request;

            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == "Signature")
            {
                var signer = new HttpSigner(new AuthorizationParser(), new HttpSignatureStringExtractor());

                var spec = new SignatureSpecification()
                {
                    Algorithm = "hmac-sha256",
                    Headers = headers,
                    //KeyId = "some-key" // TODO: make this dynamic..., seems like this can be omitted
                };

                var sigRequest = WebApiRequestConverter.FromHttpRequest(request);

                var keyStore = new KeyStore(keys);

                var signature = signer.Signature(sigRequest, spec, keyStore);

                if (!signature.Valid)
                {
                    throw new Exception("signature invalid");
                }
            }
            else
            {
                throw new Exception("missing authentication");
            }
        }
    }
}
