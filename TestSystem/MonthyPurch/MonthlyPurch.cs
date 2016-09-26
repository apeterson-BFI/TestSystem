using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TestSystem.MonthyPurch
{
    public class MonthlyPurch
    {
        public static string connString = "Data Source=ER12;Initial Catalog=GMS2;Integrated Security=True";

        public SqlConnection sqlCon;

        static string rcmd = "SELECT Receipt_ID, Trans_Date, Grower_ID, NetPayment FROM ReceiptSummaryView";
        static string ccmd = "SELECT Check_Number, Check_Date, Grower_ID, Amount FROM Check";

        static string fileOut = @"c:\users\aap\desktop\monthlypurchases.txt";
        StreamWriter sw;

        public MonthlyPurch()
        {
            sqlCon = new SqlConnection(connString);
        }

        public void runMonthlyPurch()
        {
            sqlCon.Open();

            sw = File.CreateText(fileOut);

           
        }
    }
}
