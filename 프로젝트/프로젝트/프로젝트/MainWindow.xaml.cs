using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient;
using 프로젝트.Models;
using System.Data;



namespace 프로젝트
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool isFavorite = false;

        public MainWindow()
        {
            InitializeComponent();
            CboReqDate.ItemsSource = gulist;
        }

        public List<string> gulist = new List<string>()
        {
            "부산광역시 강서구",
            "부산광역시 금정구",
            "부산광역시 기장군",
            "부산광역시 남구",
            "부산광역시 동구",
            "부산광역시 동래구",
            "부산광역시 부산진구",
            "부산광역시 북구",
            "부산광역시 사상구",
            "부산광역시 사하구",
            "부산광역시 서구",
            "부산광역시 수영구",
            "연제구",
            "부산광역시 영도구",
            "부산광역시 중구",
            "부산광역시 해운대구"
        };

        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            //string openapi_Key = "s86nUoT8OvF9KjCQEnAYi6kAQ56CU5iiqDHjh384K4gzAVzXj4qFqiCulxZJuhz9yfgwb87yUG%2FCmL1hD5RO%2Bg%3D%3D";
            string openApiUrl = $@"https://apis.data.go.kr/6260000/VillageBusService/VillageBusStusInfo?serviceKey=s86nUoT8OvF9KjCQEnAYi6kAQ56CU5iiqDHjh384K4gzAVzXj4qFqiCulxZJuhz9yfgwb87yUG%2FCmL1hD5RO%2Bg%3D%3D&pageNo=1&numOfRows=143&resultType=json"; // 검색URL
            string result = string.Empty; // 결과값
            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUrl);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var status = Convert.ToInt32(jsonResult["VillageBusStusInfo"]["header"]["resultCode"]);

            try
            {
                if (status == 00) // 정상이면 데이터받아서 처리
                {
                    var data = jsonResult["VillageBusStusInfo"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var townbus = new List<Townbus>();
                    foreach (var sensor in json_array)
                    {
                        townbus.Add(new Townbus
                        {

                            Gugun = Convert.ToString(sensor["gugun"]),
                            Route_no = Convert.ToString(sensor["route_no"]), // openAPI
                            Starting_point = Convert.ToString(sensor["starting_point"]),
                            Transfer_point = Convert.ToString(sensor["transfer_point"]),
                            End_point = Convert.ToString(sensor["end_point"]),
                            First_bus_time = Convert.ToString(sensor["first_bus_time"]),
                            Last_bus_time = Convert.ToString(sensor["last_bus_time"]),
                            Bus_interval = Convert.ToString(sensor["bus_interval"])
                        });
                    }

                    GrdResults.ItemsSource = townbus;
                    isFavorite = false;
                    StsResult.Content = $"OpenAPI {townbus.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"JSON 처리오류 {ex.Message}");
            }
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResults.Items.Count == 0)
            {
                await this.ShowMessageAsync("오류", "조회쫌하고 저장하세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open(); // db는 똑같음 외우기
                                                                                       // 


                    var query = @"INSERT INTO townbus
                                            (Gugun,
                                            Route_no,
                                            Starting_point,
                                            Transfer_point,
                                            End_point,
                                            First_bus_time,
                                            Last_bus_time,
                                            Bus_interval)
                                            VALUES
                                            (@Gugun,
                                            @Route_no,
                                            @Starting_point,
                                            @Transfer_point,
                                            @End_point,
                                            @First_bus_time,
                                            @Last_bus_time,
                                            @Bus_interval)";
                    // workbench가서 `,{< 없애고 @추가, id는 뺴고(id는 ai체크 자동추가여서) 들고오기
                    var insRes = 0;
                    foreach (var temp in GrdResults.Items)
                    {
                        if (temp is Townbus)
                        {
                            var item = temp as Townbus;

                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Gugun", item.Gugun);
                            cmd.Parameters.AddWithValue("@Route_no", item.Route_no);
                            cmd.Parameters.AddWithValue("@Starting_point", item.Starting_point);
                            cmd.Parameters.AddWithValue("@Transfer_point", item.Transfer_point);
                            cmd.Parameters.AddWithValue("@End_point", item.End_point);
                            cmd.Parameters.AddWithValue("@First_bus_time", item.First_bus_time);
                            cmd.Parameters.AddWithValue("@Last_bus_time", item.Last_bus_time);
                            cmd.Parameters.AddWithValue("@Bus_interval", item.Bus_interval);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await this.ShowMessageAsync("저장", "DB저장 성공!!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"DB저장 오류! {ex.Message}");
            }
        }

        public void SearchBus(string busName)
        {
            if (CboReqDate.SelectedValue != null)
            {
                // MessageBox.Show(CboReqDate.SelectedValue.ToString());
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();
                    var query = $@"SELECT 
                                         Gugun,
                                         Route_no,
                                         Starting_point,
                                         Transfer_point,
                                         End_point,
                                         First_bus_time,
                                         Last_bus_time,
                                         Bus_interval
                                    FROM townbus
                                    WHERE Gugun LIKE '%{busName}%'";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "townbus");
                    List<Townbus> townbuss = new List<Townbus>();
                    foreach (DataRow row in ds.Tables["townbus"].Rows)
                    {
                        townbuss.Add(new Townbus
                        {

                            Gugun = Convert.ToString(row["gugun"]),
                            Route_no = Convert.ToString(row["route_no"]),
                            Starting_point = Convert.ToString(row["starting_point"]),
                            Transfer_point = Convert.ToString(row["transfer_point"]),
                            End_point = Convert.ToString(row["end_point"]),
                            First_bus_time = Convert.ToString(row["first_bus_time"]),
                            Last_bus_time = Convert.ToString(row["last_bus_time"]),
                            Bus_interval = Convert.ToString(row["bus_interval"])

                        });
                    }

                    GrdResults.ItemsSource = townbuss;
                    isFavorite = false;
                    StsResult.Content = $"DB {townbuss.Count}건 조회완료";
                }
            }
            else
            {
                GrdResults.ItemsSource = null;
                StsResult.Content = $"DB 조회클리어";
            }
        }

        private async void BtnSearchBus_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CboReqDate.SelectedValue.ToString()))
            {
                await this.ShowMessageAsync("검색", "검색할 노선명을 입력하세요.");
                return;
            }

            //if (TxtMovieName.Text.Length <= 2)
            //{
            //    await Commons.ShowMessageAsync("검색", "검색어를 2자이상 입력하세요.");
            //    return;
            //}

            try
            {
                SearchBus(CboReqDate.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }

        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResults.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("즐겨찾기", "추가할 노선을 선택하세요(복수선택가능).");
                return;
            }

            if (isFavorite == true) // 즐겨찾기 보기한 뒤 다시 즐겨찾기하려고할때 막음.
            {
                await this.ShowMessageAsync("즐겨찾기", "이미 즐겨찾기한 노선입니다.");
                return;
            }

            var addMovieItems = new List<FavoriteTownbus>();
            foreach (Townbus item in GrdResults.SelectedItems)
            {
                addMovieItems.Add(new FavoriteTownbus()
                {
                    Id = item.Id,
                    Gugun = item.Gugun,
                    Route_no = item.Route_no,
                    Starting_point = item.Starting_point,
                    Transfer_point = item.Transfer_point,
                    End_point = item.End_point,
                    First_bus_time = item.First_bus_time,
                    Last_bus_time = item.Last_bus_time,
                    Bus_interval = item.Bus_interval,
                });
            }

            Debug.WriteLine(addMovieItems.Count);
            try
            {
                var insRes = 0;

                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    foreach (FavoriteTownbus item in addMovieItems)
                    {
                        // 저장되기 전에 이미 저장된 데이터인지 확인 후 
                        SqlCommand chkCmd = new SqlCommand(FavoriteTownbus.CHECK_QUERY, conn);
                        chkCmd.Parameters.AddWithValue("@Id", item.Id);
                        var cnt = Convert.ToInt32(chkCmd.ExecuteScalar()); // COUNT(*) 등의 1row, 1coloumn값을 리턴할때

                        if (cnt == 1) continue;  // 이미 데이터가 있으면 패스

                        SqlCommand cmd = new SqlCommand(Models.FavoriteTownbus.INSERT_QUERY, conn);
                        //cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Gugun", item.Gugun);
                        cmd.Parameters.AddWithValue("@Route_no", item.Route_no);
                        cmd.Parameters.AddWithValue("@Starting_point", item.Starting_point);
                        cmd.Parameters.AddWithValue("@Transfer_point", item.Transfer_point);
                        cmd.Parameters.AddWithValue("@End_point", item.End_point);
                        cmd.Parameters.AddWithValue("@First_bus_time", item.First_bus_time);
                        cmd.Parameters.AddWithValue("@Last_bus_time", item.Last_bus_time);
                        cmd.Parameters.AddWithValue("@Bus_interval", item.Bus_interval);

                        insRes += cmd.ExecuteNonQuery(); // 데이터 하나마다 INSERT쿼리 실행
                    }
                }

                if (insRes == addMovieItems.Count)
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {insRes}건 저장성공!");
                }
                else
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {addMovieItems.Count}건중 {insRes}건 저장성공!");
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }
            BtnViewFavorite_Click(sender, e); // 저장후 저장된 즐겨찾기 바로보기
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            GrdResults.ItemsSource = null;  // 데이터그리드에 보낸 데이터를 모두 삭제

            List<FavoriteTownbus> favorites = new List<FavoriteTownbus>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(Models.FavoriteTownbus.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "Favorite");

                    
                    foreach (DataRow row in dSet.Tables["Favorite"].Rows)
                    {
                        var favorite = new FavoriteTownbus()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Gugun = Convert.ToString(row["gugun"]),
                            Route_no = Convert.ToString(row["route_no"]),
                            Starting_point = Convert.ToString(row["starting_point"]),
                            Transfer_point = Convert.ToString(row["transfer_point"]),
                            End_point = Convert.ToString(row["end_point"]),
                            First_bus_time = Convert.ToString(row["first_bus_time"]),
                            Last_bus_time = Convert.ToString(row["last_bus_time"]),
                            Bus_interval = Convert.ToString(row["bus_interval"]),

                        };
                        
                        favorites.Add(favorite);
                    }

                    GrdResults.ItemsSource = favorites;
                    isFavorite = true; // 즐겨찾기 DB에서 
                    StsResult.Content = $"즐겨찾기 {favorites.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 조회오류 {ex.Message}");
            }

        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await this.ShowMessageAsync("삭제", "즐겨찾기한 노선이 아닙니다.");
                return;
            }

            if (GrdResults.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("삭제", "삭제할 노선을 선택하세요.");
                return;
            }

            // 삭제시작!
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var delRes = 0;

                    foreach (FavoriteTownbus item in GrdResults.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(Models.FavoriteTownbus.DELETE_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResults.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {delRes}건 삭제");
                    }
                    else
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {GrdResults.SelectedItems.Count}건중 {delRes} 건 삭제");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 삭제 오류 {ex.Message}");
            }

            BtnViewFavorite_Click(sender, e);
        }
    }
   
}