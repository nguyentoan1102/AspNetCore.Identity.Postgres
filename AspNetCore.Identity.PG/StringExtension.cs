using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.PG
{
    public static class StringExtension
    {
        public static string Quote(this string str)
        {
            return "\"" + str + "\"";
        }
    }
}
