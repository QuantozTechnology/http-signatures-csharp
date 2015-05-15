using HttpSignatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using AspNetActionFilter;

namespace AspNetSample.Controllers
{
    [Route("api")]
    public class ValuesController : ApiController
    {
        [Route("signature")]
        public HttpResponseMessage GetSignature()
        {
            var signer = new HttpSigner(new AuthorizationParser(), new HttpSignatureStringExtractor());

            var key = "some-key";
            var secret = "6Jj3c7lQr6dhDf4oMsYnTfrjFnwezP4GzqHlqr2heyw=";

            var request = new Request();
            request.Path = "/api";
            request.Method = HttpMethod.Get;
            request.SetHeader("host", "localhost:53755");
            //request.SetHeader("date", DateTime.UtcNow.ToString("r"));

            var spec = new SignatureSpecification()
            {
                Algorithm = "hmac-sha256",
                Headers = new string[] { "host", "(request-target)" },
                KeyId = "some-key"
            };

            var keyStore = new KeyStore(new Dictionary<string, string>()
            {
                { key, secret }
            });

            signer.Sign(request, spec, key, secret);
            var signature = signer.Signature(request, spec, keyStore);

            return Request.CreateResponse(signature);
        }

        //[HttpSignature("host", "(request-target)")]
        [HttpSignatureAuthentication("host", "(request-target)")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse("Succes!");
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
