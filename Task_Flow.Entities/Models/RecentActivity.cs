using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class RecentActivity:IEntity
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? Type { get; set; }
        public DateTime? Created { get; set; }
        public string? UserId { get; set; }
        public virtual CustomUser? User { get; set; }
        public RecentActivity()
        {
            Created = DateTime.UtcNow;
        }
    }
}
