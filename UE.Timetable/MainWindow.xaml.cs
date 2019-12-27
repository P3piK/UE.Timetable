using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UE.Timetable.Controller;
using UE.Timetable.Model;

namespace UE.Timetable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetDataButton_Click(object sender, RoutedEventArgs e)
        {
            responseBox.Text = "Getting timetable...";

            var manager = new TimetableManager(DateFromPicker.SelectedDate, DateToPicker.SelectedDate);
            var responseTask = Task.Run(async () => await manager.GetTimetable());

            var contentTask = responseTask.ContinueWith(async r =>
            {
                var message = r.Result;
                return await message.Content.ReadAsStringAsync();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            responseTask.ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    responseBox.Text += String.Format("{0}{1}", "\n", t.Exception.Message);
                });
            }, TaskContinuationOptions.OnlyOnFaulted);

            var dataTask = contentTask.ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    responseBox.Text += "\nDeserializing...";
                });

                var data = JsonConvert.DeserializeObject<ResponseContent>(t.Result.Result);

                Dispatcher.Invoke(() =>
                {
                    responseBox.Text += String.Format("\nObtained {0} events!", data.Courses.Count);
                });

                return data;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);


            var calendarEventsTask = dataTask.ContinueWith(async d =>
            {
                Dispatcher.Invoke(() =>
                {
                    responseBox.Text += "\nAdding to Google Calendar...";
                });

                var calendarManager = new GoogleCalendarManager(d.Result.Courses, 
                                            Dispatcher.Invoke(() => DateFromPicker.SelectedDate), 
                                            Dispatcher.Invoke(() => DateToPicker.SelectedDate));
                await calendarManager.Run();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            calendarEventsTask.ContinueWith(t =>
            {
                t.Result.ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        responseBox.Text += "\nSuccessfully added new events!";
                    });
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                t.Result.ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        responseBox.Text += "\nError while adding or deleting Google Events!";
                        responseBox.Text += "\n" + t.Result.Exception.Message;
                    });
                }, TaskContinuationOptions.OnlyOnFaulted);
            });
        }
    }
}
