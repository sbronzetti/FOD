using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Xml;
using System.IO;

namespace FOD.Helpers
{
    public static class SearchHelper
    {

        public static bool IsSearchEnabled()
        {
            var res = false;
            var datesearch = DateTime.Now.Date;
            using (var context = new FodEntities())
            {
                if (!context.RicercheVolis.Any(x => x.DataRicerca > datesearch))
                {
                    res = true;
                }

                if (Convert.ToBoolean(Helpers.SettingsHelper.GetSettings().Where(X => X.Key == "ForceSearch").FirstOrDefault().Value))
                {
                    context.Settings.Where(x => x.Key == "ForceSearch").FirstOrDefault().Value = "False";
                    context.SaveChanges();
                    res = true;
                }

                return res;

            }

        }


        public static void ComponiRicercaTransavia()
        {
            var searchFroms = "http://www.transavia.com/hv/it-IT/home";
            WebClient client = new WebClient();
            string html = client.DownloadString(searchFroms);
            //Parse aereoporti
            var froms = html.Remove(0, html.IndexOf("<div id=\"ffrom\">"));
            froms = froms.Remove(froms.IndexOf("</select></div>"));
            froms = froms.Remove(0, froms.IndexOf("<option value=\"ZZZ\"/>")).Replace("<option value=\"ZZZ\"/>", "").Replace("</option>", "§");
            var airports = froms.Split('§');

            var fromItalianAirports = new List<Airports>();
            var toAirports = new List<Airports>();
            List<Airports> listAirportSelected = FOD.Helpers.AirportHelper.GetAirportsFromList();
            foreach (var a in airports)
            {
                if (a.Length >= 15)
                {
                    var air = a.Substring(15, 3);
                    if (listAirportSelected.Any(x => x.Id == air))
                    {
                        fromItalianAirports.Add(new Airports { Id = air });
                    }
                    else
                    {
                        toAirports.Add(new Airports { Id = air });
                    }
                }
            }



            var searchPattern = "http://www.transavia.com/hv/main/nav/processflightqry?toDay={1}&toMonth={3}&lang=it&adults=1&from={4}&fromMonth={2}&to={5}&country=IT&infants=0&children=0&fromDay={0}&opensearchform=true&tab=cal";
            List<RicercheVoli> ParametriRicerca = new List<RicercheVoli>();
            List<Settings> settings = SettingsHelper.GetSettings();
            string price = settings.Where(x => x.Key == "Price").FirstOrDefault().Value.ToString();
            int dateRange = Convert.ToInt32(settings.Where(x => x.Key == "DateRange").FirstOrDefault().Value.ToString());
            int minTrip = Convert.ToInt32(settings.Where(x => x.Key == "MinTrip").FirstOrDefault().Value.ToString());
            int maxTrip = Convert.ToInt32(settings.Where(x => x.Key == "MaxTrip").FirstOrDefault().Value.ToString());
            string dateMin = settings.Where(x => x.Key == "DateMin").FirstOrDefault().Value.ToString();
            foreach (var air in fromItalianAirports)
            {
                DateTime dateOut = string.IsNullOrEmpty(dateMin) ? DateTime.Now : Convert.ToDateTime(dateMin);
                DateTime dateOutCalculed;
                DateTime dateInCalculed;
                DateTime dateIn = dateOut;
                string DateInString = string.Empty;
                string DateOutString = string.Empty;

                dateIn = dateOut.AddDays(dateRange);
                int months = dateRange / 30 + 1;

                foreach (var toDestination in toAirports)
                {
                    bool noroute = false;
                    for (int i = 0; i < months; i++)
                    {
                        if (!noroute)
                        {
                            dateOutCalculed = dateOut.AddDays(30 * i);

                            dateInCalculed = dateOutCalculed.AddDays(30 * i);
                            DateOutString = dateOutCalculed.ToString("yyyy-MM");
                            DateInString = dateInCalculed.ToString("yyyy-MM");

                            string result = string.Empty;
                            bool error = false;
                            string url = string.Format(searchPattern, dateOut.Day.ToString(), dateIn.Day.ToString(), DateOutString, DateInString, air.Id, toDestination.Id);
                            try
                            {
                                result = client.DownloadString(url);
                            }
                            catch (Exception ex)
                            {
                                noroute = true;
                                error = true;

                            }
                            if (!result.Contains("Durante la sua prenotazione si è verificato un errore.") && !error)
                            {

                                ParametriRicerca.Add(new RicercheVoli { Url = url, FromCode = air.Id, Vettore = "Transavia" });
                            }
                        }
                    }
                }




            }

            using (var context = new FodEntities())
            {
                foreach (var p in ParametriRicerca)
                {

                    if (!context.RicercheVolis.Any(x => x.Url == p.Url))
                    {
                        //Verifica Link






                        context.RicercheVolis.AddObject(new RicercheVoli
                        {
                            Url = p.Url,
                            Processato = false,
                            DataRicerca = DateTime.Now,
                            FromCode = p.FromCode,
                            Vettore = p.Vettore

                        });

                    }

                }
                context.SaveChanges();
            }


        }


