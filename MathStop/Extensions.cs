using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MathStop
{
   class Extensions
   {
      public static bool IsNumber(string number)
      {
         return Regex.IsMatch(number, @"^\d+$");
      }

      public static bool IsOperator(string input)
      {
         return Regex.IsMatch(input, @"*/-\+");
      }

      public static bool IsParenthesis(string input)
      {
         return Regex.IsMatch(input, @"\(\)");
      }
   }
}
