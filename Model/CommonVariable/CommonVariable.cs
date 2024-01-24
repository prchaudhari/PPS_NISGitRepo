using System.Configuration;

namespace nIS
{
    public static class CommonVariable
    {
        public static string ConnectionString = "Data Source=10.200.122.16;Initial Catalog=SalesLogix;User ID=Drai;Password=abc123!@#A!;";
        //public static string ConnectionString = ConfigurationManager.ConnectionStrings["TenantManagerConnectionString"]?.ConnectionString;
    }
}
