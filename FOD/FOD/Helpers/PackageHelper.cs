using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using FOD.Models;

namespace FOD.Helpers
{
    public static class PackageHelper
    {
        


        public static int isWeekend(DateTime? dal, DateTime? al)
        {
            var dalDinamic = dal;
            int contWE = 0;
            while (dalDinamic <= al)
            {
                if (((DateTime)dalDinamic).DayOfWeek == DayOfWeek.Saturday || ((DateTime)dalDinamic).DayOfWeek == DayOfWeek.Sunday)
                {
                    contWE++;
                }
                dalDinamic = ((DateTime)dalDinamic).AddDays(1);
            }
            return contWE;
        }




        public static List<VoloExtended> GetAllPackage()
        {
            PackageModel p = new PackageModel();
            List<VoloExtended> Voli = new List<VoloExtended>();
            // List<Airports> listAirportSelected = Helpers.AirportHelper.GetAirportsFromList();
            using (var context = new FodEntities())
            {
                var aeroportiPartenza = context.Voli.Select(x => x.FromCode).Distinct().ToList();
                var airportCosts = context.AirportsCosts.ToList();
                var gplcosts = Convert.ToDouble(context.Settings.Where(x => x.Key == "gpl").FirstOrDefault().Value);
                foreach (var froms in aeroportiPartenza)
                {

                    var voli = context.Voli.Where(x => x.FromCode.ToLower() == froms.ToLower()).GroupBy(x => x.To).Select(grp => grp.FirstOrDefault()).OrderBy(x => x.Price).ToList();
                    foreach (var vo in voli)
                    {
                        int isWE = isWeekend(vo.Dal, vo.Al);
                        if (isWE > 0) vo.Weekend = true;
                        vo.Days = ((TimeSpan)((DateTime)vo.Al).Subtract(((DateTime)vo.Dal))).Days;
                        var costs = airportCosts.Where(x => x.Code == vo.FromCode).FirstOrDefault();
                        if (costs != null)
                        {
                            VoloExtended v = new VoloExtended();
                            v.Al = vo.Al;
                            v.Dal = vo.Dal;
                            v.DataRicerca = vo.DataRicerca;
                            v.Days = vo.Days;
                            v.From = vo.From;
                            v.FromCode = vo.FromCode;
                            v.Price = vo.Price;
                            v.To = vo.To;
                            v.Vettore = vo.Vettore;
                            v.Weekend = vo.Weekend;

                            v.Parcheggio = Convert.ToUInt32(vo.Days) * (double)costs.ParkDayCost;
                            v.Carburante = 2* (double)costs.Distance * 0.077 * gplcosts;
                            v.Autostrada = 2* (double)costs.AutorouteCost;

                            v.PackagePrice = (double)v.Price + ((v.Carburante + v.Autostrada + v.Parcheggio) / 2);

                            v.GiorniFerie = Convert.ToInt32(v.Days - isWE);


                            Voli.Add(v);
                        }
                    }
                }
            }


            return Voli;



        }




    }
}