using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LazyStockDiaryApi.Models
{
    [Index(nameof(Code), nameof(Exchange), nameof(RecordDate))]
    public class Dividend
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Exchange { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? DeclarationDate {get; set; }
        public DateTime? RecordDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Period { get; set; }
        public double Value { get; set; }
        public double UnadjustedValue { get; set; }
        public string Currency { get; set; }
    }
}