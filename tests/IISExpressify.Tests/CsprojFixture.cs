using System.Xml.Linq;

namespace IISExpressify.Tests
{
    public class CsprojFixture
    {
        public XDocument Document { get; }

        public CsprojFixture()
        {
            var path = "../../../../../src/IISExpressify/IISExpressify.csproj";

            Document = XDocument.Load(path);
        }
    }
}
