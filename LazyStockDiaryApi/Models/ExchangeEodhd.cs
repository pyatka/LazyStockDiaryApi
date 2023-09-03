using System;
namespace LazyStockDiaryApi.Models
{
	public class ExchangeEodhd
	{
		public string Name { get; set; }
		public string Code { get; set; }
		public string OperatingMIC { get; set; }
		public string Country { get; set; }
		public string Currency { get; set; }
		public string Timezone { get; set; }
		public TradingHours TradingHours { get; set; }

        public Exchange ToExchange()
        {
            Exchange exchange = new Exchange();
			exchange.Name = Name;
			exchange.Code = Code;
			exchange.OperatingMIC = OperatingMIC;
			exchange.Country = Country;
			exchange.Currency = Currency;
			exchange.Timezone = Timezone;
			exchange.OpenUTC = TradingHours.OpenUTC;
			exchange.CloseUTC = TradingHours.CloseUTC;
			exchange.WorkingDays = TradingHours.WorkingDays;
            return exchange;
        }
    }
}