using RecommendationEngineClient.Models;
using System.Net.Sockets;
using System.Text;

namespace RecommendationEngineClient
{
    public static class ServerCommunicator
    {
        public static ServerResponse SendRequestToServer(string request)
        {
            try
            {
                using (TcpClient client = new TcpClient("localhost", 5000))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.ASCII.GetBytes(request);
                        stream.Write(data, 0, data.Length);

                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        return new ServerResponse { Name = "ServerResponse", Value = response };
                    }
                }
            }
            catch (SocketException ex)
            {
                return new ServerResponse { Name = "Error", Value = $"SocketException: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new ServerResponse { Name = "Error", Value = $"Error: {ex.Message}" };
            }
        }
    }
}
    