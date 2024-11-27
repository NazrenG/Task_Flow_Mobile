using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.DataAccess;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Abstract
{
    public interface IChatDal  :IEntityRepository<Chat>{ }
}
