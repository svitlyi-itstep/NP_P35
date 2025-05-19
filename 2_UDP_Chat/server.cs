// SERVER   

using System.Net;
using System.Net.Sockets;
using System.Text;

class UDPSeverApp
{
    static int port = 5055;
    static List<IPEndPoint> clients = new List<IPEndPoint>();
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

            if(!clients.Contains(remoteEP))
            {
                Console.WriteLine($"Перше повідомлення від {remoteEP}");
                clients.Add(remoteEP);
            }

            Console.WriteLine($"{remoteEP}: {message}");

            server.Send(Encoding.UTF8.GetBytes("OK"), remoteEP);
            Console.WriteLine("Відправлено відповідь: ОК");
        }
    }
}

/*
    Реалізувати чат через протокол UDP. Сторона сервера:
    Зробити зберігання підключених користувачів. При отриманні повідомлення
    від нового користувача розцінювати це повідомлення як ім'я користувача.
    Подальші повідомлення розсилати всім іншим підключеним користувачам.
*/
