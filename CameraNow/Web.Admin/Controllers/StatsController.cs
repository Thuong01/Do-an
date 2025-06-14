﻿using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Services;

namespace Web.Admin.Controllers
{
    public class StatsController : BaseController
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var start = (startDate ?? DateTime.Today.AddDays(-6)).ToUniversalTime();
            var end = (endDate ?? DateTime.Today).ToUniversalTime();

            var model = await _statsService.GetGeneralStatsAsync(start, end);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(DateTime startDate, DateTime endDate)
        {
            var model = await _statsService.GetGeneralStatsAsync(startDate, endDate);
            return PartialView("_StatsPartial", model);
        }
    }
}
