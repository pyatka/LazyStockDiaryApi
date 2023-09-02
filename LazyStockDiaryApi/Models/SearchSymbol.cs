using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    [Index(nameof(Code))]
    public class SearchSymbol
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Exchange { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string? ISIN { get; set; }
        public double? PreviousClose { get; set; }
        public DateTime? PreviousCloseDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdateDate { get; set; }

        public Symbol ToSymbol()
        {
            var s = new Symbol();
            s.Code = Code;
            s.Exchange = Exchange;
            s.Type = Type;
            s.Country = Country;
            s.Currency = Currency;
            s.ISIN = ISIN;
            s.Close = PreviousClose;
            s.PreviousCloseDate = PreviousCloseDate;
            return s;
        }
    }
}