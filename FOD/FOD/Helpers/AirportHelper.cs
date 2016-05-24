using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FOD.Helpers
{
    public static class AirportHelper
    {

        public static List<Airports> GetAirports()
        {
            
            using (var context = new FodEntities())
            {

                return context.Airports.ToList();
            }
        }

        public static List<string> GetItalianAirports()
        {
            using (var context = new FodEntities())
            {

                return context.Airports.Select(x=>x.Name).ToList();
            }

        }


        public static List<Airports> GetAirportsFromList()
        {
            List<Settings> settings = SettingsHelper.GetSettings();
            var airports = settings.Where(x => x.Key == "Airports").FirstOrDefault().Value;

            List<Airports> listAirportSelected = new List<Airports>();
            foreach (string air in airports.Split(','))
            {
                using (var context = new FodEntities())
                {
                    var a = context.Airports.Where(x => x.Id.ToLower()== air.ToLower()).FirstOrDefault();
                    if (a != null)
                    {
                        listAirportSelected.Add(a);
                    }
                }
            }
            return listAirportSelected;

        }

        public static void PopulateAirports()
        {
            List<Settings> settings = SettingsHelper.GetSettings();
            var refresh = settings.Where(x => x.Key == "RefreshAirports").FirstOrDefault();

            if (refresh != null && Convert.ToBoolean(refresh.Value))
            {
                List<Airports> airportsList = new List<Airports>();
                AirportInfo.airportSoapClient ai = new AirportInfo.airportSoapClient();
                var countries = settings.Where(x => x.Key == "SearchCountries").FirstOrDefault();
                if (countries != null)
                {
                    foreach (var country in countries.Value.Split(','))
                    {

                        string airports = ai.GetAirportInformationByCountry(country);
                        XDocument dox = XDocument.Parse(airports);
                        var AirportCodes = dox.Descendants().Where(x => x.Name == "AirportCode").Distinct().ToList();
                        var AirportNames = dox.Descendants().Where(x => x.Name == "CityOrAirportName").Distinct().ToList();

                        foreach (var air in AirportCodes)
                        {
                            if (!airportsList.Any(x => x.Id == air.Value))
                            {
                                var index = AirportCodes.IndexOf(air);
                                airportsList.Add(new Airports
                                {
                                    Country = country,
                                    Id = air.Value,
                                    Name = AirportNames.ElementAt(index).Value

                                });
                            }

                        }
                    }

                }

                using (var context = new FodEntities())
                {
                    foreach (var acode in airportsList)
                    {
                        context.Airports.AddObject(acode);


                    }
                    context.SaveChanges();
                }

            }

        }
    }
}