using System;
using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LazyStockDiaryApi.Controllers
{
    [ApiController]
    [Route("/dividend")]
    public class DividendContoller : ControllerBase
	{
        [HttpGet]
        public async Task<List<Dividend>> GetDividends(IServiceProvider serviceProvider,
                                                        [FromQuery]string code,
                                                        [FromQuery] string exchange,
                                                        [FromQuery] DateTime startDate,
                                                        [FromQuery] DateTime? endDate = null)
        {
            var symbolIntegrityService = serviceProvider.GetService<SymbolIntegrityService>();

            if (endDate == null)
            {
                endDate = DateTime.Now;
            }
            return await symbolIntegrityService.GetDividends(code, exchange, startDate, endDate.Value);
        }
    }
}

