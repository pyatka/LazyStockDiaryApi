using System;
using System.Runtime.InteropServices;
using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LazyStockDiaryApi.Services
{
	public class SymbolIntegrityService
	{
		private EodhdService _eodhdService;
        private IConfiguration _configuration;

        public SymbolIntegrityService(IOptions<ApiSettings> settings, IConfiguration configuration)
		{
			_eodhdService = new EodhdService(settings.Value.EodhdApiKey);
            _configuration = configuration;
        }

        public async Task<List<Dividend>> GetDividends(string code,
                                                        string exchange,
                                                        DateTime startDate,
                                                        DateTime endDate)
        {
            using (var context = new DataContext(_configuration))
            {
                var symbolExists = context.Dividend.Any(d => d.Code == code && d.Exchange == exchange);
                if (symbolExists)
                {
                    var dividends = await context.Dividend.Where(d => d.Code == code
                                                                    && d.Exchange == exchange
                                                                    && d.RecordDate > startDate
                                                                    && d.RecordDate <= endDate)
                                                          .ToListAsync();
                    return dividends;
                } else
                {
                    return new List<Dividend>();
                }
            }
        }

        public async Task<HistoricalEodEodhd> GetEodhdChanges(Symbol symbol)
        {
            using (var context = new DataContext(_configuration))
            {
                var historicalData = await context.HistoricalEod.Where(eod => eod.Code == symbol.Code
                                                          && eod.Exchange == symbol.Exchange)
                                           .OrderByDescending(eod => eod.Date)
                                           .Take(2).ToArrayAsync();
                HistoricalEodEodhd result = new HistoricalEodEodhd(historicalData[0]);
                result.PreviousClose = historicalData[1].Close;
                result.ChangeAbsolute = Math.Round(result.Close.Value - result.PreviousClose.Value, 2);
                result.ChangePercent = Math.Round((result.ChangeAbsolute.Value / result.PreviousClose.Value) * 100, 2);
                return result;
            }
        }

        public async Task<HistoricalEod> GetLastEod(Symbol symbol)
        {
            using (var context = new DataContext(_configuration))
            {
                return await context.HistoricalEod.Where(eod => eod.Code == symbol.Code
                                                           && eod.Exchange == symbol.Exchange)
                                            .OrderByDescending(eod => eod.Date)
                                            .FirstOrDefaultAsync();
            }
        }

		public async Task<bool> RegisterExchange(string code)
		{
            using (var context = new DataContext(_configuration))
            {
                var exchangeExists = context.Exchange.Any(e => e.Code == code);
                if (!exchangeExists)
                {
                    ExchangeEodhd exchangeEodhd = await _eodhdService.GetExchangeDetails(code);
                    context.Exchange.Add(exchangeEodhd.ToExchange());
                    context.SaveChanges();
                }
            }
            return true;
		}

        public async Task<DateTime> UpdateDividend(Symbol symbol)
        {
            using (var context = new DataContext(_configuration))
            {
                List<Dividend> dividends = await _eodhdService.GetSymbolDividendData(symbol.Code, symbol.Exchange);
                Dividend[] registredDividends = await context.Dividend.Where(d => d.Code == symbol.Code
                                                                                && d.Exchange == symbol.Exchange)
                                                                      .ToArrayAsync();
                if(registredDividends.Count() == 0)
                {
                    // First data receive and company pays dividends
                    if(dividends.Count > 0)
                    {
                        foreach (Dividend div in dividends)
                        {
                            div.Code = symbol.Code;
                            div.Exchange = symbol.Exchange;
                            context.Dividend.Add(div);
                        }
                    }
                } else
                {
                    foreach (Dividend div in dividends)
                    {
                        var divExists = Array.FindIndex(registredDividends, a => a.Date == div.Date) != -1;
                        if (!divExists)
                        {
                            div.Code = symbol.Code;
                            div.Exchange = symbol.Exchange;
                            context.Dividend.Add(div);
                        }
                    }
                }
                context.SaveChanges();
            }
            return DateTime.Now;
        }

        public async Task<DateTime> UpdateHistoricalEod(Symbol symbol)
        {
            using (var context = new DataContext(_configuration))
            {
                List<HistoricalEod> historicalEods = await _eodhdService.GetSymbolEodHistoryData(symbol.Code, symbol.Exchange);
                HistoricalEod[] registredHistoricalEods = await context.HistoricalEod.Where(e => e.Code == symbol.Code
                                                                                                        && e.Exchange == symbol.Exchange)
                                                                                         .ToArrayAsync();
                // First data receive for symbol
                if(registredHistoricalEods.Count() == 0)
                {
                    foreach (HistoricalEod eod in historicalEods)
                    {
                        eod.Code = symbol.Code;
                        eod.Exchange = symbol.Exchange;
                        context.HistoricalEod.Add(eod);
                    }
                } else
                {
                    foreach(HistoricalEod eod in historicalEods)
                    {
                        var eodExists = Array.FindIndex(registredHistoricalEods, a => a.Date == eod.Date) != -1;
                        if (!eodExists)
                        {
                            eod.Code = symbol.Code;
                            eod.Exchange = symbol.Exchange;
                            context.HistoricalEod.Add(eod);
                        }
                    }
                }
                context.SaveChanges();
            }
            return DateTime.Now;
        }
	}
}

