using AutoMapper;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Datas.ViewModels.Image;

namespace Services.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ImageService(IImageRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void SaveChanged()
        {
            _unitOfWork.CommitAsync();
        }

        public async Task UploadMoreImage(List<IFormFile> imgs, Guid product_Id, string product_Name)
        {
            var img_path = "";
            var listImgs = new List<ProductImage>();

            if (imgs != null && imgs.Count() > 0) 
            {
                var folder_path = Path.Combine("wwwroot", "img", product_Name);

                if (!Directory.Exists(folder_path))
                {
                    Directory.CreateDirectory(folder_path);
                }

                foreach (var item in imgs)
                {
                    img_path = Path.Combine(folder_path, Path.GetFileName(item.FileName));

                    if (!System.IO.File.Exists(img_path))
                    {
                        using (var stream = new FileStream(img_path, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }
                    }

                    var path = $"/img/{product_Name}/{Path.GetFileName(item.FileName)}";

                    var vm = new CreateImageViewModel()
                    {
                        Link = path,
                        Product_Id = product_Id,
                    };

                    listImgs.Add(new ProductImage
                    {
                        Link = path,
                        Product_Id = product_Id
                    });
                }

                if (listImgs.Count() > 0)
                {
                    _repository.AddRange(listImgs);
                    _unitOfWork.Commit();
                }
            }
        }


        public async Task<string> UploadImage(IFormFile img, string product_Name, string name_image)
        {
            var img_path = "";

            if (img != null)
            {
                var folder_path = Path.Combine("wwwroot", "img", product_Name, name_image);

                if (!Directory.Exists(folder_path))
                {
                    Directory.CreateDirectory(folder_path);
                }

                img_path = Path.Combine(folder_path, Path.GetFileName(img.FileName));

                if (!System.IO.File.Exists(img_path))
                {
                    using (var stream = new FileStream(img_path, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }
                }

                img_path = $"/img/{product_Name}/{name_image}/{Path.GetFileName(img.FileName)}";
            }

            return img_path;
        }

        public bool DeleteImage(string relativeImagePath)
        {
            try
            {
                // Lấy đường dẫn gốc của wwwroot
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Ghép thành đường dẫn tuyệt đối đến file ảnh
                var fullPath = Path.Combine(wwwRootPath, relativeImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                // Nếu file tồn tại thì xóa
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return true;
                }

                return false; // File không tồn tại
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                Console.WriteLine($"Lỗi khi xóa ảnh: {ex.Message}");
                return false;
            }
        }

    }
}

