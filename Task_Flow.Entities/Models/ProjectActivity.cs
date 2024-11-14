using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class ProjectActivity:IEntity
    {
        public int Id {  get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Text { get; set; }

        public virtual CustomUser User { get; set; }
        public virtual Project Project { get; set; }
        public ProjectActivity()
        {
            CreateTime=DateTime.UtcNow;
        }


    }
}
