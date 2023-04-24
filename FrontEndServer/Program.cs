using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FrontEndServer;

internal class Program
{
    private static void Main(string[] args)
    {
        var server = new ServerObject();
        server.Start();
    }
}

internal class ServerObject
{
    private readonly Socket _server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private readonly List<Socket> _connectedClients = new();
    private readonly Dictionary<int, List<Thread>> _receiveThreadList = new();

    public void Start()
    {
        _server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));
        _server.Listen(100);

        WaitClientConnection();
    }

    private void WaitClientConnection()
    {
        var index = 1;
        while (true)
        {
            Console.WriteLine("The number of current links:" + _receiveThreadList.Count);
            Console.WriteLine("Waiting for the client's link:");
            var clientSocket = _server.Accept();
            Console.WriteLine("{0} is successful!", clientSocket.RemoteEndPoint);
            _connectedClients.Add(clientSocket);

            var receive = new Thread(ReceiveMessage);
            receive.Start(new ArrayList { index, clientSocket });

            var send = new Thread(SendMessage);
            send.Start(new ArrayList { index, clientSocket });

            _receiveThreadList.Add(index, new List<Thread> { receive, send });
            index++;
        }
    }

    private void ReceiveMessage(object? clientSockets)
    {
        var arraylist = clientSockets as ArrayList;
        var index = (int)arraylist[0];
        var clientSocket = arraylist[1] as Socket;
        while (true)
        {
            try
            {
                var encodingMessage = new byte[1024];
                var count = clientSocket.Receive(encodingMessage);
                var message = Encoding.UTF8.GetString(encodingMessage, 0, count);
                Console.WriteLine("{0} Send you message: {1}", clientSocket.RemoteEndPoint, message);
            }
            catch (Exception)
            {
                //Termination thread when the client is connected
                Console.WriteLine("The code for the code: {0} has left!", index);
                _receiveThreadList[index][0].Abort();
            }
        }
    }

    private void SendMessage(object? clientSockets)
    {
        var arraylist = clientSockets as ArrayList;
        var index = (int)arraylist[0];
        var clientSocket = arraylist[1] as Socket;
        while (true)
        {
            try
            {
                Console.WriteLine("Please enter the message you want to send:");
                var message = Console.ReadLine();
                var encodingMessage = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(encodingMessage);
            }
            catch (Exception)
            {
                Console.WriteLine("The code for the code: {0} has left! Message failed!");
                _receiveThreadList[index][1].Abort();
                _receiveThreadList.Remove(index);
            }
        }
    }
}