        public static void ComponiRicerca()
        {

            string searchPattern = "http://www.ryanair.com/it/voli-economici/?price={0}&limit=150&offset=0&from={1}&to=&out-date-start={2}&out-date-end={3}&in-date-start={4}&in-date-end={5}&roundtrip=1&min-trip={6}&max-trip={7}&view=list";
            List<RicercheVoli> ParametriRicerca = new List<RicercheVoli>();
            List<Settings> settings = SettingsHelper.GetSettings();
            string price = settings.Where(x => x.Key == "Price").FirstOrDefault().Value.ToString();
            int dateRange = Convert.ToInt32(settings.Where(x => x.Key == "DateRange").FirstOrDefault().Value.ToString());
            int minTrip = Convert.ToInt32(settings.Where(x => x.Key == "MinTrip").FirstOrDefault().Value.ToString());
            int maxTrip = Convert.ToInt32(settings.Where(x => x.Key == "MaxTrip").FirstOrDefault().Value.ToString());
            string dateMin = settings.Where(x => x.Key == "DateMin").FirstOrDefault().Value.ToString();

            List<Airports> listAirportSelected = FOD.Helpers.AirportHelper.GetAirportsFromList();


            DateTime dateOut = string.IsNullOrEmpty(dateMin) ? DateTime.Now : Convert.ToDateTime(dateMin);
            TimeSpan ts = new TimeSpan(dateRange, 0, 0, 0);
            DateTime dateIn = dateOut.Add(ts);
            string DateOutString = dateOut.ToString("yyyy-MM-dd");
            string DateInString = dateIn.ToString("yyyy-MM-dd");

            foreach (var air in listAirportSelected)
            {
                //DateTime dateOut = string.IsNullOrEmpty(dateMin) ? DateTime.Now : Convert.ToDateTime(dateMin);
                //DateTime dateOutCalculed;
                //DateTime dateInCalculed;
                //DateTime dateIn = dateOut;
                //string DateInString = string.Empty;
                //string DateOutString = string.Empty;
                ////for (int i = 0; i < dateRange; i++)
                ////{

                //dateOutCalculed = dateOut.AddDays(dateRange);
                    for (int j = minTrip; j < maxTrip; j++)
                    {
                        //dateInCalculed = dateOutCalculed.AddDays(j);
                        //DateOutString = dateOutCalculed.ToString("yyyy-MM-dd");
                        //DateInString = dateInCalculed.ToString("yyyy-MM-dd");
                        string url = string.Format(searchPattern, price, air.Id, DateOutString, DateInString, DateOutString, DateInString, j.ToString(), j.ToString());
                        ParametriRicerca.Add(new RicercheVoli { Url = url, FromCode = air.Id, Vettore = "Ryanair" });
                    }

                //}
            }


            using (var context = new FodEntities())
            {
                foreach (var p in ParametriRicerca)
                {

                    if (!context.RicercheVolis.Any(x => x.Url == p.Url))
                    {
                        context.RicercheVolis.AddObject(new RicercheVoli
                        {
                            Url = p.Url,
                            Processato = false,
                            DataRicerca = DateTime.Now,
                            FromCode = p.FromCode,
                            Vettore = p.Vettore

                        });
                    }
                    context.SaveChanges();
                }
            }


        }

        public static void ClearRicerche()
        {

            using (var context = new FodEntities())
            {
                foreach (var rv in context.RicercheVolis.Where(x => x.Processato == true).ToList())
                {
                    context.DeleteObject(rv);
                }


                context.SaveChanges();
            }


        }







