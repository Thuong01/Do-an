using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Datas.Extentions;

namespace Services.Services
{
    public class CategoryService 
        : BaseService<ProductCategoryViewModel, ProductCategory, CreateProductCategoryViewModel, UpdateProductCategoryViewModel>, ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(ICategoryRepository repository, 
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IImageService image,
            IHttpContextAccessor httpContextAccessor) : base(repository, unitOfWork, mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = image;
            _httpContextAccessor = httpContextAccessor;
        }
        #region Mapping data
        protected override ProductCategory ConvertToEntity(ProductCategoryViewModel dto)
        {
            return _mapper.Map<ProductCategory>(dto);
        }

        protected override ProductCategory ConvertToEntity(CreateProductCategoryViewModel dto)
        {
            return _mapper.Map<ProductCategory>(dto);
        }

        protected override ProductCategory ConvertToEntity(UpdateProductCategoryViewModel dto)
        {
            return _mapper.Map<ProductCategory>(dto);
        }

        protected override ProductCategoryViewModel ConvertToViewModel(ProductCategory entity)
        {
            return _mapper.Map<ProductCategoryViewModel>(entity);
        }
        #endregion

        public override async Task<int> CreateAsync(CreateProductCategoryViewModel create)
        {
            var pc = _mapper.Map<ProductCategory>(create);
            var uploadList = new List<Task<string>>();
            uploadList.Add(_imageService.UploadImage(create.Image, create.Id.ToString(), create.Id.ToString()));
            var uploadResult = await Task.WhenAll(uploadList);

            pc.Creation_Date = DateTime.UtcNow;
            pc.Creation_By = _httpContextAccessor.HttpContext.User.Identity.Name;
            pc.Image = uploadResult[0].ToString();

            var res = _repository.Add(pc);

            if (res == null)
                return 0;

            return await _unitOfWork.CommitAsync();
        }

        public override async Task<int> DeleteRangeAsync(List<Guid> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                _repository.DeleteMulti(x => ids.Contains(x.ID));
                return await _unitOfWork.CommitAsync();
            }

            return 0;
        }

        public async Task<PaginationSet<ProductCategoryViewModel>> GetListAsync(BaseSpecification spec, PaginatedParams pageParams, string[] includes = null)
        {
            var queryable = _repository.ApplyFilter(_repository.GetAll(), spec);
            var pagedList = PaginatedList<ProductCategory>.Create(queryable, pageParams.PageNumber, pageParams.PageSize);
            var pagedListVm = _mapper.Map<PaginatedList<ProductCategoryViewModel>>(pagedList);
            return new PaginationSet<ProductCategoryViewModel>(pagedListVm.PageIndex, pagedListVm.TotalPages, pagedListVm.TotalCount, pagedListVm);
        }

        public async Task<IEnumerable<ProductCategoryViewModel>> GetListParentAsync(BaseSpecification spec)
        {
            var queryable = await _repository.ApplyFilter(_repository.GetAll(), spec).ToListAsync();

            return _mapper.Map<IEnumerable<ProductCategoryViewModel>>(queryable);
        }

        public async Task<ProductCategoryViewModel> Update(Guid id, UpdateProductCategoryViewModel input, IFormFile img = null)
        {
            // Tạo các tác vụ bất đồng bộ
            var getProductTask = Task.Run(() => _repository.GetSingleById(id));
            Task<string> uploadImageTask = null;

            // Nếu có ảnh, bắt đầu tải lên Cloudinary song song
            if (img != null)
            {
                uploadImageTask = _imageService.UploadImage(img, input.ID.ToString(), input.ID.ToString())
                    .ContinueWith(t => (t.Result));
            }

            // Chờ cả hai tác vụ hoàn thành
            await Task.WhenAll(getProductTask, uploadImageTask ?? Task.CompletedTask);

            var inforAfterUpdate = getProductTask.Result;

            var queryable = getProductTask.Result;
            var createdDate = queryable.Creation_Date;
            var createdBy = queryable.Creation_By;
            var image = queryable.Image ?? null;
            var img_public_id = queryable.Image_Public_Id ?? null;

            _mapper.Map(input, queryable);

            if (uploadImageTask != null)
            {
                // Nếu có ảnh mới, cập nhật đường dẫn và public ID
                queryable.Image = uploadImageTask.Result;
                // Xóa ảnh cũ từ Cloudinary
                if (img_public_id != null)
                {
                    _imageService.DeleteImage(image);
                }
            }

            queryable.Creation_Date = createdDate;
            queryable.Creation_By = createdBy;
            queryable.Last_Modify_Date = DateTime.UtcNow;
            queryable.Last_Modify_By = _httpContextAccessor.HttpContext.User.Identity.Name;
            var updated = _repository.Update(queryable);
            _unitOfWork.Commit();
            return _mapper.Map<ProductCategoryViewModel>(updated);

        }
    }
}
