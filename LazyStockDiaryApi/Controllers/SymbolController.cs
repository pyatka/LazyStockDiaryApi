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
        [HttpGet]
        public Symbol? GetSymbol([FromQuery] string code,
                                    [FromQuery] string exchange,
                                    IOptions<ApiSettings> settings,
                                    IConfiguration configuration)
        {
            Symbol? symbol = null;
            code = code.ToUpper();
            exchange = exchange.ToUpper();

            using (var context = new DataContext(configuration))
            {
                symbol = context.Symbol.FirstOrDefault(s => s.Code == code && s.Exchange == exchange);
            }

            if(symbol == null)
            {
                StatusCode(StatusCodes.Status204NoContent);
            }
            return symbol;
        }

        [HttpPost]
        public async Task<Symbol?> RegisterSymbol([FromBody]RegisterSymbolData symbol,
                                            IOptions<ApiSettings> settings,
                                            IServiceProvider serviceProvider,
                                            IConfiguration configuration)
        {
            using (var context = new DataContext(configuration))
            {
                var searchSymbol = context.SearchSymbol.Where(s => s.Code == symbol.Code && s.Exchange == symbol.Exchange);
                SearchSymbol? searchSymbolData = searchSymbol.FirstOrDefault<SearchSymbol>();

                /* Check if symbol was searched previously, 
                 * elswhere it's unauthorized traffic and we can't be sure in 
                 * Eodhd request.
                 */
                if (searchSymbolData != null)
                {
                    var symbolExists = context.Symbol.Any(s => s.Code == symbol.Code && s.Exchange == symbol.Exchange);

                    if (symbolExists)
                    {
                        return context.Symbol.Where(s => s.Code == symbol.Code
                                                    && s.Exchange == symbol.Exchange).FirstOrDefault();
                    } else {
                        var symbolIntegrityService = serviceProvider.GetService<SymbolIntegrityService>();
                        await symbolIntegrityService.RegisterExchange(symbol.Exchange);

                        Symbol newSymbol = searchSymbolData.ToSymbol();
                        newSymbol.EodLastUpdate = await symbolIntegrityService.UpdateHistoricalEod(newSymbol);
                        newSymbol.DividendLastUpdate = await symbolIntegrityService.UpdateDividend(newSymbol);

                        var lastEod = await symbolIntegrityService.GetLastEod(newSymbol);
                        newSymbol.UpdateWithEod(lastEod);

                        StatusCode(StatusCodes.Status201Created);

                        context.Symbol.Add(newSymbol);
                        context.SaveChanges();

                        return newSymbol;      
                    }
                }
            }
            return null;
        }
    }
}

