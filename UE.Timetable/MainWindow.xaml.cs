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
            statusBox.Text = "Pending...";

            var manager = new TimetableManager(DateFromPicker.SelectedDate, DateToPicker.SelectedDate);
            var response = manager.GetTimetable();
            var data = manager.Deserialize(response.Data["result"].ToString());

            var calendarManager = new GoogleCalendarManager(data);
            calendarManager.Run();

            statusBox.Text = response.StatusCode.ToString();
            countBox.Text = response.Data["totalResultCount"].ToString();
            responseBox.Text = data.ToString();
        }

    }
}
