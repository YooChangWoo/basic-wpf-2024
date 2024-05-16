using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using 프로젝트.Model;


namespace 프로젝트
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://apis.data.go.kr/6260000/BusanPblcPrkngInfoService/getPblcPrkngInfo?serviceKey=76VwlwEHa2dVbgvluevXK%2BBu5E%2BiSOR4qZ77aiy7AneHZ7YqFaZL%2B68AkzI8%2Fphd0EGRT0SlHDOZbF8mMQXF1g%3D%3D&pageNo=1&numOfRows=10&resultType=json"; // OpenAPI URI
            string result = string.Empty; // 결과 문자열 초기화

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                //await this.ShowMessageAsync("결과", result);
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var resultCode = Convert.ToString(jsonResult["getPblcPrkngInfo"]["header"]["resultCode"]);

            if (resultCode == "00")
            {
                var data = jsonResult["getPblcPrkngInfo"]["body"]["items"]["item"];
                var jsonArray = data as JArray; // json자체에서 []안에 들어간 배열데이터만 JArray 변환가능

                var dustSensors = new List<Parking>();
                foreach (var item in jsonArray)
                {
                    dustSensors.Add(new Parking()
                    {
                        Id = 0,
                        guNm = Convert.ToString(item["guNm"]),
                        pkNam = Convert.ToString(item["pkNam"]),
                        mgntNum = Convert.ToInt32(item["mgntNum"]),
                        doroAddr = Convert.ToString(item["doroAddr"]),
                        jibunAddr = Convert.ToString(item["jibunAddr"]),
                        tponNum = Convert.ToString(item["tponNum"]),
                        pkFm = Convert.ToString(item["pkFm"]),
                        pkCnt = Convert.ToInt32(item["pkCnt"]),
                        svcSrtTe = Convert.ToString(item["svcSrtTe"]),
                        svcEndTe = Convert.ToString(item["svcEndTe"]),
                        satSrtTe = Convert.ToString(item["satSrtTe"]),
                        satEndTe = Convert.ToString(item["satEndTe"]),
                        hldSrtTe = Convert.ToString(item["hldSrtTe"]),
                        hldEndTe = Convert.ToString(item["hldEndTe"]),
                        ldRtg = Convert.ToInt32(item["ldRtg"]),
                        tenMin = Convert.ToInt32(item["tenMin"]),
                        ftDay = Convert.ToInt32(item["ftDay"]),
                        ftMon = Convert.ToInt32(item["ftMon"]),
                        xCdnt = Convert.ToString(item["xCdnt"]),
                        yCdnt = Convert.ToString(item["yCdnt"]),
                        fnlDt = Convert.ToDateTime(item["fnlDt"]),
                        pkGubun = Convert.ToString(item["pkGubun"]),
                        bujeGubun = Convert.ToString(item["bujeGubun"]),
                        oprDay = Convert.ToString(item["oprDay"]),
                        feeInfo = Convert.ToString(item["feeInfo"]),
                        pkBascTime = Convert.ToInt32(item["pkBascTime"]),
                        pkAddTime = Convert.ToInt32(item["pkAddTime"]),
                        feeAdd = Convert.ToInt32(item["feeAdd"]),
                        ftDayApplytime = Convert.ToString(item["ftDayApplytime"]),
                        payMtd = Convert.ToString(item["payMtd"]),
                        spclNote = Convert.ToString(item["spclNote"]),
                        currava = Convert.ToString(item["currava"]),
                        oprt_fm = Convert.ToString(item["oprt_fm"])
                    });
                }

                this.DataContext = dustSensors;
                StsResult.Content = $"OpenAPI {dustSensors.Count}건 조회완료!";
            }
        }
    }
}