using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FOD.Models
{
    public class SettingsModel
    {
        public List<Settings> Settings { get; set; }
        public string Airports { get; set; }
        public string DateMin { get; set; }
        public string DateRange { get; set; }
        public string ForceSearch { get; set; }
        public string MaxTrip { get; set; }
        public string MinTrip { get; set; }
        public string Price { get; set; }
        public string SearchCountries { get; set; }
        public string SendDailyReport { get; set; }
        public string RefreshAirports { get; set; }
        public string StepProcess { get; set; }
        public string gpl { get; set; }
    }
}