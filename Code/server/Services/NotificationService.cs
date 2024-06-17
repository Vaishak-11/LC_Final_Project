using AutoMapper;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
using System.Linq.Expressions;

namespace RecommendationEngineServer.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> AddNotification(Notification notification)
        {
            if (string.IsNullOrEmpty(notification.Message) || notification.UserId < 0)
                return 0;

            int notificationId =  await _notificationRepository.Add(notification);

            if (notificationId > 0)
            {
                List<User> users = (await _userRepository.GetList()).ToList();

                foreach (User user in users)
                {
                    if ((user.Role.RoleName.ToLower() == "employee" || (user.Role.RoleName.ToLower() == "chef") && notification.Message.ToLower().Contains("item")))
                    {
                       AddUserNotificationToDictionary(user.UserId, notificationId);
                    }
                    else if (user.Role.RoleName.ToLower() == "employee" && (notification.Message.Contains("item") || notification.Message.ToLower().Contains("recommended menu")))
                    {
                        AddUserNotificationToDictionary(user.UserId, notificationId);
                    }
                }
            }

            return notificationId;
        }

        public async Task<ServerResponse> GetNotifications(int userId = 0)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                Expression<Func<Notification, bool>> predicate = n => !n.IsDelivered;

                if (userId != 0)
                {
                    User user = await _userRepository.GetById(userId);

                    if (user == null)
                    {
                        response.Name = "Error";
                        response.Value = "User not found.";
                        return response;
                    }

                    switch (user.RoleId)
                    {
                        case 2:
                            predicate = n => n.Message.ToLower().Contains("item") && !n.IsDelivered;
                            break;
                        case 3:
                            predicate = n => (n.Message.ToLower().Contains("recommended menu") || n.Message.ToLower().Contains("item")) && !n.IsDelivered;
                            break;
                        default:
                            break;
                    }
                }

                List<Notification> notifications = (await _notificationRepository.GetList(predicate)).ToList();

                if (!notifications.Any())
                {
                    response.Name = "Success";
                    response.Value = "No new notifications.";
                }
                else
                {
                    response.Name = "Success";
                    response.Value = notifications;
                }
            }
            catch (Exception ex)
            {
                response.Name = "Error";
                response.Value = $"Error retrieving notifications: {ex.Message}";
            }

            return response;
        }


        public async Task<ServerResponse> UpdateNotificationStatus(int notificationId)
        {
            ServerResponse response = new ServerResponse();

            Notification notification = await _notificationRepository.GetById(notificationId);

            if (notification == null)
            {
                response.Name = "Error";
                response.Value = "Notification not found.";

                return response;
            }   

            notification.IsDelivered = !notification.IsDelivered? true : notification.IsDelivered;

            try
            {
                await _notificationRepository.Update(notification);
                response.Name = "Update";
                response.Value = "Updated succesfully";
            }
            catch (Exception ex)
            {
                response.Name = "Error";
                response.Value = ex.Message;
            }

            return response;
        }

        private static void AddUserNotificationToDictionary(int userId, int notificationId)
        {
            if (!UserData.NotificationDeliverStatus.ContainsKey(notificationId))
            {
                UserData.NotificationDeliverStatus.Add(notificationId, new List<NotificationStatus>());
            }

            UserData.NotificationDeliverStatus[notificationId].Add(new NotificationStatus
            {
                UserId = userId,
                IsDelivered = false
            });
        }
    }
}
