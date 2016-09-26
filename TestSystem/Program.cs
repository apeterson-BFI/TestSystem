using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using TestSystem.Polynomials;
using System.Globalization;
using TestSystem.InterEvol;
using TestSystem.Evol;

namespace TestSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Evolutionary.runEnviro();

        }        
    }
}