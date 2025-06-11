using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels;
using Datas.Extensions;
using Services.Interfaces.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    public class BaseAPIController<TViewModel, TCreate, TUpdate> 
        : ControllerBase where TViewModel : class where TCreate : class where TUpdate : class
    {
        private readonly IBaseService<TViewModel, TCreate, TUpdate> _baseService;
        private readonly ILogger<BaseAPIController<TViewModel, TCreate, TUpdate>> _logger;

        public BaseAPIController(IBaseService<TViewModel, TCreate, TUpdate> baseService, 
                                ILogger<BaseAPIController<TViewModel, TCreate, TUpdate>> logger)
        {
            _baseService = baseService;
            _logger = logger;
        }
        
    }
}
