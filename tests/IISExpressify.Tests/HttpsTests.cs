using System.Threading.Tasks;
using Xunit;

namespace IISExpressify.Tests
{
    public class HttpsTests
    {
        static HttpsIisExpressOptions CreateHttpsOptions() =>
            IisExpress.Https().PhysicalPath(Hosting.GetHostingDirectory()).Port(44300);

        [HttpsFact]
        public async Task Https_ShouldDownloadTestFile()
        {
            var iisExpress = CreateHttpsOptions().Start();
            var http = iisExpress.CreateHttpClient();

            var contents = await http.GetStringAsync("/Test.txt");
            Assert.Equal("Lorem ipsum", contents);
        }

        [HttpsFact]
        public async Task Https_ShouldRenderAsp_GivenClassic20Pipeline()
        {
            var iisExpress = CreateHttpsOptions().SetPipelineClassic20().Start();
            var http = iisExpress.CreateHttpClient();

            var contents = await http.GetStringAsync("/Classic.asp");
            Assert.Equal("1/10/2019", contents);
        }

        [HttpsFact]
        public async Task Https_ShouldRenderAsp_GivenClassic40Pipeline()
        {
            var iisExpress = CreateHttpsOptions().SetPipelineClassic40().Start();
            var http = iisExpress.CreateHttpClient();

            var contents = await http.GetStringAsync("/Classic.asp");
            Assert.Equal("1/10/2019", contents);
        }

        [HttpsFact]
        public async Task Https_ShouldRenderAspx_GivenClassic20Pipeline()
        {
            var iisExpress = CreateHttpsOptions().SetPipelineClassic20().Start();
            var http = iisExpress.CreateHttpClient();

            var contents = await http.GetStringAsync("/WebForm.aspx");
            Assert.Equal("2019-01-10", contents);
        }

        [HttpsFact]
        public async Task Https_ShouldRenderAspx_GivenClassic40Pipeline()
        {
            var iisExpress = CreateHttpsOptions().SetPipelineClassic40().Start();
            var http = iisExpress.CreateHttpClient();

            var contents = await http.GetStringAsync("/WebForm.aspx");
            Assert.Equal("2019-01-10", contents);
        }
    }
}
