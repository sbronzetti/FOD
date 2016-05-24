using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Web.Mvc;

namespace FOD.Helpers
{

    public class SearchFilter
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public String AirportFrom { get; set; }
        public String AirportTo { get; set; }
        public Boolean? Weekend { get; set; }
        public int Days { get; set; }
        public int Price { get; set; }
    }

    public static class VoloHelper
    {
        /// <summary>
        /// Elimina i voli piu vecchi di oggi
        /// </summary>
        public static void ClearOldFly()
        {
            using (var context = new FodEntities())
            {
                foreach (var old in context.Voli.Where(x => x.Dal <= DateTime.Now).ToList())
                {
                    context.Voli.DeleteObject(old);
                }

                context.SaveChanges();
            }

        }

        public static Voli GetBestVoloInternational()
        {
            Voli v = new Voli();

            using (var context = new FodEntities())
            {
                foreach (var vo in context.Voli.OrderBy(x => x.Price).ToList())
                {
                    if (!context.Airports.Any(x => x.Name.ToLower().Contains(vo.To.ToLower())))
                    {
                        v = vo;
                        return v;
                    }
                }

            }
            return v;



        }


        public static Voli GetBestVolo()
        {
            Voli v = new Voli();
            using (var context = new FodEntities())
            {
                v = context.Voli.OrderBy(x => x.Price).FirstOrDefault();

            }

            return v;
        }


        public static List<City> GetCities()
        {
            using (var context = new FodEntities())
            {
                return context.Cities.ToList();
            }
        }


        public static List<Voli> GetAllVoli()
        {

            List<Voli> Voli = new List<Voli>();
           // List<Airports> listAirportSelected = Helpers.AirportHelper.GetAirportsFromList();
            using (var context = new FodEntities())
            {
                var aeroportiPartenza = context.Voli.Select(x => x.FromCode).Distinct().ToList();
                foreach (var froms in aeroportiPartenza)
                {

                    var voli = context.Voli.Where(x => x.FromCode.ToLower() == froms.ToLower()).OrderBy(x => x.Price).ToList();
                    foreach (var vo in voli)
                    {
                        if (isWeekend(vo.Dal, vo.Al)) vo.Weekend = true;
                        vo.Days = ((TimeSpan)((DateTime)vo.Al).Subtract(((DateTime)vo.Dal))).Days;
                        Voli.Add(vo);
                    }
                }
            }


            return Voli;

        }


        public static bool isWeekend(DateTime? dal, DateTime? al)
        {
            var dalDinamic = dal;
            while (dalDinamic <= al)
            {
                if (((DateTime)dalDinamic).DayOfWeek == DayOfWeek.Saturday || ((DateTime)dalDinamic).DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }
                dalDinamic = ((DateTime)dalDinamic).AddDays(1);
            }
            return false;
        }


        public static MvcHtmlString GetWiki(string id)
        {
            WebClient client = new WebClient();

            string html = client.DownloadString("http://it.wikipedia.org/wiki/" + id);
            if (html.Contains("<table class=\"sinottico\""))
            {
                html = html.Remove(0, html.IndexOf("<table class=\"sinottico\""));
                if (html.Contains("<p><b>"))
                {
                    html = html.Remove(html.IndexOf("<p><b>"));
                }
                else html = html.Remove(html.IndexOf("<p>"));
               // html += "</table>";
            }
            else
                html = "";

            //html = html.Remove(html.IndexOf("Sito istituzionale"));
            //html += "Sito istituzionale</a></th></tr></tbody></table>";
            return MvcHtmlString.Create(html);
        }


        public static List<string> GetMesi()
        {
            List<string> Mesi = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                var date = DateTime.Now.AddMonths(i);
                var outdate = Convert.ToDateTime("01/" + date.Month + "/" + date.Year);
                Mesi.Add(outdate.ToShortDateString());
            }


            return Mesi;
        }



        public static List<Voli> GetVoli()
        {
            List<Voli> Voli = new List<Voli>();
            //List<Airports> listAirportSelected = Helpers.AirportHelper.GetAirportsFromList();
            using (var context = new FodEntities())
            {
                var aeroportiPartenza = context.Voli.Select(x => x.FromCode).Distinct().ToList();
                foreach (var froms in aeroportiPartenza)
                {
                    //var bestFly = context.Voli.OrderBy(x => x.Price).FirstOrDefault();
                    var voli = context.Voli.Where(x => x.FromCode.ToLower() == froms.ToLower()).GroupBy(x => x.To).Select(grp => grp.FirstOrDefault()).OrderBy(x => x.Price).ToList();
                    foreach (var vo in voli)
                    {
                        if (isWeekend(vo.Dal, vo.Al)) vo.Weekend = true;
                        vo.Days = ((TimeSpan)((DateTime)vo.Al).Subtract(((DateTime)vo.Dal))).Days;
                        Voli.Add(vo);
                    }
                }
            }


            return Voli;

        }



        public static List<Voli> GetSearchVoli(SearchFilter f)
        {
            
            List<Voli> Voli = GetAllVoli();
            
            if(f.Days > 0)
            {
               Voli = Voli.Where(x=>x.Days == f.Days).ToList();
            }
            if(!string.IsNullOrEmpty(f.AirportFrom))
            {
                Voli = Voli.Where(x=>x.From.ToLower().Contains(f.AirportFrom.ToLower())).ToList();
            }
            if(!string.IsNullOrEmpty(f.AirportTo))
            {
                Voli = Voli.Where(x=>x.To.ToLower().Contains(f.AirportTo.ToLower())).ToList();
            }
            if(f.Price>0)
            {
                Voli = Voli.Where(x=>x.Price <= f.Price).ToList();
            }
            if (f.Weekend != null)
            {
                Voli = Voli.Where(x => x.Weekend == f.Weekend).ToList();
            }
            if (f.DateFrom != null)
            {
                Voli = Voli.Where(x => x.Dal >= (DateTime)f.DateFrom).ToList();
            }
            if (f.DateTo != null)
            {
                Voli = Voli.Where(x => x.Al <= (DateTime)f.DateTo).ToList();
            }



            return Voli;

        }



    }
}