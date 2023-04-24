using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

internal class Program
{
    private const string Ip = "127.0.0.1";
    private const int Port = 8888;

    private static void Main(string[] args)
    {
        var client = new Client();
        client.ConnectToServer(Ip, Port);

        var receive = new Thread(client.ReceiveMessage);
        var send = new Thread(client.SendMessage);

        receive.Start();
        send.Start();

        receive.Join();
        send.Join();

        Console.ReadKey();
    }
}

internal class Client
{
    private readonly Socket _clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public void ConnectToServer(string ip, int port)
    {
        // When the TCP is connected to the server, it only needs to be connected once because TCP is a long link.
        _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public void ReceiveMessage()
    {
        while (true)
        {
            var encodingMessage = new byte[1024];
            var count = _clientSocket.Receive(encodingMessage);
            var message = Encoding.UTF8.GetString(encodingMessage, 0, count);
            if (count > 0)
            {
                Console.WriteLine("Received news from {0} is: {1}", _clientSocket.RemoteEndPoint, message);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    public void SendMessage()
    {
        while (true)
        {
            Console.WriteLine("Please enter the message you want to send to the server:");
            var message = Console.ReadLine();
            var encodingMessage = Encoding.UTF8.GetBytes(message);
            _clientSocket.Send(encodingMessage);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}