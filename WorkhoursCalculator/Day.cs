using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkhoursCalculator
{
    public class Day
    {
        public DateTime Date { get; set; }
        public string Work { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime Ende => End != null ? End.Value : DateTime.Now;
        public TimeSpan Pause { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan WorkHours => End != null ? (End.Value - Start) - Pause : (DateTime.Now - Start) - Pause;

        public string DateString => Date.ToShortDateString();
        public string StartString => Start.ToShortTimeString();
        public string EndeString => Ende.ToShortTimeString();

    }
}