        public static void ProcessaRicerche(List<RicercheVoli> queries)
        {



            List<Voli> Voli = new List<Voli>();
            foreach (var url in queries)
            {
                if (url.Vettore == "Ryanair")
                {
                    WebClient client = new WebClient();
                    //url.Url = "http://www.ryanair.com/it/voli-economici/?price=80&limit=500&offset=0&from=BLQ&to=&out-date-start=2014-09-24&out-date-end=2015-05-22&in-date-start=2014-09-24&in-date-end=2015-05-22&roundtrip=1&min-trip=3&max-trip=3&view=list";
                    string html = client.DownloadString(url.Url);
                    string htmloffers = html.Remove(0, html.IndexOf("<div data-view=\"list\" class=\"view view-page list list-fares float-l pn-r\" id=\"list-fares\">"));
                    htmloffers = htmloffers.Remove(htmloffers.IndexOf("<script type=\"text/javascript\">"));
                    htmloffers = htmloffers.Replace("<a title=", "§");
                    string[] rows = htmloffers.Split('§');


                    foreach (string s in rows)
                    {
                        if (s.Contains("no-results"))
                        {
                            continue;
                        }
                        if (s.Contains("<span class=\"airport d-inblock float-l fs16\">"))
                        {
                            string offertRow = s.Remove(0, s.IndexOf(">") + 1);
                            offertRow = System.Text.RegularExpressions.Regex.Replace(offertRow, "<[^>]*>", " ");
                            offertRow = System.Text.RegularExpressions.Regex.Replace(offertRow, @"\s+", " ");
                            string[] elements = offertRow.Split(' ');
                            string dateIn = s.Remove(0, s.IndexOf("in-date=") + "in-date=".Length);
                            dateIn = dateIn.Remove(dateIn.IndexOf("&out-date="));
                            string dateOut = s.Remove(0, s.IndexOf("&out-date=") + "&out-date=".Length);
                            dateOut = dateOut.Remove(dateOut.IndexOf(">") - 1);
                            
                            Voli.Add(new Voli
                            {

                                From = elements[1],
                                To = elements[2].Contains("(")? elements[3] : elements[2],
                                Price = searchPrice(elements),
                                Dal = Convert.ToDateTime(dateOut),
                                Al = Convert.ToDateTime(dateIn),
                                DataRicerca = DateTime.Now,
                                IsPriceChanged = false,
                                FromCode = url.FromCode,
                                Vettore = url.Vettore,
                                Star = false
                            });


                        }

                    }



                }

            }

            using (var context = new FodEntities())
            {
                foreach (var q in queries)
                {
                    context.RicercheVolis.Where(x => x.Id == q.Id).FirstOrDefault().Processato = true;

                }

                foreach (var v in Voli)
                {
                    var old = context.Voli.Where(x => x.Dal == v.Dal && x.Al == v.Al && x.From == v.From && x.To == v.To).FirstOrDefault();
                    if (old != null)
                    {
                        if (v.Price > old.Price)
                        {
                            v.IsPriceChanged = true;
                            v.OldPrice = old.Price;
                        }

                    }
                    else
                    {
                        context.AddToVoli(v);
                    }


                    if (!context.Cities.Any(x => x.Name.ToLower() == v.From.ToLower()))
                    {
                        context.Cities.AddObject(new City
                        { 
                             Name = v.From,
                              Visible = true,
                              Type = 0
                        
                        });
                    }
                    if (!context.Cities.Any(x => x.Name.ToLower() == v.To.ToLower()))
                    {
                        context.Cities.AddObject(new City
                        {
                            Name = v.To,
                            Visible = true,
                            Type = 0

                        });
                    }


                }
                context.SaveChanges();


                


            }

        }


