using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestEFVersions
{
    public class EF6Support
    {
        public EF6Support()
        {
            //Change paths to the bak, mdf and ldf file.

            //Path to the TestEFVersion, xUnit test project.
            var bak = @"D:\Dropbox\Dropbox\Projekti\EFIntercept\TestEFVersions\AdventureWorks2017.bak";
            var mdfTo = @"'C:\Program Files\Microsoft SQL Server\MSSQL10_50.Z1SQL2008R2\MSSQL\DATA\AdventureWorksDW2016.mdf'";
            var ldfTo = @"'C:\Program Files\Microsoft SQL Server\MSSQL10_50.Z1SQL2008R2\MSSQL\DATA\AdventureWorksDW2016.ldf'";

            var sqlCommand = @"USE [master]
                RESTORE DATABASE AdventureWorksDW2016ForEF6Support
                FROM disk=" + bak +
                @"WITH MOVE 'AdventureWorksDW2016_data'" +
                "TO " + mdfTo +
                ", MOVE 'AdventureWorksDW2016_Log'" +
                "TO " + ldfTo + ",REPLACE";

            var connectionString = ConfigurationManager.ConnectionStrings["AdventureWorksDW2016ForEF6Support"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, connection);
                connection.Open();
                command.BeginExecuteNonQuery();
            }
        }

        [Fact]
        public void Select()
        {
            Assert.Equal(2, 3);
        }

    }
}
    