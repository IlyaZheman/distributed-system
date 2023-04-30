using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // IP-адрес и порт сервера
        var ipAddress = "127.0.0.1";
        var port = 8000;

        // Создаем экземпляр класса Socket
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Подключаемся к серверу
            await clientSocket.ConnectAsync(IPAddress.Parse(ipAddress), port);
            Console.WriteLine("Connected to server");

            while (true)
            {
                // Считываем сообщение с консоли
                Console.Write("Enter message: ");
                string message = Console.ReadLine();

                // Отправляем сообщение на сервер
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await clientSocket.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);

                // Принимаем ответ от сервера
                byte[] buffer = new byte[1024];
                int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("Received from server: " + response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        // Закрываем соединение с сервером
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        Console.WriteLine("Disconnected from server");
    }
}