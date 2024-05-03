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

namespace ex04_wpf_bikeshop
{
    /// <summary>
    /// ContactPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContactPage : Page
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 코드에서는 속성에 값을 지정할때 사용방법
            Bike myBike = new Bike();
            myBike.Speed = 60;
            myBike.Color = Colors.Black;

            TextBox text1 = new TextBox();
            StpBike.DataContext = myBike;
            //MessageBox.Show(DgBike.Speed.ToString());
        }

        //private void SldValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    PrgValue.Value = (double)e.NewValue;
        //    LblValue.Content = Math.Round(PrgValue.Value, 1);
        //}
    }
}
