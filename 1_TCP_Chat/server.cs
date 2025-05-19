// SERVER

using System.IO;
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

class Server
{
    static TcpListener listener;
    static int port = 5050;
    static List<TcpClient> clients = new List<TcpClient>();
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine("Сервер запущено");
        Console.WriteLine("Очікування підключень...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    static void SendMessage(NetworkStream stream, string message)
    {
        if (stream == null) return;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }
    static string GetMessage(NetworkStream stream, int bufsize = 1024)
    {
        if (stream == null) return "";
        byte[] buffer = new byte[bufsize];
        stream.Read(buffer, 0, bufsize);
        return Encoding.UTF8.GetString(buffer).Split(char.MinValue).First();
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        clients.Add(client);
        var endPoint = client.Client.RemoteEndPoint;
        // --- Отримання імені від клієнта
        NetworkStream stream = client.GetStream();
        string name = GetMessage(stream);
        //Broadcast($"{name} ({endPoint}) підключився до сервера");
        Broadcast(new Message
        {
            user = "SERVER",
            text = $"{name} ({endPoint}) підключився до сервера"
        });
        try
        {
            while (true) {
                Message? clientMessage = 
                    JsonSerializer.Deserialize<Message>(
                        GetMessage(stream)
                    );

                if (clientMessage != null)
                {
                    //Broadcast($"[{clientMessage.user}] {clientMessage.text}");
                    Broadcast(clientMessage);
                }
            }
        }
        catch (Exception ex)
        {
            // Console.WriteLine(ex.Message);
            Broadcast($"{name} ({endPoint}) відключився");
        }
        finally
        {
            lock (clients) clients.Remove(client);
            client.Close();
        }
    }

    static void Broadcast(string message)
    {
        Console.WriteLine(message);
        foreach (var client in clients)
        {
            try
            {
                SendMessage(client.GetStream(), message);
            }
            catch (Exception ex) { }
        }
    }
    static void Broadcast(Message message)
    {
        Console.WriteLine($"[{message.user}] {message.text}");
        foreach (var client in clients)
        {
            try
            {
                SendMessage(client.GetStream(), JsonSerializer.Serialize(
                    new Message { 
                        user = message.user, 
                        text = message.text 
                    }
                ));
            }
            catch (Exception ex) { }
        }
    }
}
