using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    [Index(nameof(Code), nameof(Exchange))]
    public class Symbol
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Exchange { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string? ISIN { get; set; }

        public DateTime? PreviousCloseDate { get; set; }

        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public int? Volume { get; set; }

        //public double PreviousClose { get; set; }

        public DateTime? DividendLastUpdate { get; set; }
        public DateTime? EodLastUpdate { get; set; }

        public void UpdateWithEod(HistoricalEod data)
        {
            PreviousCloseDate = data.Date;
            Open = data.Open;
            Close = data.Close;
            High = data.High;
            Low = data.Low;
            Volume = data.Volume;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Code, Exchange);
        }
    }
}

