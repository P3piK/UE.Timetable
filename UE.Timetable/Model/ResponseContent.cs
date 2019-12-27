using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UE.Timetable.Model
{
    public class ResponseContent
    {
        [JsonProperty("result")]
        public IList<Course> Courses { get; set; }
    }
}
