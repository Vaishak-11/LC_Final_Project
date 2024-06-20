using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Profiles;
using RecommendationEngineServer.Repositories;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;

namespace RecommendationEngineServer
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                var configuration = new ConfigurationBuilder()
                                 .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                 .AddJsonFile("appsettings.json")
                                 .Build();

                var services = new ServiceCollection();
                ConfigureServices(services, configuration);

                ServiceProvider = services.BuildServiceProvider();

                StartServer();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw new Exception(exception.Message);
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ServerDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IFoodItemRepository, FoodItemRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IRecommendedMenuRepository, RecommendedMenuRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFoodItemService, FoodItemService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IRecommendedMenuService, RecommendedMenuService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddAutoMapper(config => config.AddProfile<MapProfile>(), AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IRequestHandlerService, RequestHandlerService>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(configuration);
            });
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

                Thread clientThread = new Thread(() =>
                {
                    using (var scope = ServiceProvider.CreateScope())
                    {
                        var requestHandlerService = scope.ServiceProvider.GetRequiredService<IRequestHandlerService>();
                        HandleClient(client, requestHandlerService);
                    }
                });
                clientThread.Start();
                Console.WriteLine($"Started thread {clientThread.ManagedThreadId} for client {client.Client.RemoteEndPoint}");
            }
        }

        static void HandleClient(TcpClient client, IRequestHandlerService requestHandlerService)
        {
            Console.WriteLine($"Handling client {client.Client.RemoteEndPoint} on thread {Thread.CurrentThread.ManagedThreadId}");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received request from {client.Client.RemoteEndPoint}: {request}");
            ServerResponse response;

            try
            {
                response = requestHandlerService.ProcessRequest(request).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                response = new ServerResponse
                {
                    Name = "Error",
                    Value = ex.Message,
                };
            }

            byte[] responseData = Encoding.ASCII.GetBytes($"{response.Name}#{response.Value}#{response.UserId}#{response.RoleId}");
            stream.Write(responseData, 0, responseData.Length);

            if (request.ToLower().Contains("exit"))
            {
                Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
                client.Close();
            }
        }
    }
}

