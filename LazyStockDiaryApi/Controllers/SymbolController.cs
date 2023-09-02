using System;
using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LazyStockDiaryApi.Controllers
{
    [ApiController]
    [Route("/symbol")]
    public class SymbolController : ControllerBase
	{
        [HttpPost]
        public async Task<bool> RegisterSymbol([FromForm]string code,
                                               [FromForm]string exchange,
                                            IOptions<ApiSettings> settings,
                                            IConfiguration configuration)
        {
            code = code.ToUpper();
            exchange = exchange.ToUpper();
            using (var context = new DataContext(configuration))
            {
                var searchSymbol = context.SearchSymbol.Where(s => s.Code == code && s.Exchange == exchange);
                SearchSymbol? searchSymbolData = searchSymbol.FirstOrDefault<SearchSymbol>();
                if(searchSymbolData != null)
                {
                    var eodhd = new EodhdService(settings.Value.EodhdApiKey);
                    List<HistoricalEod> historicalEods = await eodhd.GetSymbolEodHistoryData(code, exchange);
                    List<Dividend> dividends = await eodhd.GetSymbolDividendData(code, exchange);

                    Symbol newSymbol = searchSymbolData.ToSymbol();
                    newSymbol.UpdateWithEod(historicalEods.Last());

                    foreach(HistoricalEod eod in historicalEods)
                    {
                        eod.Code = code;
                        eod.Exchange = exchange;
                        context.HistoricalEod.Add(eod);
                    }

                    foreach (Dividend div in dividends)
                    {
                        div.Code = code;
                        div.Exchange = exchange;
                        context.Dividend.Add(div);
                    }

                    context.Symbol.Add(newSymbol);

                    context.SaveChanges();
                }
            }
            return true;
        }
    }
}

