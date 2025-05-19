using System.Net;
using System.Text;
using System.Text.Json;

class FTPClient
{
    static string ftpHost = "ftp://ftpupload.net";
    static string username = "ftp-username";
    static string password = "ftp-password";

    static string FtpListDirectory(string path="")
    {
        try
        {
            // Створюємо об`єкт запиту
            string uri = $"{ftpHost}/{path}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(username, password);

            // Виконуєм запит
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            // Створюємо об`єкт для зчитування потоку даних
            StreamReader reader = new StreamReader(response.GetResponseStream());

            // Виводимо результат на екран
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return "";
    }

    static void FtpDownloadFile(string remotePath, string localPath="")
    {
        try
        {
            if (localPath.Length == 0) localPath = 
                    $"{Path.GetDirectoryName(Environment.ProcessPath)}/" +
                    $"{Path.GetFileName(remotePath)}";
            
            // Створюємо об`єкт запиту
            string uri = $"{ftpHost}/{remotePath}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(username, password);

            // Виконуєм запит
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            // Створюємо об`єкт для зчитування потоку даних
            using (Stream responseStream = response.GetResponseStream())
            using (FileStream ft = new FileStream(localPath, FileMode.Create))
            {
                responseStream.CopyTo(ft);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.WriteLine(FtpListDirectory());
        Console.WriteLine("Downloading file...");
        FtpDownloadFile("readme.txt");
    }
}
