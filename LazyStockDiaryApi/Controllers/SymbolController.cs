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
                                            IConfiguration configuration)
        {
            using (var context = new DataContext(configuration))
            {
                var searchSymbol = context.SearchSymbol.Where(s => s.Code == symbol.Code && s.Exchange == symbol.Exchange);
                SearchSymbol? searchSymbolData = searchSymbol.FirstOrDefault<SearchSymbol>();
                if (searchSymbolData != null)
                {
                    var symbolExists = context.Symbol.Any(s => s.Code == symbol.Code && s.Exchange == symbol.Exchange);

                    if (!symbolExists)
                    {
                        var eodhd = new EodhdService(settings.Value.EodhdApiKey);

                        var exchangeExists = context.Exchange.Any(e => e.Code == symbol.Exchange);
                        if (!exchangeExists)
                        {
                            ExchangeEodhd exchangeEodhd = await eodhd.GetExchangeDetails(symbol.Exchange);
                            context.Exchange.Add(exchangeEodhd.ToExchange());
                        }

                        List<HistoricalEod> historicalEods = await eodhd.GetSymbolEodHistoryData(symbol.Code, symbol.Exchange);
                        List<Dividend> dividends = await eodhd.GetSymbolDividendData(symbol.Code, symbol.Exchange);

                        Symbol newSymbol = searchSymbolData.ToSymbol();
                        newSymbol.UpdateWithEod(historicalEods.Last());

                        foreach (HistoricalEod eod in historicalEods)
                        {
                            eod.Code = symbol.Code;
                            eod.Exchange = symbol.Exchange;
                            context.HistoricalEod.Add(eod);
                        }

                        foreach (Dividend div in dividends)
                        {
                            div.Code = symbol.Code;
                            div.Exchange = symbol.Exchange;
                            context.Dividend.Add(div);
                        }

                        newSymbol.DividendLastUpdate = DateTime.Now;
                        newSymbol.EodLastUpdate = DateTime.Now;
                        context.Symbol.Add(newSymbol);

                        context.SaveChanges();

                        StatusCode(StatusCodes.Status201Created);
                        return newSymbol;
                    }
                    else
                    {
                        return context.Symbol.Where(s => s.Code == symbol.Code && s.Exchange == symbol.Exchange).FirstOrDefault();
                    }
                }
            }
            return null;
        }
    }
}

