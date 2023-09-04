using System;
using System.Runtime;
using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.Models;

namespace LazyStockDiaryApi.HostedServices
{
	public class SymbolCacheCleaner : BackgroundService
    {
        private readonly ILogger<SymbolCacheCleaner> _logger;
        private readonly DataContext _context;
        private int _checkTimeout = 1000 * 60 * 60 * 2; // 2 hours

        private DateTime expiredDate
        {
            get
            {
                return DateTime.Now.AddHours(-12);
            }
        }

        public SymbolCacheCleaner(ILogger<SymbolCacheCleaner> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _context.RemoveRange(_context.SearchSymbol.Where(s => s.UpdateDate < expiredDate));
                _context.SaveChanges();
                await Task.Delay(_checkTimeout);
            }
        }
    }
}

