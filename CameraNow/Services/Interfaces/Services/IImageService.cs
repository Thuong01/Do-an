using Microsoft.AspNetCore.Http;

namespace Services.Interfaces.Services
{
    public interface IImageService
    {
        Task<string> UploadImage(IFormFile img, string product_Name, string name_image);
        Task UploadMoreImage(List<IFormFile> imgs, Guid product_Id, string product_Name);
        //Task<(string url, string publicId)> UploadImageToCloudinary(IFormFile img, string rootFolderName, string folderName, string name_image);
        //Task UploadMoreImageToCloudinary(List<IFormFile> imgs, Guid product_Id, string rootFolderName, string folderName);
        //Task DeleteImageFromCloudinary(string publicId, string imagePath);
        bool DeleteImage(string relativeImagePath);
        void SaveChanged();
    }
}
