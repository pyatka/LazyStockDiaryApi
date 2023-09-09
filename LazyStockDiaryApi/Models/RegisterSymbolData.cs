using System;
namespace LazyStockDiaryApi.Models
{
    public class RegisterSymbolData : SearchSymbol
    {
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public DateTime Date { get; set; }
    }
}

