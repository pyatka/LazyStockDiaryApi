using System;
using System.Web;
using Newtonsoft.Json;

namespace LazyStockDiaryApi.Helpers;

public class HTTPbase
{
	private HttpClient client;

	private string prepareGetUrl(Uri uri, Dictionary<string, string> parameters)
	{
		string url = uri.AbsoluteUri;
            url = string.Format("{0}?{1}", url,
                string.Join("&",
				parameters.Select(kvp =>
					string.Format("{0}={1}", kvp.Key, kvp.Value))));
		return url;
        }

	public HTTPbase()
	{
		client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
        }

	public async Task<T> Get<T>(Uri uri, Dictionary<string, string>? parameters = null)
	{
		string url = prepareGetUrl(uri, parameters);
		var jsonText = await client.GetStringAsync(url);
		var o = JsonConvert.DeserializeObject<T>(jsonText);
            return o;
        }
}

