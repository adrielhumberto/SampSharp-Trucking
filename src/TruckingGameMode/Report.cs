using System.Collections.Generic;

namespace TruckingGameMode
{
    public class Report
    {
        public static List<Report> Reports { get;} = new List<Report>();

        public string IssuerName { get; set; }
        public string ReportedName { get; set; }
        public string Message { get; set; }
    }
}