namespace BankWeb.ViewComponents
{
    public class ApiResponse
    {
        public Location location { get; set; }
        public Current current { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
    }

    public class Current
    {
        public double temp_c { get; set; }
    }

}
