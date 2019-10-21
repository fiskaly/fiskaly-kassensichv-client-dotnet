using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal static class Constants
    {
        internal const int MaxRetryInterval = 16; // seconds
        internal const int HttpRequestTimeout = 10; // seconds
        internal const string BaseAddress = "https://kassensichv.fiskaly.com/api/v0/";
    }
}
