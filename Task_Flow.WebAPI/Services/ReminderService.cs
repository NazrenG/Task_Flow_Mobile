using Microsoft.AspNetCore.SignalR;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; 
        private readonly IHubContext<ConnectionHub> _context;

        public ReminderService(IServiceProvider serviceProvider,IHubContext<ConnectionHub> hubContext)
        {
            _context = hubContext;
            _serviceProvider = serviceProvider; 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<MailService>();
                    var notificationService=scope.ServiceProvider.GetRequiredService<INotificationService>();


                    var tomorrow = DateTime.UtcNow.AddDays(1).Date;
                    var userTasks = dbContext.UserTasks
                        .Where(t => t.Deadline.Date == tomorrow).
                        ToList();

                    var taskInProjects = dbContext.Works
                        .Where(t => t.Deadline.Date == tomorrow).
                        ToList();
                    var nottificationSetting = dbContext.NotificationSettings.ToList();
                    bool check = true;
                    foreach (var task in userTasks)
                    {
                        var user = dbContext.Users.FirstOrDefault(u => u.Id == task.CreatedById);
                        if (user != null)
                        {
                            foreach (var item in nottificationSetting)
                            {
                                if (item.UserId == user.Id && item.TaskDueDate == false)
                                {
                                    check = false; break;
                                }

                            }
                            if (check)
                            {
                                emailService.SendEmail(user.Email,
                                                           $"Hi {user.Firstname} {user.Lastname}, the deadline for your task titled {task.Title} is approaching. Please complete it.");
                                await notificationService.Add(new Notification
                                {
                                    IsCalendarMessage = true,
                                    Text = $"{task.Title} judicial mission almost over",
                                    UserId = user.Id
                                }); 
                             //   await _context.Clients.User(user.Id).SendAsync("ReminderRequestList");
                                await _context.Clients.User(user.Id).SendAsync("CalendarNotificationCount");
                             //   await _context.Clients.User(user.Id).SendAsync("CalendarNotificationList2");
                            }
                        }
                    }
                    foreach (var task in taskInProjects)
                    {
                        var user = dbContext.Users.FirstOrDefault(u => u.Id == task.CreatedById);
                        foreach (var item in nottificationSetting)
                        {
                            if (item.UserId == user.Id && item.TaskDueDate == false)
                            {
                                check = false; break;
                            }

                        }
                        if (check)
                        {
                            emailService.SendEmail(user.Email,
                                                       $"Hi {user.Firstname} {user.Lastname}, the deadline for your task titled {task.Title} is approaching. Please complete it.");

                            await notificationService.Add(new Notification
                            {
                                IsCalendarMessage = true,
                                Text = $"{task.Title} judicial mission almost over",
                                UserId = user.Id
                            });
                          //  await _context.Clients.User(user.Id).SendAsync("ReminderRequestList");
                            await _context.Clients.User(user.Id).SendAsync("CalendarNotificationCount");
                          //  await _context.Clients.User(user.Id).SendAsync("CalendarNotificationList2");
                        }
                    }


                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                };
            };
        }
    }
}
