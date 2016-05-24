using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace FOD.Helpers
{
    public static class MailHelper
    {
        public static bool isSendToday()
        {
            using (var context = new FodEntities())
            {
                var lastdatesend = Convert.ToDateTime(context.Settings.Where(x => x.Key == "LastMailingData").FirstOrDefault().Value);
                var SendDailyReport = Convert.ToBoolean(context.Settings.Where(x => x.Key == "SendDailyReport").FirstOrDefault().Value);

                if (SendDailyReport)
                {
                    if (lastdatesend.Date < DateTime.Now.Date)
                    {
                        return false;
                    }
                    else return true;
                }
                else return true;

            }
        }


        public static void SendReminder()
        {
            List<Voli> Voli = new List<Voli>();
            var pattern = "<div style='padding: 5px;font-size:12px;clear:both;'><div style='line-height:15px'><div style='width:60px;float:left;font-weight:bold;padding-right: 10px;'>&euro; {0}</div> <div style='float:left;width:80px'>{1}</div><div style='float:left; padding-right:10px'><></div><div style='float:left;width:80px;'>{2}</div><div style='float:left;padding-right:10px;'>dal {3} </div> <div style='float:left;padding-right:10px;'>al {4} </div><div style='float:left;width:70px;'>[{6} {7} ]</div><div style='float:left'> -> {5} </div></div></div>";
            string body = "<meta charset=\"utf-8\"/><div style=' font-size: 12px; font-family:\"verdana\"; color: #22356b'><h1 style='line-height: 30px; font-size:30px;font-weight:lighter'>Flight Offers Daily</h1><hr/>";
            List<Airports> listAirportSelected = Helpers.AirportHelper.GetAirportsFromList();
             string MailingList;
            using (var context = new FodEntities())
            {
                
                    var bestFly = context.Voli.OrderBy(x => x.Price).FirstOrDefault();
                    if (bestFly != null)
                    {
                        var we = bestFly.Weekend == null ? false : true;
                        body += "<h2 style='line-height: 20px; font-size:18px;font-weight:lighter; border-bottom: 1px solid #eeeee'>Best Fly: &euro; <b>" + bestFly.Price + "</b></h2>";
                        body += string.Format(pattern, bestFly.Price, bestFly.From, bestFly.To, bestFly.Dal.Value.ToString("dd-MM-yyyy"), bestFly.Al.Value.ToString("dd-MM-yyyy"), bestFly.Vettore, (bool)we ? " WE /" : "", bestFly.Days.ToString());
                    }
                    body += "<div style='clear:both;'></div>";
                    var voli = VoloHelper.GetVoli();
                    if (voli.Count > 0)
                    {
                        body += "<h2 style='line-height: 15px; font-size:18px;font-weight:lighter; border-bottom: 1px solid #eeeee'>Migliori offerte per destinazione</h2>";
                        foreach (var v in voli)
                        {
                            var we = v.Weekend == null ? false :  true;
                            body += string.Format(pattern, v.Price, v.From, v.To, v.Dal.Value.ToString("dd-MM-yyyy"), v.Al.Value.ToString("dd-MM-yyyy"), v.Vettore, (bool)we? " WE /" : "", v.Days.ToString());
                        }

                    }

                
                MailingList = context.Settings.Where(x => x.Key == "MailingList").FirstOrDefault().Value;
            }



            var mail = new MailMessage("flightoffers@fod.bronztec.com", MailingList.ToString(), "Flight Offers Daily", body += "</div>");
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("relay-hosting.secureserver.net");
            
            smtp.Credentials = new System.Net.NetworkCredential("info@rugiainternational.com", "rugiaint11*!");
            smtp.Port = 25;
            smtp.EnableSsl = false;

            smtp.Send(mail);

            using (var context = new FodEntities())
            {
                context.Settings.Where(x => x.Key == "LastMailingData").FirstOrDefault().Value = DateTime.Now.Date.ToShortDateString();
                context.SaveChanges();
            }


        }







        

    }
}