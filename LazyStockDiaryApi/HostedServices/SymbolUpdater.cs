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
                    var eods = await eodhd.GetBulkEod(_symbolsToUpdate.Take(30).ToArray<Symbol>().Select(s => s.ToString()).ToArray<string>());
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

        public SymbolUpdater(ILogger<SymbolCacheCleaner> logger, IServiceScopeFactory factory, IOptions<ApiSettings> settings)
        {
            _logger = logger;
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _settings = settings;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                EodhdUpdateManager eodhdUpdateManager = new EodhdUpdateManager(_settings);
                Exchange[] exchanges = _context.Exchange.ToArray();
                DateTime nowDateTime = DateTime.Now;
                foreach (Exchange e in exchanges)
                {
                    // Check if it's postmarket state
                    if(true || e.GetExchangeStatus() == ExchangeStatus.Post)
                    {
                        var secondsPostMarket = e.GetPostStateSeconds(nowDateTime.TimeOfDay);

                        // Wait for postmarket data to receive in EodHd (30 min)
                        if(true || secondsPostMarket > 60 * 30)
                        {
                            var exchangeSymbols = _context.Symbol.Where(s => s.Exchange == e.Code).ToArray();
                            foreach(Symbol s in exchangeSymbols)
                            {
                                if(s.EodLastUpdate != null)
                                {
                                    // If last update was more than day ago
                                    if(true || (nowDateTime - ((DateTime)s.EodLastUpdate)).TotalDays > 0)
                                    {
                                        eodhdUpdateManager.PutSymbol(s);
                                    }
                                } else
                                {
                                    eodhdUpdateManager.PutSymbol(s);
                                }
                            }
                        }
                    }
                }

                List<HistoricalEodEodhd> updateData = await eodhdUpdateManager.Update();

                await Task.Delay(_checkTimeout);
            }
        }
    }
}

