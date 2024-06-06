using System;
using System.Net.Sockets;
using System.Text;

public static class ServerCommunicator
{
    public static string SendRequestToServer(string request)
    {
        try
        {
            TcpClient client = new TcpClient("localhost", 5000);
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(request);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            client.Close();
            return response;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public static void ConnectToNotificationServer()
    {
        try
        {
            TcpClient client = new TcpClient("localhost", 5001);
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string notification = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Notification: {notification}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Notification connection error: {ex.Message}");
        }
    }
}
