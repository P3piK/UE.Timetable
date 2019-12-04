using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace UE.Timetable
{
    public class Lesson
    {
        [JsonProperty("przedmiot")]
        public string Subject { get; set; }
        [JsonProperty("dydaktyk")]
        public string Tutor { get; set; }
        [JsonProperty("lokalizacja")]
        public string Building { get; set; }
        [JsonProperty("nazwaSali")]
        public string Room { get; set; }
        [JsonProperty("dataZajec")]
        public DateTime Date { get; set; }
        [JsonProperty("godzinaOd")]
        public string TimeFrom { get; set; }
        [JsonProperty("godzinaDo")]
        public string TimeTo { get; set; }

        // DateTime DateFrom { get; set; }
        // DateTime DateTo { get; set; }
    }
}
