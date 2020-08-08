using System;
using System.IO;
using System.Text.RegularExpressions;

namespace IISExpressify.Tests
{
    public class Readme
    {
        public string InstallationVersion { get; }

        public Readme(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            InstallationVersion = "";

            var lines = File.ReadLines(path);
            foreach (var line in lines)
            {
                if (line.Contains("PackageReference"))
                {
                    var match = Regex.Match(line, "Version=\"(.+)-\\*\"");
                    if (match.Success)
                    {
                        InstallationVersion = match.Groups[1].Value;
                    }
                }
            }
        }
    }
}
