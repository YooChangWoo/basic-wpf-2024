using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 프로젝트.Models
{
    public class FavoriteTownbus
    {
        public int Id { get; set; }
        public string Gugun { get; set; }
        public string Route_no { get; set; }
        public string Starting_point { get; set; }
        public string Transfer_point { get; set; }
        public string End_point { get; set; }
        public string First_bus_time { get; set; }
        public string Last_bus_time { get; set; }
        public string Bus_interval { get; set; }

        public static readonly string CHECK_QUERY = @"SELECT COUNT(*)
                                                        FROM favoritetownbus
                                                       WHERE Id = @Id";

        public static readonly string SELECT_QUERY = @"SELECT  [Id]
                                                              ,[Gugun]
                                                              ,[Route_no]
                                                              ,[Starting_point]
                                                              ,[Transfer_point]
                                                              ,[End_point]
                                                              ,[First_bus_time]
                                                              ,[Last_bus_time]
                                                              ,[Bus_interval]
                                                          FROM [dbo].[favoritetownbus]";

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[favoritetownbus]
                                                                       ([Gugun]
                                                                       ,[Route_no]
                                                                       ,[Starting_point]
                                                                       ,[Transfer_point]
                                                                       ,[End_point]
                                                                       ,[First_bus_time]
                                                                       ,[Last_bus_time]
                                                                       ,[Bus_interval])
                                                                 VALUES
                                                                       (@Gugun
                                                                       ,@Route_no
                                                                       ,@Starting_point
                                                                       ,@Transfer_point
                                                                       ,@End_point
                                                                       ,@First_bus_time
                                                                       ,@Last_bus_time
                                                                       ,@Bus_interval)";

        public static readonly string DELETE_QUERY = @"DELETE FROM [dbo].[favoritetownbus] WHERE Id = @id";
    }
}
