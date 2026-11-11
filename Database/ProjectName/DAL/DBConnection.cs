
using Microsoft.Data.SqlClient;

namespace RetailSalesSystem.DAL
{
    public class DBConnection
    {
        private static string connectionString =
            @"Data Source=LAPTOP-DH21KTTC\SQLEXPRESS;
              Initial Catalog=SalesDB;
              Integrated Security=True;
              TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
