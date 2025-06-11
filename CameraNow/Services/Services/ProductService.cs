using AutoMapper;
using Microsoft.AspNetCore.Http;
using Commons.Commons;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Product;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Datas.Extentions;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Services.Services
{
    public class ProductService 
        : BaseService<ProductViewModel, Product, CreateProductViewModel, UpdateProductViewModel>, IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IProductRepository repository, 
                                IUnitOfWork unitOfWork, IMapper mapper,
                                IImageService imageService,
                                ICategoryService categoryService,
                                IHttpContextAccessor httpContextAccessor) : base(repository, unitOfWork, mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Convert entity
        protected override Product ConvertToEntity(ProductViewModel dto)
        {
            return _mapper.Map<Product>(dto);
        }

        protected override Product ConvertToEntity(CreateProductViewModel dto)
        {
            return _mapper.Map<Product>(dto);
        }

        protected override Product ConvertToEntity(UpdateProductViewModel dto)
        {
            return _mapper.Map<Product>(dto);
        }

        protected override ProductViewModel ConvertToViewModel(Product entity)
        {
            return _mapper.Map<ProductViewModel>(entity);
        }

        #endregion

        public override async Task<int> CreateAsync(CreateProductViewModel create)
        {           
            var entity = _mapper.Map<Product>(create);

            var uploadTask = new List<Task<string>>();
            uploadTask.Add(_imageService.UploadImage(create.Image, CommonExtensions.GenerateSEOTitle(create.Name), CommonExtensions.GenerateSEOTitle(create.Name)));

            var uploadResult = await Task.WhenAll(uploadTask);

            entity.Image = uploadResult[0];
            entity.Creation_Date = DateTime.UtcNow;
            entity.Creation_By = _httpContextAccessor.HttpContext.User.Identity.Name;

            var res = _repository.Add(entity);

            await _imageService.UploadMoreImage(create.MoreImage, res.ID, CommonExtensions.GenerateSEOTitle(res.Name));
            
            return await _unitOfWork.CommitAsync();
        }

        public override async Task<ProductViewModel> GetByIdAsync(Guid id, string[] includes = null)
        {
            var entity = _repository.GetSingleByCondition(e => e.ID == id, new string[] { "Images", "ProductCategory"});
            var vm = _mapper.Map<ProductViewModel>(entity);
            return vm;
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

        public override async Task<int> DeleteAsync(Guid id)
        {
            var product = _repository.GetSingleByCondition(x => x.ID == id, new[] { "Images" });

            if (!string.IsNullOrEmpty(product.Image_Public_Id))
                _imageService.DeleteImage(product.Image);

            foreach (var item in product.Images)
            {
                if (!string.IsNullOrEmpty(item.Public_Id))
                    _imageService.DeleteImage(item.Link);
            }

            await _repository.DeleteAsync(id);
            return await _unitOfWork.CommitAsync();
        }

        public int GetCount(string query, params object[] parameter)
        {
            return Convert.ToInt32(_repository.AddQuery(query, parameter));
        }

        public async Task<PaginationSet<ProductViewModel>> GetListAsync(ProductSpecification spec, PaginatedParams pageParams, string[] includes = null)
        {
            var entities = _repository.ApplyFilter(_repository.GetAll(includes), spec);

            var pageList = PaginatedList<Product>.Create(entities, pageParams.PageNumber, pageParams.PageSize);

            var pageListVm = _mapper.Map<PaginatedList<ProductViewModel>>(pageList);

            return new PaginationSet<ProductViewModel>(pageListVm.PageIndex, pageListVm.TotalPages, pageListVm.TotalCount, pageListVm);
        }

        public async Task<ProductViewModel> Update(Guid id, UpdateProductViewModel input, IFormFile? img)
        {
            var queryable = _repository.GetSingleById(id);
            
            var image = string.IsNullOrEmpty(queryable.Image) ? null : queryable.Image;
            _mapper.Map(input, queryable);
            queryable.Last_Modify_Date = DateTime.UtcNow;
            queryable.Last_Modify_By = _httpContextAccessor.HttpContext.User.Identity.Name;
            queryable.Image = image;

            // Ensure all DateTime fields are in UTC
            if (queryable.Creation_Date.HasValue && queryable.Creation_Date.Value.Kind == DateTimeKind.Unspecified)
                queryable.Creation_Date = DateTime.SpecifyKind(queryable.Creation_Date.Value, DateTimeKind.Utc);

            if (queryable.Last_Modify_Date.HasValue && queryable.Last_Modify_Date.Value.Kind == DateTimeKind.Unspecified)
                queryable.Last_Modify_Date = DateTime.SpecifyKind(queryable.Last_Modify_Date.Value, DateTimeKind.Utc);
            

            if (img != null)
            {
                var uploadResult = await _imageService.UploadImage(img, CommonExtensions.GenerateSEOTitle(input.Name), CommonExtensions.GenerateSEOTitle(input.Name));
                queryable.Image = uploadResult;
            }

            if (input.MoreImage != null)
            {
                await _imageService.UploadMoreImage(input.MoreImage, id, CommonExtensions.GenerateSEOTitle(queryable.Name));
            }

            var updated = _repository.Update(queryable);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ProductViewModel>(updated);
        }

        public async Task<double> UpdateAverageRating(Guid productId, double rating)
        {
            var product = await _repository.GetSingleByIdAsync(productId);

            if (product != null)
            {
                product.Rating = rating;
            }

            return await _unitOfWork.CommitAsync(); 
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllAsync(string[] includes = null)
        {
            return _repository.GetAll(includes).Select(x => _mapper.Map<ProductViewModel>(x));
        }

        public async Task UpdateBuyCount(Guid productId)
        {
            var product = _repository.GetSingleByCondition(x => x.ID == productId);
            product.Buy_Count += 1;
            _repository.Update(product);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateQuantity(Guid productId, int quantity)
        {
            var product = _repository.GetSingleByCondition(x => x.ID == productId);
            product.Quantity -= quantity;
            _repository.Update(product);
            await _unitOfWork.CommitAsync();
        }
    }
}
