using AutoMapper;
using Datas.Extensions;
using Datas.Extentions;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Feedback;
using Models.Models;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public FeedbackService(IFeedbackRepository feedbackRepository, IProductService productService,
            IUnitOfWork unitOfWork, IMapper mapper, IOrderService orderService)
        {
            _feedbackRepository = feedbackRepository;
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task<int> CreateAsync(FeedbackCreateViewModel input)
        {
            var res = _feedbackRepository.Add(_mapper.Map<Feedback>(input));

            var feedbacks = await _feedbackRepository.GetManyAsync(x => x.Product_ID == input.Product_ID);

            var avgFeedbackRate = 0.0;

            if (feedbacks != null && feedbacks.Count() > 0)
            {
                avgFeedbackRate = feedbacks.Sum(x => x.Rating) / feedbacks.Count();
            }
            else
            {
                avgFeedbackRate = res.Rating;
            }

            var order = await _orderService.UpdateOrderIsCommented(input.Order_ID, input.User_ID);

            var ratingResult = await _productService.UpdateAverageRating(input.Product_ID, avgFeedbackRate);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> DeleteFeedback(Guid fbId, Guid productId)
        {
            var fb = _feedbackRepository.GetSingleByCondition(x => x.ID == fbId && x.Product_ID == productId);

            if (fb != null)
            {
                await _feedbackRepository.DeleteAsync(fbId);
                return await _unitOfWork.CommitAsync();
            }

            return 0;
        }

        public async Task<FeedbackViewModel> GetFeedbacks(Guid productId, PaginatedParams pageParams)
        {
            var feedbacks = await _feedbackRepository.GetManyAsync(x => x.Product_ID == productId, new[] { "User", "Images" });

            double Rating_Average = 0.0;
            List<Stars> stars = new List<Stars>();

            if (feedbacks != null && feedbacks.Count() > 0)
            {
                var starsGroup = feedbacks.GroupBy(x => x.Rating);

                var star1Gr = starsGroup.FirstOrDefault(x => x.Key == 1);
                var star2Gr = starsGroup.FirstOrDefault(x => x.Key == 2);
                var star3Gr = starsGroup.FirstOrDefault(x => x.Key == 3);
                var star4Gr = starsGroup.FirstOrDefault(x => x.Key == 4);
                var star5Gr = starsGroup.FirstOrDefault(x => x.Key == 5);

                var star1 = new Stars
                {
                    star = "1",
                    SumStar = star1Gr != null ? star1Gr.Count() : 0,
                    Percent = star1Gr != null ? (double)star1Gr.Count() / feedbacks.Count() * 100 : 0
                };
                stars.Add(star1);

                var star2 = new Stars
                {
                    star = "2",
                    SumStar = star2Gr != null ? star2Gr.Count() : 0,
                    Percent = star2Gr != null ? (double)star2Gr.Count() / feedbacks.Count() * 100 : 0
                };
                stars.Add(star2);

                var star3 = new Stars
                {
                    star = "3",
                    SumStar = star3Gr != null ? star3Gr.Count() : 0,
                    Percent = star3Gr != null ? (double)star3Gr.Count() / feedbacks.Count() * 100 : 0
                };
                stars.Add(star3);

                var star4 = new Stars
                {
                    star = "4",
                    SumStar = star4Gr != null ? star4Gr.Count() : 0,
                    Percent = star4Gr != null ? (double)star4Gr.Count() / feedbacks.Count() * 100 : 0
                };
                stars.Add(star4);

                var star5 = new Stars
                {
                    star = "5",
                    SumStar = star5Gr != null ? star5Gr.Count() : 0,
                    Percent = star5Gr != null ? (double)star5Gr.Count() / feedbacks.Count() * 100 : 0
                };
                stars.Add(star5);

                Rating_Average = (double)feedbacks.Sum(x => x.Rating) / feedbacks.Count();
            }

            feedbacks = feedbacks.OrderByDescending(x => x.Date);

            var pageList = PaginatedList<Feedback>.Create(feedbacks.AsQueryable(), pageParams.PageNumber, pageParams.PageSize);

            var pageListVM = _mapper.Map<PaginatedList<Data>>(pageList);


            return new FeedbackViewModel
            {
                Rating_Average = Rating_Average,
                Feedbacks_Count = feedbacks.Count(),
                Stars = stars,
                Data = new PaginationSet<Data>(pageListVM.PageIndex, pageListVM.TotalPages, pageListVM.PageSize, pageListVM)
            };
        }

        public async Task<Data> GetUserFeedback(string userId, Guid productId)
        {
            var res = _feedbackRepository.GetSingleByCondition(x => x.User_ID == userId && x.Product_ID == productId, new[] { "User", "Images" });

            return _mapper.Map<Data>(res);
        }

        public async Task<int> LikeFeedback(Guid fbId, Guid productId)
        {
            var fb = _feedbackRepository.GetSingleByCondition(x => x.ID == fbId && x.Product_ID == productId);

            fb.Likes += 1;

            await _unitOfWork.CommitAsync();

            return fb.Likes;
        }
    }
}
