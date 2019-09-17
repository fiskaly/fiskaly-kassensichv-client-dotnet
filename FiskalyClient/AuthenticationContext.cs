using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal class AuthenticationContext
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshInterval { get; set; }
    }
}