        public static void ProcessaRicercheTransavia(List<RicercheVoli> queries)
        {

            try
            {

                List<Voli> Voli = new List<Voli>();
                WebClient client = new WebClient();
                foreach (var url in queries)
                {


                    try
                    {
                        string html = client.DownloadString(url.Url);

                        //Recupero From -> To
                        string htmlDa = html.Remove(0, html.IndexOf("<div id=\"toflight"));
                        htmlDa = htmlDa.Remove(htmlDa.IndexOf("<div class=\"right\">"));
                        htmlDa = htmlDa.Remove(0, htmlDa.LastIndexOf("alt="));
                        htmlDa = System.Text.RegularExpressions.Regex.Replace(htmlDa, "<[^>]*>", " ");
                        htmlDa = System.Text.RegularExpressions.Regex.Replace(htmlDa, @"\s+", " ");
                        htmlDa = htmlDa.Remove(0, htmlDa.IndexOf("\"") + 1);
                        htmlDa = htmlDa.Remove(htmlDa.IndexOf("\""));
                        string fromAirport = htmlDa.Split('-')[0].Trim();
                        string toAirport = htmlDa.Split('-')[1].Trim();

                        //Recupero Calendar From
                        var htmlCalendar = html.Remove(0, html.IndexOf("<table class=\"calenderview\">"));
                        htmlCalendar = htmlCalendar.Remove(htmlCalendar.IndexOf("</table>"));
                        var calElements = htmlCalendar.Replace("<label for=", "§").Split('§');
                        List<Voli> Andate = new List<Voli>();
                        List<Voli> Ritorni = new List<Voli>();
                        foreach (var e in calElements)
                        {
                            if (!e.Contains("calenderview"))
                            {

                                var price = e.Remove(0, e.IndexOf("â‚¬Â")).Replace("â‚¬Â", "").Trim();
                                price = price.Remove(price.IndexOf("</span>")).Replace("</span>", "");
                                Andate.Add(new Voli
                                {

                                    From = fromAirport,
                                    To = toAirport,
                                    Price = Convert.ToDouble(price),
                                    Dal = Convert.ToDateTime(e.Substring(1, 10)),
                                    DataRicerca = DateTime.Now,
                                    IsPriceChanged = false,
                                    FromCode = url.FromCode,
                                    Vettore = url.Vettore
                                });
                            }
                        }



                        //Recupero Calendar To
                        var htmlCalendarTo = html.Remove(0, html.LastIndexOf("<table class=\"calenderview\">"));
                        htmlCalendarTo = htmlCalendarTo.Remove(htmlCalendarTo.IndexOf("</table>"));
                        var calElementsTo = htmlCalendarTo.Replace("<label for=", "§").Split('§');

                        foreach (var e in calElementsTo)
                        {
                            if (!e.Contains("calenderview"))
                            {
                                var price = e.Remove(0, e.IndexOf("â‚¬Â")).Replace("â‚¬Â", "").Trim();
                                price = price.Remove(price.IndexOf("</span>")).Replace("</span>", "");
                                Ritorni.Add(new Voli
                                {

                                    From = fromAirport,
                                    To = toAirport,
                                    Price = Convert.ToDouble(price),
                                    Al = Convert.ToDateTime(e.Substring(1, 10)),
                                    DataRicerca = DateTime.Now,
                                    IsPriceChanged = false,
                                    FromCode = url.FromCode,
                                    Vettore = url.Vettore
                                });
                            }
                        }






                        foreach (var a in Andate)
                        {
                            foreach (var r in Ritorni)
                            {
                                if (r.Al > a.Dal)
                                {
                                    Voli.Add(new Voli
                                    {
                                        From = a.From,
                                        To = a.To,
                                        DataRicerca = a.DataRicerca,
                                        IsPriceChanged = false,
                                        FromCode = a.FromCode,
                                        Vettore = a.Vettore,
                                        Dal = a.Dal,
                                        Al = r.Al,
                                        Price = a.Price + r.Price
                                    });

                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }


                }
                
                using (var context = new FodEntities())
                {
                    int price = Convert.ToInt32(context.Settings.Where(x => x.Key == "Price").FirstOrDefault());
                    foreach (var q in queries)
                    {
                        context.RicercheVolis.Where(x => x.Id == q.Id).FirstOrDefault().Processato = true;

                    }

                    foreach (var v in Voli)
                    {
                        var old = context.Voli.Where(x => x.Dal == v.Dal && x.Al == v.Al && x.From == v.From && x.To == v.To).FirstOrDefault();
                        if (old != null)
                        {
                            if (v.Price > old.Price)
                            {
                                v.IsPriceChanged = true;
                                v.OldPrice = old.Price;
                            }

                        }
                        else
                        {
                            if (v.Price <= price)
                            {
                            context.AddToVoli(v);
                            }
                        }
                    }
                    context.SaveChanges();


                }
            }
            catch (Exception ex)
            {

            }

        }



        public static double searchPrice(string[] s)
        {
            int index = -1;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    bool res = Int32.TryParse(s[i], out index);
                    if (res)
                    {
                        string price = s[i + 3] + s[i + 4];
                        return Convert.ToDouble(price);

                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }


        public static List<RicercheVoli> ElaboraRicerche()
        {
            using (var context = new FodEntities())
            {
                var step = Convert.ToInt32(context.Settings.Where(x => x.Key == "StepProcess").FirstOrDefault().Value);
                return context.RicercheVolis.Where(x => x.Processato == false).Take(step).ToList();

            }
        }


        public static FOD.Models.ProgressBarModel GetStatusBar()
        {
            FOD.Models.ProgressBarModel pr = new FOD.Models.ProgressBarModel();
            using (var context = new FodEntities())
            {
                pr.Done = context.RicercheVolis.Where(x => x.Processato == true).Count();
                pr.Todo = context.RicercheVolis.Count();


            }
            return pr;
        }

    }
}