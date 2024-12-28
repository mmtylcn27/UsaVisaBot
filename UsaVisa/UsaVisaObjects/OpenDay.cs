using System;
using Newtonsoft.Json;

namespace UsaVisa.UsaVisaObjects
{
    public class OpenDay
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("business_day")]
        public bool BusinessDay { get; set; }
    }

}
