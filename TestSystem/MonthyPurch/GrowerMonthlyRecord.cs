using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GMS2.Process;

namespace TestSystem.MonthyPurch
{
    public class GrowerMonthlyRecord
    {
        string growerID;
        string growerName;
        decimal juicePounds;
        decimal peelerPounds;
        decimal totalPounds;
        decimal grossPurchases;
        decimal macmaFees;
        decimal MACfees;
        decimal netPurchases;
        decimal payments;
        decimal balanceDue;

        StreamWriter sw;

        DateTime startDate;
        DateTime endDate;

        List<Receipt> receipts;
        List<Check> checks;
    }
}
