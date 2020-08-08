using System.IO;
using System.Reflection;

namespace IISExpressify.Tests
{
    static class Hosting
    {
        public static string GetHostingDirectory() =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
