using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FrontEndServer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Инициализируем IP-адрес и порт нашего сервера
        var ipAddress = IPAddress.Parse("127.0.0.1");
        var port = 8000;

        // Создаем сокет для прослушивания входящих подключений
        var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(ipAddress, port));
        listener.Listen(10);

        Console.WriteLine($"Сервер запущен и слушает порт {port}");

        while (true)
        {
            // Принимаем входящее соединение
            var clientSocket = await listener.AcceptAsync();
            Console.WriteLine($"Принято входящее соединение от {clientSocket.RemoteEndPoint}");

            // Получаем сообщение от клиента
            var buffer = new byte[1024];
            var bytesReceived = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            var messageFromClient = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine($"Получено сообщение от клиента: {messageFromClient}");

            // Подключаемся к серверу-назначению
            var destinationSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await destinationSocket.ConnectAsync("127.0.0.1", 8100);
            Console.WriteLine($"Установлено соединение с сервером-назначением {destinationSocket.RemoteEndPoint}");

            // Отправляем сообщение на сервер-назначение
            // var bytesSent = await destinationSocket.SendAsync(buffer, bytesReceived, SocketFlags.None);
            var sent = buffer.Take(bytesReceived).ToArray();
            var bytesSent = await destinationSocket.SendAsync(sent, SocketFlags.None);
            Console.WriteLine($"Отправлено сообщение на сервер-назначение: {messageFromClient}");

            // Получаем ответ от сервера-назначения
            var responseBuffer = new byte[1024];
            var responseBytesReceived = await destinationSocket.ReceiveAsync(responseBuffer, SocketFlags.None);
            var messageFromServer = Encoding.UTF8.GetString(responseBuffer, 0, responseBytesReceived);
            Console.WriteLine($"Получен ответ от сервера-назначения: {messageFromServer}");

            // Отправляем ответ клиенту
            // await clientSocket.SendAsync(responseBuffer, responseBytesReceived, SocketFlags.None);
            var send = responseBuffer.Take(responseBytesReceived).ToArray();
            await clientSocket.SendAsync(send, SocketFlags.None);
            Console.WriteLine($"Отправлен ответ клиенту: {messageFromServer}");

            // Закрываем соединение с сервером-назначением и клиентом
            destinationSocket.Shutdown(SocketShutdown.Both);
            destinationSocket.Close();
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}