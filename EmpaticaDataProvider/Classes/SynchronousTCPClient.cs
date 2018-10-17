using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EmpaticaDataProvider.Classes;

public class SynchronousTCPClient
{
    #region Vars
    // The server ip for the BLE Empatica Server.
    private const string ServerAddress = "127.0.0.1";
    // The port number for the BLE Empatica Server.
    private const int ServerPort = 5555;
    // String containing the filtered message split on null. 

    private static string stringToSend;
    #endregion

    // Data buffer for incoming data.  
    static byte[] bytes = new byte[4096];

    // The response from the remote device.
    private String _response = String.Empty;
    
    // Create a TCP/IP  socket.  
    static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    CreateTCPCommand ctcpcommand = new CreateTCPCommand();

    private static void StartSyncTCPClient()
    {
        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(ServerAddress) } };
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, ServerPort);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                client.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}",client.RemoteEndPoint.ToString());
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void CloseTCPConnection()
    {
        try
        {
            // Release the socket.  
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }

    private static void SendTCPMessage()
    {
        try
        {
            // Encode the data string into a byte array.  
            byte[] tcpCommand = Encoding.ASCII.GetBytes(stringToSend + Environment.NewLine);
            // Send the data through the socket.  
            client.Send(tcpCommand);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ReceiveTCPMessage()
    {
        try
        {
            // Receive the response from the remote device.  
            client.Receive(bytes);
            Console.WriteLine(Encoding.ASCII.GetString(bytes));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void TCPRoutine(string DataStream)
    {
        StartSyncTCPClient();
        SendTCPMessage()
        ctcpcommand.MakeTCPCommands(DataStream);
        CreateTCPCommand.CheckReceivedMessage();


    }
}