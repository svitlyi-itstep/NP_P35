// CLIENT

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UDPClientApp
{
    static int port = 5055;
    static string serverIP = "127.0.0.1";
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        UdpClient client = new UdpClient();
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), port);

        while (true)
        {
            string message = "Hello!";
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, serverEP);
            Console.WriteLine("Повідомлення надіслано!");

            byte[] response = client.Receive(ref serverEP);
            string answer = Encoding.UTF8.GetString(response);
            Console.WriteLine($"Отримано відповідь: {answer}");
            Console.ReadLine();
        }
    }
}

/* 
    Реалізувати чат через протокол UDP. Сторона клієнта:
    При підключенні користувач вводить свій юзернейм, який
    передається серверу. Після цього програма паралельно приймає
    повідомлення від сервера, виводячи їх на екран та дає
    можливість користувачу вводити повідомлення та відправляти
    їх на сервер.
*/
