using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UE.Timetable.Model;
using static Google.Apis.Calendar.v3.Data.Event;

namespace UE.Timetable.Controller
{
    public class GoogleCalendarManager
    {
        private const string APPLICATION_NAME = "UE.Timetable";
        private const string CALENDAR_ID = "primary";
        private static string[] scopes = { CalendarService.Scope.CalendarEvents };

        public GoogleCalendarManager(IEnumerable<Course> events, DateTime? from, DateTime? to)
        {
            Events = events;
            DateFrom = from;
            DateTo = to;
        }

        public IEnumerable<Course> Events { get; set; }
        public DateTime? DateFrom { get; }
        public DateTime? DateTo { get; }
        private UserCredential Credentials { get; set; }

        public async Task Run()
        {
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                Credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = APPLICATION_NAME,
                HttpClientInitializer = Credentials,
            });
            var googleEvents = await ListAllEvents(service);
            DeleteAllEvents(service, googleEvents);
            AddNewEvents(service);

            service.Dispose();
        }

        private void AddNewEvents(CalendarService service)
        {
            Parallel.ForEach(Events, async (ev) =>
            {
                var request = service.Events.Insert(new Event()
                {
                    ColorId = GetColor(ev),
                    Creator = new CreatorData() { DisplayName = APPLICATION_NAME },
                    Description = ev.Tutor,
                    End = new EventDateTime() { DateTime = ev.DateTo },
                    Location = ev.Location,
                    Reminders = new RemindersData() { UseDefault = false, Overrides = new List<EventReminder>() },
                    //Source = new SourceData() { Title = APPLICATION_NAME },
                    Start = new EventDateTime() { DateTime = ev.DateFrom },
                    Summary = ev.Subject
                }, CALENDAR_ID);

                var ret = await request.ExecuteAsync();
                // Sth with ret?
            });
        }

        private string GetColor(Course ev)
        {
            var ret = "1";
            if (ev.Subject.Contains("wykład", StringComparison.InvariantCultureIgnoreCase))
            {
                ret = "2";
            }
            else if (ev.Subject.Contains("ćwiczenia", StringComparison.InvariantCultureIgnoreCase))
            {
                ret = "5";
            }
            return ret;
        }

        private void DeleteAllEvents(CalendarService service, Events googleEvents)
        {
            Parallel.ForEach(googleEvents.Items, async (ev) =>
            {
                var request = service.Events.Delete(CALENDAR_ID, ev.Id);
                await request.ExecuteAsync();
            });
        }

        private async Task<Events> ListAllEvents(CalendarService service)
        {
            var request = service.Events.List(CALENDAR_ID);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 500;
            request.TimeMin = DateFrom;
            request.TimeMax = DateTo;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            return await request.ExecuteAsync();
        }
    }
}
