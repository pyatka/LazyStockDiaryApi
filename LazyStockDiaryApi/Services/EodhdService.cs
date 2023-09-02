using System;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LazyStockDiaryApi.Services
{
	public class EodhdService
	{
		private HTTPbase httpClient;
		private string apiKey;
		private Uri baseUri;

		private Dictionary<string, string> prepareParameters(Dictionary<string, string>? keyValues = null)
		{
            if(keyValues == null)
			{
				keyValues = new Dictionary<string, string>();
			}
			var parameters = keyValues;
			parameters.Add("api_token", apiKey);
            parameters.Add("fmt", "json");
            return parameters;
		}

		public EodhdService(string apiKey)
		{
			httpClient = new HTTPbase();
            this.apiKey = apiKey;
			baseUri = new Uri("https://eodhistoricaldata.com/api");
		}

		public async Task<List<SearchSymbol>> Search(string query)
		{
			var uri = baseUri.Append("search", query.ToLower());
			var symbols = await httpClient.Get<List<SearchSymbol>>(uri, prepareParameters());
			return symbols;
		}

		public async void GetSymbolDetails(string code, string exchange)
		{
            var uri = baseUri.Append("eod", string.Format("{0}.{1}", code.ToUpper(), exchange.ToUpper()));
        }

		public async Task<List<HistoricalEod>> GetSymbolEodHistoryData(string code, string exchange)
		{
            var uri = baseUri.Append("eod", string.Format("{0}.{1}", code.ToUpper(), exchange.ToUpper()));
			List<HistoricalEod> historicalEods = await httpClient.Get<List<HistoricalEod>>(uri, prepareParameters());
			return historicalEods;
        }

        public async Task<List<Dividend>> GetSymbolDividendData(string code, string exchange)
        {
            var uri = baseUri.Append("div", string.Format("{0}.{1}", code.ToUpper(), exchange.ToUpper()));
            List<Dividend> dividends = await httpClient.Get<List<Dividend>>(uri, prepareParameters());
            return dividends;
        }
    }
}

