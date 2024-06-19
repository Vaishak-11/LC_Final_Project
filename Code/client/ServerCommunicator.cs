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
                        // Send the request to the server
                        byte[] data = Encoding.ASCII.GetBytes(request);
                        stream.Write(data, 0, data.Length);

                        byte[] buffer = new byte[1024];
                        StringBuilder responseBuilder = new StringBuilder();
                        int bytesRead = 0;

                        // Read the response from the server
                        do
                        {
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                        }
                        while (bytesRead > 0 && stream.DataAvailable);

                        string response = responseBuilder.ToString();

                        // Process the response
                        string[] responseParts = response.Split('#');
                        if (responseParts.Length >= 2)
                        {
                            ServerResponse serverResponse = new ServerResponse();
                            serverResponse.Name = responseParts[0];

                            if (request.Contains("login"))
                            {
                                serverResponse.Value = responseParts[1];

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

