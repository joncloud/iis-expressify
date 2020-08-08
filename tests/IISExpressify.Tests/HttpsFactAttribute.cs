using System;
using Xunit;

namespace IISExpressify.Tests
{
    public class HttpsFactAttribute : FactAttribute
    {
        public HttpsFactAttribute()
        {
            Skip = _skip;
        }

        static readonly string _skip;
        static HttpsFactAttribute()
        {
            var value = Environment.GetEnvironmentVariable("iisExpressifyTestIgnoreHttps");
            _skip = string.IsNullOrEmpty(value)
                ? ""
                : "HTTPS Tests are Ignored";
        }
    }
}
