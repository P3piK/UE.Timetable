using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace UE.Timetable
{
    public class TimetableManager
    {
        private const string E_UCZELNIA_URL = "https://e-uczelnia.ue.katowice.pl/";
        private const string HARMONOGRAM_ZAJECIA_URL = "wsrest/rest/phz/harmonogram/zajecia";

        private DateTime? dateFrom;
        private DateTime? dateTo;

        public TimetableManager(DateTime? from, DateTime? to)
        {
            this.dateFrom = from;
            this.dateTo = to;
        }

        public IRestResponse<Dictionary<string, object>> GetTimetable()
        {
            var client = new RestClient(E_UCZELNIA_URL);
            var request = new RestRequest(HARMONOGRAM_ZAJECIA_URL, Method.GET);
            AddParameters(request);

            return client.Execute<Dictionary<string, object>>(request);
        }

        public IEnumerable<Course> Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Course>>(data);
        }

        private void AddParameters(RestRequest request)
        {
            request.AddParameter("idGrupa", 44247);
            request.AddParameter("idNauczyciel", 0);
            request.AddParameter("widok", "STUDENT");
            request.AddParameter("page", 1);
            request.AddParameter("start", 0);
            request.AddParameter("limit", 500);

            if (dateFrom.HasValue
                && dateTo.HasValue)
            {
                request.AddParameter("dataOd", dateFrom.Value.ToString("yyyy-MM-dd"));
                request.AddParameter("dataDo", dateTo.Value.ToString("yyyy-MM-dd"));
            }
        }
    }
}
