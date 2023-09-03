using System;
namespace LazyStockDiaryApi.Models
{
	public class TradingHours
	{
		public DateTime OpenUTC { get; set; }
		public DateTime CloseUTC { get; set; }
		public string WorkingDays { get; set; }
	}
}

