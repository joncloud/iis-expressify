using System.Net.Http;

namespace IISExpressify
{
    public static class IisExpressExtensions
    {
        public static HttpClient CreateHttpClient(this IIisExpress iisExpress) =>
            new HttpClient { BaseAddress = iisExpress.BaseUri };
        public static HttpClient CreateHttpClient(this IIisExpress iisExpress, HttpMessageHandler handler) =>
            new HttpClient(handler) { BaseAddress = iisExpress.BaseUri };
    }
}
