namespace 프로젝트.Models
{
    class Townbus
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
    }
}
