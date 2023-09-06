using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    public class SearchSymbol
	{
        public string Code { get; set; }
        public string Exchange { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string? ISIN { get; set; }
        public double? PreviousClose { get; set; }
        public DateTime? PreviousCloseDate { get; set; }

        public Symbol ToSymbol()
        {
            var s = new Symbol();
            s.Code = Code;
            s.Exchange = Exchange;
            s.Name = Name;
            s.Type = Type;
            s.Country = Country;
            s.Currency = Currency;
            s.ISIN = ISIN;
            s.Close = PreviousClose;
            s.PreviousCloseDate = PreviousCloseDate;
            return s;
        }

        public SearchSymbolCache ToSearchSymbolCache(string query)
        {
            SearchSymbolCache result = new SearchSymbolCache();
            result.Code = Code;
            result.Exchange = Exchange;
            result.Name = Name;
            result.Type = Type;
            result.Country = Country;
            result.Currency = Currency;
            result.ISIN = ISIN;
            result.PreviousClose = PreviousClose;
            result.PreviousCloseDate = PreviousCloseDate;
            result.Query = query;
            return result;
        }
    }
}