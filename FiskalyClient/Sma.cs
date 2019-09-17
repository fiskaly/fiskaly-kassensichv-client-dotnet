using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal static class Sma
    {
        [DllImport("com.fiskaly.kassensichv.sma-windows-386.dll", EntryPoint = "Invoke")]
        internal static extern IntPtr Invoke_32([In] byte[] request);

        [DllImport("com.fiskaly.kassensichv.sma-windows-386.dll", EntryPoint = "Free")]
        internal static extern void Free_32(IntPtr response);

        [DllImport("com.fiskaly.kassensichv.sma-windows-amd64.dll", EntryPoint = "Invoke")]
        internal static extern IntPtr Invoke_64([In] byte[] request);

        [DllImport("com.fiskaly.kassensichv.sma-windows-amd64.dll", EntryPoint = "Free")]
        internal static extern void Free_64(IntPtr response);

        internal static IntPtr Invoke([In] byte[] request)
        {
            return IntPtr.Size == 8 ? Invoke_64(request) : Invoke_32(request);
        }

        internal static void Free(IntPtr response)
        {
            if (IntPtr.Size == 8)
            {
                Free_64(response);
            }
            else
            {
                Free_32(response);
            }
        }

        internal static string SignTx(string payload)
        {
            var id = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var data = $"{{ \"jsonrpc\": \"2.0\", \"method\": \"sign-transaction\", \"params\": [{payload}], \"id\": \"{id}\"}}";
            var reqByt = Encoding.UTF8.GetBytes(data);
            var resPtr = Invoke(reqByt);
            var resStr = Marshal.PtrToStringAnsi(resPtr);
            Free(resPtr);
            return resStr;
        }
    }
}
