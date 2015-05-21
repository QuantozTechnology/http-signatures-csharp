using HttpSignatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace AspNetActionFilter
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpSignatureAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private string[] headers;

        public HttpSignatureAuthenticationAttribute(params string[] headers)
        {
            this.headers = headers;
        }

        public Task AuthenticateAsync(HttpAuthenticationContext authContext, System.Threading.CancellationToken cancellationToken)
        {
            var context = authContext.ActionContext;

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

                if (signature.Valid)
                {
                    IPrincipal principal = new GenericPrincipal(new GenericIdentity(signature.KeyId), new[] { "PoolPartner" });

                    if (principal == null)
                    {
                        authContext.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
                    }
                    else
                    {
                        authContext.Principal = principal;
                    }
                }
                else
                {
                    throw new Exception("signature invalid");
                }
            }
            //else
            //{
            //    throw new Exception("missing authentication");
            //}

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, System.Threading.CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Signature");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get { throw new NotImplementedException(); }
        }
    }
}
