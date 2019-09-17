using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    [Serializable()]
    public class InvalidRequestUriException : Exception
    {
        public InvalidRequestUriException(string message)
            : base(message)
        { }
    }
}
