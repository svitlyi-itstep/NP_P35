// CLIENT
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

class Location
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    
    public Location()
    {
        (X, Y) = (0, 0);
        (Width, Height) = (20, 20);
    }

    public void Draw()
    {
        for (int y = 0; y < Height; y++)
        {
            Console.SetCursorPosition(X, Y + y);
            for (int x = 0; x < Width; x++)
            {
                Player? player = GetPlayerByPosition(x, y);
                if (player == null) Console.Write("  ");
                else Console.Write("[]");
            }
        }
    }

    public Player? GetPlayerByPosition(int x, int y)
    {
        foreach(Player player in Players) 
            if(player.X == x && player.Y == y)
                return player;
        return null;
    }
}

class UDPClientApp
{
    static Location location = new Location();
    static Player player = new Player();
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        location.Players.Add(player);
        location.Players.Add(new Player(4, 10));

        while (true)
        {
            if(Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow: player.X -= 1; break;
                    case ConsoleKey.RightArrow: player.X += 1; break;
                    case ConsoleKey.UpArrow: player.Y -= 1; break;
                    case ConsoleKey.DownArrow: player.Y += 1; break;
                }
            }

            location.Draw();
        }
    }
}
/*
    Написати функцію, яка в окремому потоці буде відправляти на сервер
    інформацію про локального гравця та отримує відповідь з переліком
    всіх інших гравців на сервері.

    Також функція має оновлювати список гравців у об`єкті класу Location,
    щоб вони виводились на екран клієнта.
 */
