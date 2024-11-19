using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Data;

namespace Task_Flow.Business.Cocrete
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ReminderService(IServiceProvider serviceProvider)
        {
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

                            }
                        }
                    }
                    foreach (var task in taskInProjects)
                    {
                        var user = dbContext.Users.FirstOrDefault(u => u.Id == task.CreatedById);
                        foreach (var item in nottificationSetting)
                        {
                            if (item.UserId == user.Id && item.TaskDueDate==false)
                            {
                                check = false; break;
                            }

                        }
                        if (check)
                        {
                            emailService.SendEmail(user.Email,
                                                       $"Hi {user.Firstname} {user.Lastname}, the deadline for your task titled {task.Title} is approaching. Please complete it.");

                        }
                    }


                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                };
            };
        }
    }
}
