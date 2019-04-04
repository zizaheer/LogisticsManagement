using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public static class CacheKeys
    {
        public static string Cities { get { return "_Cities"; } }
        public static string Countries { get { return "_Countries"; } }
        public static string Provinces { get { return "_Provinces"; } }
        public static string Tariffs { get { return "_Tariffs"; } }
        public static string Addresses { get { return "_Addresses"; } }
        public static string Customers { get { return "_Customers"; } }
        public static string Employees { get { return "_Employees"; } }
    }
}
