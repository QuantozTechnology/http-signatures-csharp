using AspNetActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetSample.Models
{
    public class KeyStoreService : IKeyStoreService
    {
        public IDictionary<string, string> GetKeyStore()
        {
            return new Dictionary<string, string>() { { "some-key", "6Jj3c7lQr6dhDf4oMsYnTfrjFnwezP4GzqHlqr2heyw=" } };
        }
    }
}