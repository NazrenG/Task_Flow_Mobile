using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.Abstract
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file);
    } 
}
