using System.Windows;

namespace 프로젝트
{
    /// <summary>
    /// WeatherWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ParkingWindow : Window
    {
        public ParkingWindow(string location)
        {
            InitializeComponent();
            ShowLocationOnMap(location);
        }

        private void ShowLocationOnMap(string location)
        {
            string mapsUrl = $"https://www.google.com/maps?q={Uri.EscapeDataString(location)}";
            BrsLoc.Address = mapsUrl;
        }
    }
}
