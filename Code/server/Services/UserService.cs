using Microsoft.Extensions.Logging;
using RecommendationEngineServer.Helpers;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace RecommendationEngineServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IEmployeeRepository employeeRepository, IRoleRepository roleRepository, INotificationService notificationService, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
            _notificationService = notificationService;
            _logger = logger;

            InitializeNotificationDeliveryStatus().Wait();
        }

        public async Task<ServerResponse> Login(UserLoginDTO userLogin)
        {
            ServerResponse response = new ServerResponse();

            if (userLogin.Role != null)
            {
                string loginMessage = string.Empty;
                int userId, roleId;

                if (userLogin.Role.ToLower() == "employee")
                {
                    Employee employee = await _employeeRepository.GetByEmployeeCode(userLogin.UserName) ?? throw new Exception("Login failed. Employee not found.");
                    (loginMessage, userId, roleId) = ValidateEmployeeLogin(employee, userLogin.UserName, userLogin.Password);
                }
                else
                {
                    User user = await _userRepository.GetUserByName(userLogin.UserName) ?? throw new Exception("Login failed. User not found."); ;
                    (loginMessage, userId, roleId) = ValidateUserLogin(user, userLogin.Password);
                }

                response.UserId = userId;
                response.RoleId = roleId;

                if (userId > 0)
                {
                    List<Notification> pendingNotifications = roleId != 1 ? await GetPendingNotifications(userId) : new List<Notification>();

                    var loginResponse = new LoginResponse
                    {
                        Message = loginMessage,
                        Notifications = pendingNotifications.Select(n => n.Message).ToList()
                    };

                    response.Name = "Success";
                    response.Value = JsonSerializer.Serialize(loginResponse);
                    _logger.LogInformation($"userId: {userId}, name: {userLogin.UserName}, message:{loginMessage}, Date: {DateTime.Now}");

                    UserData.UserId = response.UserId;
                    UserData.RoleId = response.RoleId;

                    await HandleNotificationDeliveryStatus(userId, pendingNotifications);
                }
                else
                {
                    _logger.LogError($"userId: {userId}, name: {userLogin.UserName}, message:Login failed. Invalid credentials., Date: {DateTime.Now}");
                    throw new Exception("Login failed. Invalid credentials.");
                }
            }
            else
            {
                _logger.LogError($"name: {userLogin.UserName}, message:Login failed.Invalid role., Date: {DateTime.Now}");
                throw new Exception("Login failed.Invalid role.");
            }

            return response;
        }

        public async Task<ServerResponse> RegisterUser(UserLoginDTO userRegister)
        {
            ServerResponse response = new ServerResponse();

            if (!string.IsNullOrEmpty(userRegister.Role))
            {
                Role roleDetails = await _roleRepository.GetRoleByName(userRegister.Role) ?? throw new Exception("Registration failed. Invalid role.");
                User existingUser = await _userRepository.GetUserByName(userRegister.UserName);

                if (existingUser != null)
                {
                    _logger.LogError($"name: {userRegister.UserName}, message:Registration failed. This username already exists., Date: {DateTime.Now}");
                    throw new Exception("Registration failed. This username already exists. Try with a different userName");
                }

                User user = new User { UserName = userRegister.UserName, Password = userRegister.Password, RoleId = roleDetails.RoleId };
                int userId = await _userRepository.Add(user);

                if (userRegister.Role.ToLower() == "employee")
                {
                    Employee newEmployee = new Employee { EmployeeCode = userRegister.EmployeeCode, UserId = userId };
                    int employeeId = await _employeeRepository.Add(newEmployee);

                    response.Name = "Register";
                    response.Value = (employeeId > 0) ? "Registration successful." : "Registration failed.";
                }
                else
                {
                    response.Name = "Register";
                    response.Value = (userId > 0) ? "Register successful." : "Register failed.";
                }

                _logger.LogInformation($"name: {userRegister.UserName}, message: {response.Value}, Date: {DateTime.Now}");
            }
            else
            {
                _logger.LogError($"name: {userRegister.UserName}, message: Registration failed. Invalid role., Date: {DateTime.Now}");
                throw new Exception("Registration failed. Invalid role.");
            }

            return response;
        }

        public async Task<ServerResponse> Logout(int userId)
        {
            UserData.UserId = 0;
            UserData.RoleId = 0;

            _logger.LogInformation($"userId: {userId}, message: Logout successful., Date: {DateTime.Now}");

            return ResponseHelper.CreateResponse("Logout", "Logout successful.");
        }

        public async Task<ServerResponse> UpdateProfile(EmployeeProfileDTO profile)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                Employee employee = (await _employeeRepository.GetList(predicate: e => e.UserId == profile.UserId)).FirstOrDefault();

                if (employee == null)
                {
                    _logger.LogError($"userId: {profile.UserId}, message: Profile update failed. Employee not found., Date: {DateTime.Now}");
                    throw new Exception("Profile update failed. Employee not found.");
                }

                employee.FoodDiet = profile.FoodDiet;
                employee.SpiceLevel = profile.SpiceLevel;
                employee.Cuisine = profile.Cuisine;

                await _employeeRepository.Update(employee);

                _logger.LogInformation($"userId: {profile.UserId}, message: Profile updated successfully., Date: {DateTime.Now}");
                response = ResponseHelper.CreateResponse("Profile", "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"userId: {profile.UserId}, message: Profile update failed. {ex.Message}, Date: {DateTime.Now}");
                response = ResponseHelper.CreateResponse("Profile", ex.Message);
            }

            return response;
        }

        private static (string message, int userId, int roleId) ValidateEmployeeLogin(Employee employee, string name, string password)
        {
            if (employee != null && employee.EmployeeCode == name && employee.User.Password == password)
            {
                return ("Login successful.", employee.UserId, employee.User.RoleId);
            }

            return ("Login failed.", 0, 0);
        }

        private static (string message, int userId, int roleId) ValidateUserLogin(User user, string password)
        {
            if (user != null && user.Password == password)
            {
                return ("Login successful.", user.UserId, user.RoleId);
            }

            return ("Login failed. Invalid Credentials", 0, 0);
        }

        private async Task InitializeNotificationDeliveryStatus()
        {
            try
            {
                List<Notification> notifications = (await _notificationService.GetNotifications()).Value as List<Notification>;

                if (notifications.Any())
                {
                    foreach (var notification in notifications)
                    {
                        Expression<Func<User, bool>> predicate = null;

                        if (!UserData.NotificationDeliverStatus.ContainsKey(notification.NotificationId))
                        {
                            UserData.NotificationDeliverStatus[notification.NotificationId] = new List<NotificationStatus>();
                        }

                        if (notification.Message.Contains("item") || notification.Message.Contains("dish") || notification.Message.Contains("recommended menu"))
                        {
                            predicate = u => u.Role.RoleName.ToLower() == "employee";
                        }
                        else if (notification.Message.Contains("item"))
                        {
                            predicate = u => u.Role.RoleName.ToLower() == "chef";
                        }

                        List<User> users = (await _userRepository.GetList(predicate: predicate)).ToList();
                        if (users.Count > 0)
                        {
                            foreach (var user in users)
                            {
                                if (!UserData.NotificationDeliverStatus[notification.NotificationId].Any(ns => ns.UserId == user.UserId))
                                {
                                    UserData.NotificationDeliverStatus[notification.NotificationId].Add(new NotificationStatus
                                    {
                                        UserId = user.UserId,
                                        IsDelivered = false
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing NotificationDeliverStatus: {ex.Message}");
            }
        }

        private async Task HandleNotificationDeliveryStatus(int userId, List<Notification> pendingNotifications)
        {
            try
            {
                foreach (var notification in pendingNotifications)
                {
                    var notificationStatus = UserData.NotificationDeliverStatus[notification.NotificationId].FirstOrDefault(ns => ns.UserId == userId);
                    if (notificationStatus != null)
                    {
                        notificationStatus.IsDelivered = true;
                    }
                    else
                    {
                        UserData.NotificationDeliverStatus[notification.NotificationId].Add(new NotificationStatus { UserId = userId, IsDelivered = true });
                    }

                    if (IsNotificationDeliveredToAllUsers(notification.NotificationId))
                    {
                        await _notificationService.UpdateNotificationStatus(notification.NotificationId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling notification delivery status: {ex.Message}");
            }
        }

        private bool IsNotificationDeliveredToAllUsers(int notificationId)
        {
            foreach (var userNotifications in UserData.NotificationDeliverStatus[notificationId])
            {
                if (!userNotifications.IsDelivered)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<List<Notification>> GetPendingNotifications(int userId)
        {
            ServerResponse notificationResponse = await _notificationService.GetNotifications(userId);
            var notifications = notificationResponse.Value as List<Notification> ?? new List<Notification>();

            foreach (var notification in notifications)
            {
                if (!UserData.NotificationDeliverStatus.ContainsKey(notification.NotificationId))
                {
                    UserData.NotificationDeliverStatus[notification.NotificationId] = new List<NotificationStatus>();
                }
            }

            var pendingNotifications = notifications.Where(n => !UserData.NotificationDeliverStatus[n.NotificationId].Any(ns => ns.UserId == userId && ns.IsDelivered)).ToList();

            return pendingNotifications;
        }
    }
}
