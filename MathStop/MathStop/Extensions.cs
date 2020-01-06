using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MathStop_Extensions
{
   static class Extensions
   {
      public static string Remove_Parentheses(this string input)
      {
         try { return input.Replace("(", "").Replace(")", ""); }
         catch { return input; }
      }
   }
}
