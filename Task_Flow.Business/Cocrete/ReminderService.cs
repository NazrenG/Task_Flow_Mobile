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


                    var projects = dbContext.Projects
                  .Where(p => p.EndDate <= DateTime.Now.AddDays(1))
                  .ToList();

                    foreach (var project in projects)
                    {
                        if (project.CreatedBy != null)
                        {
                            // E-posta gönderin
                            emailService.SendEmail("nezrin.qu@gmail.com", "project.CreatedBy.UserName", "Deadline reminder",
                                $"Projenizin teslim süresi yaklaşıyor: {project.Title} - {project.EndDate:dd.MM.yyyy}");

                        }
                    }

                    // Günlük görevleri dashboard için hazırlayın
                    var dailyTasks = dbContext.Works
                        .Where(t => t.Deadline.Date == DateTime.Now.Date)
                        .ToList();
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                };
            };
        }
    }
}
