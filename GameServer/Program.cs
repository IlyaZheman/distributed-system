using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var ipAddress = IPAddress.Any;
        var port = 8100;
        
        var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(ipAddress, port));
        listener.Listen(10);

        Console.WriteLine($"Server started on {ipAddress}:{port}");

        while (true)
        {
            var clientSocket = await listener.AcceptAsync();
            _ = Task.Run(() => HandleClientAsync(clientSocket));
        }
    }

    private static async Task HandleClientAsync(Socket clientSocket)
    {
        Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint}");

        // Receive data from client
        var buffer = new byte[1024];
        var receivedBytes = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);

        // Convert received data to string
        var message = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
        Console.WriteLine($"Received from {clientSocket.RemoteEndPoint}: {message}");

        // Send response to client
        var responseMessage = "Hello, client!";
        var responseBytes = Encoding.ASCII.GetBytes(responseMessage);
        await clientSocket.SendAsync(responseBytes, SocketFlags.None);

        // Close connection
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();

        Console.WriteLine($"Client disconnected: {clientSocket.RemoteEndPoint}");
    }
}