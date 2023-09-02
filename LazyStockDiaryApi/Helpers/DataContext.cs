using System;
using LazyStockDiaryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Helpers
{
	public class DataContext : DbContext
	{
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
		{
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration.GetConnectionString("MySqlDbConnectionString");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public DbSet<SearchSymbol> SearchSymbol { get; set; }
        public DbSet<HistoricalEod> HistoricalEod { get; set; }
        public DbSet<Symbol> Symbol { get; set; }
        public DbSet<Dividend> Dividend { get; set; }
    }
}

