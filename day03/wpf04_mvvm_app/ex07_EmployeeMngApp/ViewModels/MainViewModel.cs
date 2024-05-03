using Caliburn.Micro;
using ex07_EmployeeMngApp.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ex07_EmployeeMngApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        // 멤버변수
        private int id;
        private string empName;
        private decimal salary;
        private string deptName;
        private string addr;

        // List는 정적인 컬렉션, BindableCollection은 동적인 컬렉션.
        // MVVM처럼 LIST를 사용못함.
        private BindableCollection<Employees> listEmployees;

        private Employees selectedEmployee;

        public Employees SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
                // 데이터를 TextBox들에 전달.

                if (selectedEmployee != null)
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Addr = value.Addr;

                    NotifyOfPropertyChange(() => SelectedEmployee); // View에 데이터가 표시될려면 필수!!!
                    NotifyOfPropertyChange(() => Id);
                    NotifyOfPropertyChange(() => EmpName);
                    NotifyOfPropertyChange(() => Salary);
                    NotifyOfPropertyChange(() => DeptName);
                    NotifyOfPropertyChange(() => Addr);
                }
            }
        }
        // 속성

      

        public int Id
        {
            get => id; set
            {
                id = value;
                NotifyOfPropertyChange(() => id);
                NotifyOfPropertyChange(() => CanDelEmployee);  // 삭제여부 속성도 변경했다는 공지를 해야함
            }
        }
        public string EmpName
        {
            get => empName; 
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => EmpName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }
        public decimal Salary
        {
            get => salary; 
            set
            {
                salary = value;
                NotifyOfPropertyChange(() => Salary);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }
        public string DeptName
        {
            get => deptName;
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => deptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);

            }
        }
        // 저장버튼 활성화 여부 속성
        public bool CanSaveEmployee
        {
            get
            {
                if (string.IsNullOrEmpty(EmpName) || Salary == 0 || string.IsNullOrEmpty(DeptName))
                    return false;
                else
                    return true;

            }
        }
        public string Addr
        {
            get => addr; 
            set
            {
                addr = value;
                NotifyOfPropertyChange(() => Addr);
            }
        }

        // DataGrid에 뿌릴 Employees 테이블 데이터
        public BindableCollection<Employees> ListEmployees
        {
            get => listEmployees;
            set
            {
                listEmployees = value;
                // 값이 변경된 것을 시스템 알려줌
                NotifyOfPropertyChange(() => ListEmployees); // 필수!!!
            }
        }

        public MainViewModel()
        {
            DisplayName = "직원관리 시스템";

            // 조회 실행
            GetEmployees();
        }

        /// <summary>
        /// Caliburn.Micro가 Xaml의 버튼 x:Name과 동일한 이름의 메서드로 매핑
        /// </summary>
        public void SaveEmployee()
        {
            // MessageBox.Show("저장버튼 동작!");
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                if (Id == 0)
                    cmd.CommandText = Models.Employees.INSERT_QUERY;
                else
                    cmd.CommandText = Models.Employees.UPDATE_QUERY;

                SqlParameter prmEmpName = new SqlParameter("@EmpName", EmpName);
                cmd.Parameters.Add(prmEmpName);

                SqlParameter prmSlalry = new SqlParameter("@Salary", Salary);
                cmd.Parameters.Add(prmSlalry);

                SqlParameter prmDeptName = new SqlParameter("@DeptName", DeptName);
                cmd.Parameters.Add(prmDeptName);

                SqlParameter prmAddr = new SqlParameter("@Addr", Addr ?? (object)DBNull.Value);  // 주소가 빈값일 때 컬럼에 null값을 입력
                cmd.Parameters.Add(prmAddr);

                if (id != 0)  // 업데이트면 id 파라미터가 필요
                {
                    SqlParameter prmId = new SqlParameter("@id", Id);
                    cmd.Parameters.Add(prmId);
                }

                var result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("저장성공!");
                }
                else
                {
                    MessageBox.Show("저장실패!");
                }
                GetEmployees();  // 그리드 재조회
                NewEmployee();  // 모든 입력컨트롤 초기화
            }
        }

       
        public void GetEmployees()
        {
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open ();
                SqlCommand cmd = new SqlCommand(Models.Employees.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ListEmployees = new BindableCollection<Employees>();
               
                while (reader.Read())
                {
                    ListEmployees.Add(new Employees()
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Addr = reader["Addr"].ToString()
                    });
                }
            }
        }
        // 삭제버튼을 누를 수 있는지 확인여부
        public bool CanDelEmployee
        {
            get { return Id != 0; }  // TextBox Id 속성의 값이 0이면 fales
        }

        public void DelEmployee()
        {
            if (Id == 0)
            {
                MessageBox.Show("삭제불가!");
                return;
            }
            if (MessageBox.Show("삭제하시겠습니까?", "삭제여부", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }


            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open ();
                SqlCommand cmd = new SqlCommand(Models.Employees.DELETE_QUERY, conn);
                SqlParameter prmId = new SqlParameter("@Id", Id);
                cmd.Parameters.Add(prmId);

                var res = cmd.ExecuteNonQuery();
                if (res >= 0)
                {
                    MessageBox.Show("삭제성공");
                }
                else
                {
                    MessageBox.Show("삭제실패");
                }

                GetEmployees();
            }
        }
        
        public void NewEmployee()
        {
            Id = 0;
            Salary = 0;
            EmpName = DeptName = Addr = "";
        }
    }
}
