using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private async void GetDataButton_Click(object sender, RoutedEventArgs e)
        {
            statusBox.Text = "Getting timetable...";

            var manager = new TimetableManager(DateFromPicker.SelectedDate, DateToPicker.SelectedDate);
            var response = await manager.GetTimetable();
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ResponseContent>(content);

            responseBox.Text = content;
            statusBox.Text = "Managing events...";

            var calendarManager = new GoogleCalendarManager(data.Courses, DateFromPicker.SelectedDate, DateToPicker.SelectedDate);
            await calendarManager.Run();

            statusBox.Text = "Success";
        }

    }
}
