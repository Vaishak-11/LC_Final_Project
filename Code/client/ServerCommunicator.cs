using RecommendationEngineClient.Models;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

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

                        string[] responseParts = response.Split('#');
                        if (responseParts.Length >= 4)
                        {
                            ServerResponse serverResponse = new ServerResponse();
                            serverResponse.Name = responseParts[0];

                            if (request.Contains("login"))
                            {
                                var responseValueParts = responseParts[1].Split('%');

                                if (responseValueParts.Length > 1)
                                {
                                    serverResponse.Value = new
                                    {
                                        Message = responseValueParts[0],
                                        Notifications = JsonSerializer.Deserialize<List<string>>(responseValueParts[1])
                                    };
                                }
                                else
                                {
                                    serverResponse.Value = responseValueParts[0];
                                }

                                serverResponse.UserId = Convert.ToInt32(responseParts[2]);
                                serverResponse.RoleId = Convert.ToInt32(responseParts[3]);
                            }
                            else
                            {
                                if (IsJsonObject(responseParts[1]))
                                {
                                    serverResponse.Value = JsonSerializer.Deserialize<object>(responseParts[1]);
                                }
                                else
                                {
                                    serverResponse.Value = responseParts[1];
                                }
                            }

                            return serverResponse;
                        }
                        else
                        {
                            return new ServerResponse
                            {
                                Name = "Error",
                                Value = "Invalid response format"
                            };
                        }
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

        private static bool IsJsonObject(string str)
        {
            str = str.Trim();
            return (str.StartsWith("{") && str.EndsWith("}")) || 
                   (str.StartsWith("[") && str.EndsWith("]"));   
        }
    }
}
    