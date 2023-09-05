﻿using System;
using Newtonsoft.Json;

namespace LazyStockDiaryApi.Models
{
    /* Class for compensate differencies in EodHd API response data
     */
    public class HistoricalEodEodhd : HistoricalEod
    {
        [JsonProperty(PropertyName = "exchange_short_name")]
        public string Exchange
        {
            get
            {
                return base.Exchange;
            }
            set
            {
                base.Exchange = value;
            }
        }
        [JsonProperty(PropertyName = "prev_close")]
        public double? PreviousClose { get; set; }
        [JsonProperty(PropertyName = "change")]
        public double? ChangeAbsolute { get; set; }
        [JsonProperty(PropertyName = "change_p")]
        public double? ChangePercent { get; set; }

        public HistoricalEod ToHistoricalEod()
        {
            return (HistoricalEod)this;
        }
    }
}
