using System;
using System.Collections.Generic;

namespace CallsAndSmses.Models
{
    public partial class Sms
    {
        public long? Msisdn { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
