using System;
using System.Diagnostics;
using System.IO;

namespace IISExpressify
{
    public class IisExpress : IIisExpress, IDisposable
    {
        public static HttpIisExpressOptions Http() =>
             new HttpIisExpressOptions(Start);

        public static HttpsIisExpressOptions Https() =>
            new HttpsIisExpressOptions(Start);

        Process _process;
        public Uri BaseUri { get; }

        static string GetProcessFileName() =>
            Path.Combine(
                Environment.Is64BitOperatingSystem
                    ? Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
                    : Environment.GetEnvironmentVariable("PROGRAMFILES"),
                "IIS Express",
                "iisexpress.exe"
            );

        static Process StartProcess(string arguments) =>
            Process.Start(
                new ProcessStartInfo(GetProcessFileName(), arguments)
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false
                }
            );

        internal static IisExpress Start(string scheme, ushort port, string arguments) =>
            new IisExpress(
                new UriBuilder(scheme, "localhost", port).Uri,
                StartProcess(arguments)
            );

        IisExpress(Uri baseUri, Process process)
        {
            _process = process;
            BaseUri = baseUri;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                _process.StandardInput.WriteLine("Q");
                if (!_process.WaitForExit((int)TimeSpan.FromSeconds(1).TotalSeconds))
                {
                    _process.Kill();
                }
                _process = null;

                disposedValue = true;
            }
        }

        ~IisExpress()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
