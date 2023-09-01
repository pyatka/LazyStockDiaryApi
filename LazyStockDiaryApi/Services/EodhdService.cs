using System;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Helpers;

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
	}
}

