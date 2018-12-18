using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallsAndSmses.Models
{
    public class MainList
    {
        public List<Sms> AllSmses { get; set; }
        public List<Calls> AllCalls { get; set; }
        public int CountOfSmses { get; set; }
        public int CountOfCalls { get; set; }
        public Dictionary<long?, int?> Top5OfSmses { get; set; }
        public Dictionary<long?, int?> Top5OfCalls { get; set; }
    }
}
