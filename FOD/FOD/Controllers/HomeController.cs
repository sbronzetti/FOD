using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using FOD.Helpers;

namespace FOD.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //List<Voli> Voli = new List<Voli>();
            Models.VoliModel v = new Models.VoliModel();
            v.BestVoloCapitali = VoloHelper.GetBestVolo();
            v.BestVoloInternational = VoloHelper.GetBestVolo();
            v.BestVoloMare = VoloHelper.GetBestVolo();
            v.BestVoloNational = VoloHelper.GetBestVolo();
            v.BestVolo = VoloHelper.GetBestVolo();

            v.Cities = VoloHelper.GetCities();
            v.Voli = VoloHelper.GetVoli();
            //v.TuttiVoli = VoloHelper.GetAllVoli();
            
            //v.BestVoloInternational = VoloHelper.GetBestVoloInternational();
            return View(v);
        }

      

        public ActionResult Voli(string id)
        {
            Models.VoliModel v = new Models.VoliModel();
           
            v.TuttiMesi = VoloHelper.GetMesi();

            v.TuttiVoli = VoloHelper.GetAllVoli();
            v.Cities = VoloHelper.GetCities();
            v.H2 = v.TuttiVoli.Count() + " voli disponibili";

            if (id != null)
            {
                DateTime dateFromFilter;
                if (id.Contains("_"))
                {
                    if (DateTime.TryParse(id.Split('_')[1].Replace("-","/"), out dateFromFilter))
                    {
                        id = id.Split('_')[0];
                        if (id == "Tutti")
                        {
                            var voli = v.TuttiVoli.Where(x => x.Dal >= dateFromFilter).ToList();
                            v.TuttiVoli = voli.OrderBy(x => x.Price).ToList();
                        }
                        else
                        {
                            var voli = v.TuttiVoli.Where(x => x.From.ToLower() == id.ToLower() || x.To.ToLower() == id.ToLower()).Where(x => x.Dal >= dateFromFilter).ToList();
                            v.TuttiVoli = voli.OrderBy(x => x.Price).ToList();
                        }
                    }
                }
                else
                {
                    var voli = v.TuttiVoli.Where(x => x.From.ToLower() == id.ToLower() || x.To.ToLower() == id.ToLower()).ToList();
                    v.TuttiVoli = voli.OrderBy(x => x.Price).ToList();
                }

                v.H2 = v.TuttiVoli.Count().ToString() + " voli da/per " + id;
                v.Wiki = VoloHelper.GetWiki(id);
            }

            var voliserarch = VoloHelper.GetSearchVoli(new SearchFilter { AirportFrom = "bologna", AirportTo="berlino", Days = 0, Price = 80, Weekend = true });
            v.TuttiVoli = voliserarch;
          
            return View(v);
        }




        public ActionResult Packages(string id)
        {
            Models.PackageModel v = new Models.PackageModel();
            if (id != null)
            {
                int days = Convert.ToInt32(id);
                v.Voli = PackageHelper.GetAllPackage().Where(x=>x.Days <= days).OrderBy(X => X.PackagePrice).ToList();
            }
            else
            {
                v.Voli = PackageHelper.GetAllPackage().OrderBy(X => X.PackagePrice).ToList();
            }
            return View(v);

        }





        public ActionResult Airports()
        {
            Models.AirportsModel s = new Models.AirportsModel();
            s.Airports = AirportHelper.GetAirports();
            return View(s);


            
        }

        public ActionResult About()
        {

           
            
            return View();
        
        }


        public ActionResult Settings()
        {
            Models.SettingsModel s = new Models.SettingsModel();
            s.Settings = SettingsHelper.GetSettings();
            s.Airports = s.Settings.Where(x => x.Key == "Airports").FirstOrDefault().Value;
            s.DateMin = s.Settings.Where(x => x.Key == "DateMin").FirstOrDefault().Value;
            s.DateRange = s.Settings.Where(x => x.Key == "DateRange").FirstOrDefault().Value;
            s.ForceSearch = s.Settings.Where(x => x.Key == "ForceSearch").FirstOrDefault().Value;
            s.MaxTrip = s.Settings.Where(x => x.Key == "MaxTrip").FirstOrDefault().Value;
            s.MinTrip = s.Settings.Where(x => x.Key == "MinTrip").FirstOrDefault().Value;
            s.Price = s.Settings.Where(x => x.Key == "Price").FirstOrDefault().Value;
            s.RefreshAirports = s.Settings.Where(x => x.Key == "RefreshAirports").FirstOrDefault().Value;
            s.SearchCountries = s.Settings.Where(x => x.Key == "SearchCountries").FirstOrDefault().Value;
            s.SendDailyReport = s.Settings.Where(x => x.Key == "SendDailyReport").FirstOrDefault().Value;
            s.StepProcess = s.Settings.Where(x => x.Key == "StepProcess").FirstOrDefault().Value;
            s.gpl = s.Settings.Where(x => x.Key == "gpl").FirstOrDefault().Value;
            return View(s);
        }

        [HttpPost]
        public ActionResult Settings(Models.SettingsModel model)
        {
           using (var context = new FodEntities())
           {
               context.Settings.Where(x => x.Key == "Airports").FirstOrDefault().Value = model.Airports;
               context.Settings.Where(x => x.Key == "DateMin").FirstOrDefault().Value = model.DateMin;
               context.Settings.Where(x => x.Key == "DateRange").FirstOrDefault().Value = model.DateRange;
               context.Settings.Where(x => x.Key == "ForceSearch").FirstOrDefault().Value = model.ForceSearch;
               context.Settings.Where(x => x.Key == "MaxTrip").FirstOrDefault().Value = model.MaxTrip;
               context.Settings.Where(x => x.Key == "MinTrip").FirstOrDefault().Value = model.MinTrip;
               context.Settings.Where(x => x.Key == "Price").FirstOrDefault().Value = model.Price;
               context.Settings.Where(x => x.Key == "RefreshAirports").FirstOrDefault().Value = model.RefreshAirports;
               context.Settings.Where(x => x.Key == "SearchCountries").FirstOrDefault().Value = model.SearchCountries;
               context.Settings.Where(x => x.Key == "SendDailyReport").FirstOrDefault().Value = model.SendDailyReport;
               context.Settings.Where(x => x.Key == "StepProcess").FirstOrDefault().Value = model.StepProcess;
               context.Settings.Where(x => x.Key == "gpl").FirstOrDefault().Value = model.gpl;

               context.SaveChanges();
           }



            return RedirectToAction("settings", "home");
        }



        public ActionResult Processa()
        {
           

            //Elimina i voli piu vecchi di oggi
            FOD.Helpers.VoloHelper.ClearOldFly();

            //Elaborazione ricerche (se prima esecuzione giornaliera)
            if (FOD.Helpers.SearchHelper.IsSearchEnabled())
            {
                FOD.Helpers.SearchHelper.ClearRicerche();
                FOD.Helpers.SearchHelper.ComponiRicerca();
                //FOD.Helpers.SearchHelper.ComponiRicercaTransavia();
                
            }

            var ricerche = FOD.Helpers.SearchHelper.ElaboraRicerche();
            

            FOD.Helpers.SearchHelper.ProcessaRicerche(ricerche.Where(x=>x.Vettore == "Ryanair").ToList());
            //FOD.Helpers.SearchHelper.ProcessaRicercheTransavia(ricerche.Where(x => x.Vettore == "Transavia").ToList());

            var status = FOD.Helpers.SearchHelper.GetStatusBar();
            if (status.Todo == status.Done && !MailHelper.isSendToday())
            {
                MailHelper.SendReminder();
            }

           return View(status);
        }


    }
}
