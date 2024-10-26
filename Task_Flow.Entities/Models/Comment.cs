using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Comment:IEntity
    {
        public int Id { get; set; }
        public string? Context {  get; set; }
        public DateTime? Created { get; set; }  
        public int ? TaskForUserId { get; set; }
        public string ? UserId { get; set; }

        public virtual Work? TaskForUser { get; set; }
        public virtual CustomUser? User { get; set; }
        public Comment()
        {
            Created = DateTime.Now;    
        }

    }
}
