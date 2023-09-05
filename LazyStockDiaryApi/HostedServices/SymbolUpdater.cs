using System;
using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Services;
using Microsoft.Extensions.Options;

namespace LazyStockDiaryApi.HostedServices
{
    internal class EodhdUpdateManager
    {
        private readonly IOptions<ApiSettings> _settings;
        private List<Symbol> _symbolsToUpdate;

        public EodhdUpdateManager(IOptions<ApiSettings> settings)
        {
            _symbolsToUpdate = new List<Symbol>();
            _settings = settings;
        }

        public void PutSymbol(Symbol s)
        {
            _symbolsToUpdate.Add(s);
        }

        public async Task<List<HistoricalEodEodhd>> Update()
        {
            List<HistoricalEodEodhd> updateData = new List<HistoricalEodEodhd>();
            if(_symbolsToUpdate.Count > 0)
            {
                var eodhd = new EodhdService(_settings.Value.EodhdApiKey);

                var takeCount = Math.Ceiling((decimal)_symbolsToUpdate.Count / (decimal)30);
                for(var i = 0; i < takeCount; i++)
                {
                    var eods = await eodhd.GetBulkEod(_symbolsToUpdate.Skip(i * 30).Take(30).ToArray<Symbol>().Select(s => s.ToString()).ToArray<string>());
                    updateData = updateData.Concat(eods).ToList<HistoricalEodEodhd>();
                }
            }
            return updateData;
        }
    }

	public class SymbolUpdater : BackgroundService
    {
        private readonly ILogger<SymbolCacheCleaner> _logger;
        private readonly DataContext _context;
        private readonly IOptions<ApiSettings> _settings;
        private int _checkTimeout = 1000 * 60 * 10; //10 minutes

        private Dictionary<string, Exchange> _exchanges;

        public SymbolUpdater(ILogger<SymbolCacheCleaner> logger, IServiceScopeFactory factory, IOptions<ApiSettings> settings)
        {
            _logger = logger;
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _settings = settings;
            _exchanges = new Dictionary<string, Exchange>();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _exchanges.Clear();
                _exchanges = new Dictionary<string, Exchange>();
                foreach (Exchange e in _context.Exchange.ToArray())
                {
                    _exchanges.Add(e.Code, e);
                }

                EodhdUpdateManager eodhdUpdateManager = new EodhdUpdateManager(_settings);
                DateTime nowDateTime = DateTime.Now;

                foreach(Symbol symbol in _context.Symbol.ToArray())
                {
                    ExchangeStatus exchangeStatus = _exchanges[symbol.Exchange].GetExchangeStatus();
                    ExchangeStatus exchangeYesterdayStatus = _exchanges[symbol.Exchange].GetExchangeStatus(nowDateTime.AddDays(-1));
                    double daysFromLastUpdate = (nowDateTime - ((DateTime)symbol.EodLastUpdate)).TotalDays;

                    // For unupdated symbols
                    if (daysFromLastUpdate > 1 && exchangeYesterdayStatus != ExchangeStatus.ClosedToday)
                    {
                        eodhdUpdateManager.PutSymbol(symbol);
                    } else 
                    // For daily postmarket updates
                    if (exchangeStatus == ExchangeStatus.Post)
                    {
                        if(symbol.EodLastUpdate != null)
                        {
                            var relativeMarketCloseSecondsLastUpdate = _exchanges[symbol.Exchange].GetRelativeMarketCloseSeconds((DateTime)symbol.EodLastUpdate);
                            var relativeMarketCloseSeconds = _exchanges[symbol.Exchange].GetRelativeMarketCloseSeconds(nowDateTime);

                            // More than half hour post market
                            if (relativeMarketCloseSeconds > 60 * 30)
                            {
                                // Didn't update today postmarket
                                if(relativeMarketCloseSecondsLastUpdate < 0)
                                {
                                    eodhdUpdateManager.PutSymbol(symbol);
                                }
                            }
                        }
                    }
                }

                List<HistoricalEodEodhd> updateData = await eodhdUpdateManager.Update();

                // Save to database
                foreach (HistoricalEodEodhd data in updateData)
                {
                    var symbolToUpdate = _context.Symbol.Where(s => s.Code == data.Code && s.Exchange == data.Exchange).FirstOrDefault();
                    if (symbolToUpdate != null)
                    {
                        symbolToUpdate.UpdateEod(data);
                        symbolToUpdate.EodLastUpdate = nowDateTime;
                        _context.Symbol.Update(symbolToUpdate);

                        HistoricalEod newHistoricalEod = data.ToHistoricalEod();
                        var historicalDataExists = _context.HistoricalEod.Any(e => e.Date == newHistoricalEod.Date
                                                                                    && e.Code == newHistoricalEod.Code
                                                                                    && e.Exchange == newHistoricalEod.Exchange);
                        // Prevent double historical data
                        if (!historicalDataExists)
                        {
                            _context.HistoricalEod.Add(newHistoricalEod);
                        }

                        _context.SaveChanges();
                    }
                }

                await Task.Delay(_checkTimeout);
            }
        }
    }
}

