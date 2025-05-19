// CLIENT

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

class Message
{
    public string user { get; set; }
    public string text { get; set; }
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
        return Encoding.UTF8.GetString(buffer)
            .Split(char.MinValue).First();
    }
    
    static void ReadingFromServer()
    {
        while (true)
        {
            try
            {
                
                Message? message =
                    JsonSerializer.Deserialize<Message>(
                        GetMessage()
                    );

                if (message != null)
                {
                    Console.WriteLine($"[{message.user}] {message.text}");
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                break; }
        }
    }
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.Write("Введіть своє ім'я: ");
        string name = Console.ReadLine();

        Console.WriteLine("Підключення до сервера...");
        TcpClient client = new TcpClient(serverIP, port);
        Console.WriteLine("Успішно підключено до сервера!");

        // --- Відправка імені на сервер
        stream = client.GetStream();
        SendMessage(name);

        Thread serverOutputThread = new Thread(ReadingFromServer);
        serverOutputThread.Start();

        while(true)
        {
            string messageText = Console.ReadLine();
            SendMessage(
                JsonSerializer.Serialize(
                    new Message{ user=name, text=messageText }
                )
            );
        }

        Console.WriteLine("Натисніть Enter, щоб вийти...");
        Console.ReadLine();
    }
}
