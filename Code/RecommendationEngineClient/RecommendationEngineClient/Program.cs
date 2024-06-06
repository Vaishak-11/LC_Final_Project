using RecommendationEngineClient;
using System;
using System.Data.Common;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Thread notificationThread = new Thread(ServerCommunicator.ConnectToNotificationServer);
        notificationThread.Start();

        



        while (true)
        {
            Console.WriteLine("Enter command (login, addmenu, recommend, feedback, exit):");
            string command = Console.ReadLine();

            if (command.ToLower() == "exit")
            {
                break;
            }

            string request = ServerRequestBuilder.BuildRequest(command);
            if (request != null)
            {
                string response = ServerCommunicator.SendRequestToServer(request);
                Console.WriteLine($"Server response: {response}");
            }
        }
    }
}
