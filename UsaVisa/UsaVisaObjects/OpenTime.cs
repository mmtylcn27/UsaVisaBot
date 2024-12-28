using System.Collections.Generic;
using Newtonsoft.Json;

namespace UsaVisa.UsaVisaObjects
{
    public class OpenTime
    {
        [JsonProperty("available_times")]
        public List<string> AvailableTimes { get; set; }
        [JsonProperty("business_times")]
        public List<string> BusinessTimes { get; set; }
    }
}
