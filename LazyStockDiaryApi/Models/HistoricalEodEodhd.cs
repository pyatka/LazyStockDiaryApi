using System;
using Newtonsoft.Json;

namespace LazyStockDiaryApi.Models
{
    /* Class for compensate differencies in EodHd API response data
     */
    public class HistoricalEodEodhd : HistoricalEod
    {

        public HistoricalEodEodhd(HistoricalEod historicalEod)
        {
            Code = historicalEod.Code;
            Exchange = historicalEod.Exchange;
            Date = historicalEod.Date;
            Open = historicalEod.Open;
            Close = historicalEod.Close;
            High = historicalEod.High;
            Low = historicalEod.Low;
            Volume = historicalEod.Volume;
        }

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

