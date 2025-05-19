// SERVER   
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public Player(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Player() : this(0, 0) { }
}

class UDPSeverApp
{
    static int port = 5055;
    static Dictionary<IPEndPoint, Player> players =
        new Dictionary<IPEndPoint, Player>();

    static Player[] GetResponseForClient(IPEndPoint client)
    {
        List<Player> response = new List<Player>();
        foreach (var player in players)
        {
            if (!player.Key.Equals(client))
                response.Add(player.Value);
        }
        return response.ToArray();
    }
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        UdpClient server = new UdpClient(port);
        Console.WriteLine("Очікування повідомлень...");

        while (true)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = server.Receive(ref remoteEP);

            string message = Encoding.UTF8.GetString(data);
            Player? player = JsonSerializer.Deserialize<Player>(message);
            if (player == null) continue;
            if (players.ContainsKey(remoteEP)) players[remoteEP] = player;
            else players.Add(remoteEP, player);

            Player[] response = GetResponseForClient(remoteEP);
            server.Send(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)),
                remoteEP
            );
            Console.WriteLine($"{remoteEP}: {message}");
        }
    }
}
 
