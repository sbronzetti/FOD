using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FOD.Models
{
    public class PackageModel
    {
        public List<VoloExtended> Voli { get; set; }

    }


    public class VoloExtended : Voli
    {
        public double Autostrada { get; set; }
        public double Carburante { get; set; }
        public double Parcheggio { get; set; }
        public int GiorniFerie { get; set; }
        public double PackagePrice { get; set; }
    }

}