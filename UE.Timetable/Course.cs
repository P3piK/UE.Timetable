using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace UE.Timetable
{
    public class Course
    {
        [JsonProperty("przedmiot")]
        public string Subject { get; set; }

        [JsonProperty("dydaktyk")]
        public string Tutor { get; set; }

        [JsonProperty("lokalizacja")]
        public string Building { get; set; }

        [JsonProperty("nazwaSali")]
        public string Room { get; set; }

        [JsonProperty("_SYS_NW_data_od")]
        public DateTime Date { get; set; }

        [JsonProperty("godzinaOd")]
        public string TimeFrom { get; set; }

        [JsonProperty("godzinaDo")]
        public string TimeTo { get; set; }


        [JsonIgnore]
        public DateTime DateFrom => Date.Add(TimeSpan.Parse(TimeFrom));

        [JsonIgnore]
        public DateTime DateTo => Date.Add(TimeSpan.Parse(TimeTo));

        [JsonIgnore]
        public string Location => String.Format("{0} {1}", Building, Room);
    }
}
