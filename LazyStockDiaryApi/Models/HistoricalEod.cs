using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    [Index(nameof(Code), nameof(Exchange), nameof(Date))]
    public class HistoricalEod
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Exchange { get; set; }
        public DateTime Date { get; set; }

        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public int? Volume { get; set; }
    }
}

