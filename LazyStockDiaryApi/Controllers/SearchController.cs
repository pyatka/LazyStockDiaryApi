using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LazyStockDiaryApi.Controllers;

[ApiController]
[Route("/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<SearchSymbol> Search(string query,
                                            IOptions<ApiSettings> settings,
                                            IConfiguration configuration)
    {
        query = query.ToLower();
        using (var context = new DataContext(configuration))
        {
            var result = context.SearchSymbol.Where(s => s.Code.ToLower() == query);
            if(result.Count<SearchSymbol>() > 0)
            {
                return result.FirstOrDefault();
            } else
            {
                var eodhd = new EodhdService(settings.Value.EodhdApiKey);
                List<SearchSymbol> symbols = await eodhd.Search(query);
                foreach (SearchSymbol s in symbols)
                {
                    context.SearchSymbol.Add(s);
                }
                context.SaveChanges();
                return await Search(query, settings, configuration);
            }
        }
    }
}

