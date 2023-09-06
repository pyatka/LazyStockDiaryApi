using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    [Index(nameof(Code), nameof(Query))]
    public class SearchSymbolCache : SearchSymbol
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdateDate { get; set; }
        public string Query { get; set; }
    }
}

