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
        private const string E_UCZELNIA_URL = "https://e-uczelnia.ue.katowice.pl/";
        private const string HARMONOGRAM_ZAJECIA_URL = "wsrest/rest/phz/harmonogram/zajecia";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetDataButton_Click(object sender, RoutedEventArgs e)
        {
            statusBox.Text = "Pending...";

            var client = new RestClient(E_UCZELNIA_URL);
            var request = new RestRequest(HARMONOGRAM_ZAJECIA_URL, Method.GET);
            AddParameters(request);

            var response = client.Execute<dynamic>(request);
            statusBox.Text = response.StatusCode.ToString();
            countBox.Text = response.Data["totalResultCount"].ToString();
            responseBox.Text = response.Data["result"].ToString();
        }

        private void AddParameters(RestRequest request)
        {
            request.AddParameter("idGrupa", 44247);
            request.AddParameter("idNauczyciel", 0);
            request.AddParameter("widok", "STUDENT");
            request.AddParameter("page", 1);
            request.AddParameter("start", 0);
            request.AddParameter("limit", 25);

            if (DateFromPicker.SelectedDate.HasValue
                && DateToPicker.SelectedDate.HasValue)
            {
                request.AddParameter("dataOd", DateFromPicker.SelectedDate.Value.ToString("yyyy-MM-dd"));
                request.AddParameter("dataDo", DateToPicker.SelectedDate.Value.ToString("yyyy-MM-dd"));
            }
        }
    }
}
