using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FOD.Helpers
{
    public static class SettingsHelper
    {
        public static List<Settings> GetSettings()
        {
            using (var context = new FodEntities())
            {
                return context.Settings.ToList();

            }
        }


    }
}