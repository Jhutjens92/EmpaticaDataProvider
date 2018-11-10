using System.Net.Sockets;
using System.Text;

namespace EmpaticaDataProvider
{
    public class StateObject
    {
        /// <summary>Client socket.</summary>
        public Socket WorkSocket;
        
        /// <summary>Size of receive buffer.</summary>
        public const int BufferSize = 4096;
        
        /// <summary>Receive buffer.</summary>
        public readonly byte[] Buffer = new byte[BufferSize];
        
        /// <summary>Received data string.</summary>   
        public readonly StringBuilder Sb = new StringBuilder();
    }
}