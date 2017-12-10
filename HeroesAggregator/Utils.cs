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
        public static string GetVersionInfo()
        {
            var version = System.Text.Encoding.Default.GetString(Properties.Resources.version);
            return version;
        }
    }
}