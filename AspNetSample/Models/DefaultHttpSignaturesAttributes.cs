using AspNetActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetSample.Models
{
    public class SampleGetSignature : HttpSignatureAttribute
    {
        public SampleGetSignature()
            : base("host", "(request-target")
        {

        }
    }
}