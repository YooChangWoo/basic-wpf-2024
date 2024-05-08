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
using System.Windows.Shapes;
using ex07_EmployeeMngApp.Helpers;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ex07_EmployeeMngApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// 여기는 프로그래밍 코딩은 암함!!!!!
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();

            Common.DialogCoordinator = DialogCoordinator.Instance;  // 생성된 다이얼로그꾸미기 객체를 공통으로 이전
            this.DataContext = Common.DialogCoordinator;
        }
    }
}
