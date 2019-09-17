using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal static class Constants
    {
        internal const int NumberOfAuthenticationRetryAttempts = 3;
        internal const string BaseAddress = "https://kassensichv.fiskaly.com/api/v0/";
    }
}
