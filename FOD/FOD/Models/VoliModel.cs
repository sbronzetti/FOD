using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FOD.Models
{
    public class VoliModel
    {
        public List<Voli> Voli {get;set;}
        public List<Voli> TuttiVoli { get; set; }
        public List<string> TuttiMesi { get; set; }
        public Voli RicercaVoli { get; set; }
        
        public string H2 { get; set; }
        public MvcHtmlString Wiki { get; set; }
        public Voli BestVolo { get; set; }
        public Voli BestVoloNational { get; set; }
        public Voli BestVoloInternational { get; set; }
        public Voli BestVoloMare { get; set; }
        public Voli BestVoloCapitali { get; set; }

        public List<City> Cities { get; set; }
    }
}