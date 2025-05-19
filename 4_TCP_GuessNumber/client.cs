// CLIENT

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

enum MessageType
{
    Text,
    Username,
    Start,
    System
}

class TcpPackage
{
    public MessageType MessageType { get; set; }
    public object? Data { get; set; }
}


class Client
{
    static string serverIP = "127.0.0.1";
    static int port = 5050;
    static NetworkStream? stream = null;

    static void SendMessage(string message)
    {
        if (stream == null) return;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }
    static string GetMessage(int bufsize = 1024)
    {
        if (stream == null) return "";
        byte[] buffer = new byte[bufsize];
        stream.Read(buffer, 0, bufsize);
        return Encoding.UTF8.GetString(buffer);
    }
    static void SendPackage(NetworkStream stream, TcpPackage package)
    {
        if (stream == null) return;
        byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(package));
        stream.Write(buffer);
    }

    static TcpPackage? GetPackage(NetworkStream stream, int bufsize = 4096)
    {
        if (stream == null) return null;
        byte[] buffer = new byte[bufsize];
        stream.Read(buffer, 0, bufsize);
        return JsonSerializer.Deserialize<TcpPackage>(
            Encoding.UTF8.GetString(buffer).Split(char.MinValue).First());
    }
    static void ShowMessagesFromServer(NetworkStream stream)
    {
        while (true)
        {
            try
            {
                TcpPackage? package = GetPackage(stream);
                if (package == null || package.Data == null) continue;
                if (package.MessageType == MessageType.Text)
                {
                    Console.WriteLine(package.Data.ToString());
                }
            }
            catch (IOException) { break; }
            //catch (Exception e)
            {
                //Console.WriteLine($"{e}{e.Message}");
            }
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.Write("Введіть своє ім'я: ");
        string? name = Console.ReadLine();

        Console.WriteLine("Підключення до сервера...");
        TcpClient client = new TcpClient(serverIP, port);
        Console.WriteLine("Успішно підключено до сервера!");

        // --- Відправка імені на сервер
        stream = client.GetStream();
        SendPackage(stream, new TcpPackage { 
            MessageType = MessageType.Username,
            Data = name
        });
        Thread messageThread = new Thread(() => { ShowMessagesFromServer(stream); });
        messageThread.Start();
        while (true)
        {
            try
            {
                string? message = Console.ReadLine();
                if (message == null) continue;
                SendPackage(stream, new TcpPackage
                {
                    MessageType = MessageType.Text,
                    Data = message
                });
            }
            catch (IOException) { break; }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                break; }
        }

        Console.WriteLine("Натисніть Enter, щоб вийти...");
        Console.ReadLine();
    }
}
