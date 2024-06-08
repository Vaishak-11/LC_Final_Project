using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;

namespace RecommendationEngineServer {
    class Program
    {
        static IUserService _userService;

        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("appsettings.json")
                                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

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
                Console.WriteLine($"New client connected: {client.Client.RemoteEndPoint}");
                HandleClient(client);
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received request from {client.Client.RemoteEndPoint}: {request}");

            ServerResponse response = ProcessRequest(request).Result;
            byte[] responseData = Encoding.ASCII.GetBytes($"{response.Name}:{response.Value}");
            stream.Write(responseData, 0, responseData.Length);
            if (request.ToLower().Contains("exit"))
            {
                Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
                client.Close();
            }
        }

        static async Task<ServerResponse> ProcessRequest(string request)
        {
            string[] parts = request.Split(' ');
            string command = parts[0];

            switch (command.ToLower())
            {
                case "login":
                    if (parts.Length < 4)
                    {
                        return new ServerResponse { Name = "Error", Value = "Invalid login command. Usage: login <username> <password> <role>" };
                    }

                    string username = parts[1];
                    string password = parts[2];
                    string roleName = parts[3];
                    Role role = new Role { RoleName = roleName };

                    Console.WriteLine($"Processing login command: username={username}, role={roleName}");
                    return await _userService.Login(username, password, role);
                default:
                    return new ServerResponse { Name = "Error", Value = "Unknown command." };
            }
        }
    }
}

