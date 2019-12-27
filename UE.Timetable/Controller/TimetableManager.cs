using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UE.Timetable.Model;

namespace UE.Timetable.Controller
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

        public async Task<HttpResponseMessage> GetTimetable()
        {
            using (var client = new HttpClient())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(E_UCZELNIA_URL);
                sb.Append(HARMONOGRAM_ZAJECIA_URL);
                sb.Append(AddParameters());
                var url = sb.ToString();
                var ret = await client.GetAsync(url);
                ret.EnsureSuccessStatusCode();

                return ret;
            }
        }

        private string AddParameters()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("idGrupa", "44247");
            dict.Add("idNauczyciel", "0");
            dict.Add("widok", "STUDENT");
            dict.Add("page", "1");
            dict.Add("start", "0");
            dict.Add("limit", "500");

            if (dateFrom.HasValue
                && dateTo.HasValue)
            {
                dict.Add("dataOd", dateFrom.Value.ToString("yyyy-MM-dd"));
                dict.Add("dataDo", dateTo.Value.ToString("yyyy-MM-dd"));
            }

            var parameters = String.Join("&", dict.Select(d => String.Format("{0}={1}", d.Key, d.Value)));
            return "?" + parameters;
        }
    }
}
