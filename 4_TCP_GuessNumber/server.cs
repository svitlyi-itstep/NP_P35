// SERVER

using System.IO;
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

class User
{
    public TcpClient Client { get; set; }
    public string? Name { get; set; }
    public bool Ready { get; set; } = false;
}

class Server
{
    static TcpListener listener;
    static int port = 5050;
    static List<TcpClient> clients = new List<TcpClient>();
    static List<User> users = new List<User>();
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
        string value = Encoding.UTF8.GetString(buffer).Split(char.MinValue).First();
        Console.WriteLine($"Package got: {value}");
        return JsonSerializer.Deserialize<TcpPackage>(value
            );
    }

    static int CountReadyUsers()
    {
        int count = 0;
        foreach(var user in users.ToArray()) 
        {
            if(user.Ready) count++;
        }
        return count;
    }

    static string? GetUsernameFromClient(NetworkStream stream)
    {
        TcpPackage? package = GetPackage(stream);
        if (package == null || package.Data == null
            || package.MessageType != MessageType.Username) 
            return null;
        Console.WriteLine(package.Data);
        return package.Data.ToString();
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        // --- Отримання імені від клієнта
        string? username = GetUsernameFromClient(stream);
        if (username == null) 
        {
            client.Close();
            return;
        }


        clients.Add(client);
        User user = new User
        {
            Client = client,
            Name = username
        };
        users.Add(user);
        var endPoint = client.Client.RemoteEndPoint;       
        Broadcast($"{username} ({endPoint}) підключився до сервера");

        try
        {
            while (true) {
                TcpPackage? package = GetPackage(stream);
                if (package == null || package.Data == null) continue;
                if (package.MessageType == MessageType.Text)
                {
                    Broadcast($"{username}: {package.Data.ToString()}");
                }
                else if (package.MessageType == MessageType.Start)
                {
                    user.Ready = true;
                }
            }
        }
        catch (Exception ex)
        {
            Broadcast($"{username} ({endPoint}) відключився");
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
                SendPackage(client.GetStream(), new TcpPackage {
                    MessageType = MessageType.Text,
                    Data = message
                });
            }
            catch (Exception ex) { }
        }
    }
}
