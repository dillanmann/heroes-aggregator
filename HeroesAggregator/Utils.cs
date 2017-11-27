using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;

namespace HeroesAggregator
{
    public static class Utils
    {
        public static int GetVersionNumber()
        {
            return Convert.ToInt32(System.Text.Encoding.Default.GetString(Properties.Resources.version));
        }
    }
}