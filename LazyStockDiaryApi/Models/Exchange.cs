using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    [Index(nameof(Code))]
    public class Exchange
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string OperatingMIC { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Timezone { get; set; }

        public DateTime OpenUTC { get; set; }
        public DateTime CloseUTC { get; set; }
        public string? WorkingDays { get; set; }
    }
}

