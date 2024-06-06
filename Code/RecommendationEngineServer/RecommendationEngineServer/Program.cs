using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;

class Program
{
    static IUserService _userService;

    static void Main(string[] args)
    {
        string connectionString = "server=localhost;user=root;database=cafeteria;port=3306;password=your_password";

        var optionsBuilder = new DbContextOptionsBuilder<ServerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        ServerDbContext context = new ServerDbContext(optionsBuilder.Options);
        IUserRepository userRepository = new UserRepository(context);
        IEmployeeRepository employeeRepository = new EmployeeRepository(context);
        _userService = new UserService(userRepository, employeeRepository);

        StartServer();
    }

    static void StartServer()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Server started on port 5000.");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        string response = ProcessRequest(request).Result;
        byte[] responseData = Encoding.ASCII.GetBytes(response);
        stream.Write(responseData, 0, responseData.Length);

        client.Close();
    }

    static async Task<string> ProcessRequest(string request)
    {
        string[] parts = request.Split(' ');
        string command = parts[0];

        switch (command.ToLower())
        {
            case "login":
                if (parts.Length < 4)
                {
                    return "Invalid login command. Usage: login <username> <password> <role>";
                }

                string username = parts[1];
                string password = parts[2];
                string roleName = parts[3];
                Role role = new Role { RoleName = roleName };

                return await _userService.Login(username, password, role);
            default:
                return "Unknown command.";
        }
    }
}