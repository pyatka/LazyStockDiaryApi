using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LazyStockDiaryApi.Models
{
    public enum ExchangeStatus
    {
        Pre = 0,
        Open = 1,
        Post = 2,
        ClosedToday = 3,
    }

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

        public double GetRelativeMarketCloseSeconds(DateTime date)
        {
            var now = DateTime.Now.ToUniversalTime();
            var closeWithDate = new DateTime(now.Year, now.Month, now.Day, CloseUTC.Hour, CloseUTC.Minute, CloseUTC.Second);
            return (date - closeWithDate).TotalSeconds;
        }

        public ExchangeStatus GetExchangeStatus(DateTime? date = null)
        {
            var now = DateTime.Now.ToUniversalTime();

            if(date != null)
            {
                now = ((DateTime)date).ToUniversalTime();
            }

            if(WorkingDays != null)
            {
                string[] workingDays = WorkingDays.Split(",");
                int[] workingDaysIndexes = new int[workingDays.Length];
                for(int i = 0; i < workingDays.Length; i++)
                {
                    workingDaysIndexes[i] = Array.IndexOf(eodhdDays, workingDays[i]);
                }

                if (!workingDaysIndexes.Contains((int)now.DayOfWeek))
                {
                    return ExchangeStatus.ClosedToday;
                }
            }

            DateTime closeWithDelta = CloseUTC.AddMinutes(30);
            if (now.TimeOfDay <= OpenUTC.TimeOfDay)
            {
                return ExchangeStatus.Pre;
            }
            else if (now.TimeOfDay > OpenUTC.TimeOfDay && now.TimeOfDay <= CloseUTC.TimeOfDay)
            {
                return ExchangeStatus.Open;
            }
            else
            {
                return ExchangeStatus.Post;
            }
        }

        private string[] eodhdDays = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
    }
}

