using Microsoft.AspNetCore.Http; 
using Task_Flow.Business.Abstract; 
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Task_Flow.WebAPI.Settings;

namespace Task_Flow.Business.Cocrete
{
    public class FileService:IFileService
    {
        private IConfiguration _configuration;
        private CloudinarySettings _cloudinarySettings;
        private Cloudinary _cloudinary;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
            _cloudinarySettings = _configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            Account account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> SaveFile(IFormFile formFile)
        {
            var file = formFile;
            var uploadedResult = new ImageUploadResult();
            if (file?.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    uploadedResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadedResult != null)
                    {
                        return uploadedResult.Url.ToString();
                    }
                }
            }
            return "";
        }
    }
}
