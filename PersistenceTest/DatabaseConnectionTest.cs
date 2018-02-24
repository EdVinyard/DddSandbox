using NUnit.Framework;
using Persistence;
using System;
using System.Data.SqlClient;

namespace PersistenceTest
{
    [TestFixture]
    public class DatabaseConnectionTest
    {
        [Test]
        public void CanConnectToDatabase()
        {
            using (var conn = new SqlConnection(Config.MsSqlDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("select * from sys.databases", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) Console.WriteLine(reader[0]);
                }
            }
        }
    }
}
