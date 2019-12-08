using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace UE.Timetable
{
    public enum Status
    {
        Error = 0,
        Success = 1
    }

    public class GoogleCalendarManager
    {
        private const string APPLICATION_NAME = "UE.Timetable";
        private static string[] scopes = { CalendarService.Scope.CalendarEvents };

        public GoogleCalendarManager(IEnumerable<Course> events)
        {
            Events = events;
        }

        public IEnumerable<Course> Events { get; }
        private UserCredential Credentials { get; set; }

        public Status Run()
        {
            var ret = Status.Error;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                Credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = APPLICATION_NAME,
                HttpClientInitializer = Credentials,
            });

            var request = service.Events.List("primary");
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var googleEvents = request.Execute();

            return ret;
        }
    }
}
