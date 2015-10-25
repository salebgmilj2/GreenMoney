using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using CsvHelper;


namespace GM.Models.Public
{
    //public class UserChallengeCsvModel
    //{
    //    public DateTime DateIssued { get; set; }
    //    public string Participant { get; set; }
    //    public string Suburb { get; set; }
    //    public string Workplace { get; set; }
    //    public string Status { get; set; }

    //}

    public class UserChallengeCsvModel
    {
        public DateTime DateIssued { get; set; }
        public string Participant { get; set; }
        public string Suburb { get; set; }
        public string Status { get; set; }

    }

    public class UserChallengeCsvAusPostModel
    {
        public DateTime DateIssued { get; set; }
        public string Participant { get; set; }
        public string Workplace { get; set; }
        public string Status { get; set; }

    }

    //public sealed class CustomClassMap : CsvHelper.Configuration.CsvClassMap<UserChallengeCsvModel>
    //{
    //    public CustomClassMap()
    //    {
    //        Map(m => m.DateIssued).Name("Date Issued");
    //        Map(m => m.Participant).Name("Participant");
    //        Map(m => m.Suburb).Name("Suburb");
    //        Map(m => m.Status).Name("Status");
    //    }
    //}

    //public sealed class CustomClassMapAuspost : CsvHelper.Configuration.CsvClassMap<UserChallengeCsvModel>
    //{
    //    public CustomClassMapAuspost()
    //    {
    //        Map(m => m.DateIssued).Name("Date Issued");
    //        Map(m => m.Participant).Name("Participant");
    //        Map(m => m.Workplace).Name("Workplace");
    //        Map(m => m.Status).Name("Status");
    //    }
    //}
}
