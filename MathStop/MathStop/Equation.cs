using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathStop
{
   class Equation
   {
      private static List<Operator> operators = new List<Operator>();
      private static List<Number>   numbers   = new List<Number>();
      private static string equation = string.Empty;
      private static string parsedEquation = string.Empty;

      private static void AddOperator(string o)
      {
         operators.Add(new Operator() {op = o });
         return;
      }
      private static void AddNumber(string numb)
      {
         numbers.Add(new Number() { Snumb = numb });
         return;
      }
      private static void RemoveOperator(int index)
      {
         operators.RemoveAt(index);
         return;
      }
      private static void RemoveNumber(int index)
      {
         numbers.RemoveAt(index);
         return;
      }
      public void AddEquation(string input)
      {
         equation = input;
      }
      public string GetEquation()
      {
         return equation;
      }
      public string GetParsedEquation()
      {
         return parsedEquation;
      }
      public int ParseEquation()
      {
         Stack<string> MStack = new Stack<string>();
         string output = string.Empty;
         string temp_pop;
         int isValid = 0;
         //Regex rgx = new Regex(@"[(\d*\.?\d+){1}+]");
         Regex rgx = new Regex(@"[(\d*(\.\d+))\+\*\-\/\(\)]");
         foreach (Match m in rgx.Matches(equation))
         {
            switch (m.Value)
            {
               // Multiplication, Division
               case "*":
               case "/":
                  isValid = 0;
                  bool Starstop = false;
                  do
                  {
                     try { temp_pop = MStack.Pop(); } catch (InvalidOperationException) { temp_pop = ""; }
                     switch (temp_pop)
                     {
                        case "*":
                        case "/":
                           output += temp_pop;
                           Starstop = true;
                           break;
                        case "(":
                        case "+":
                        case "-":
                           MStack.Push(temp_pop);
                           MStack.Push(m.Value);
                           Starstop = false;
                           break;
                        case "":
                           MStack.Push(m.Value);
                           Starstop = false;
                           break;
                        default:
                           return 0;
                     }
                  } while (Starstop);
                  break;
               // Addition, Subtraction
               case "+":
               case "-":
                  isValid = 0;
                     bool Plusstop = false;
                     do
                     {
                        try { temp_pop = MStack.Pop(); } catch (InvalidOperationException) { temp_pop = ""; }
                        switch (temp_pop)
                        {
                           case "*":
                           case "/":
                           case "+":
                           case "-":
                              output += temp_pop;
                              Plusstop = true;
                              break;
                           case "(":
                              MStack.Push("(");
                              MStack.Push(m.Value);
                              Plusstop = false;
                              break;
                           case "":
                              MStack.Push(m.Value);
                              Plusstop = false;
                              break;
                           default:
                              return 0;
                        }
                     } while (Plusstop);
                  break;
               // Closing Parenthesis
               case ")":
                  isValid = 1;
                  bool RPStop = false;
                  do
                  {
                     try { temp_pop = MStack.Pop(); } catch (InvalidOperationException) { temp_pop = ""; }
                     switch (temp_pop)
                     {
                        case "+":
                        case "*":
                        case "/":
                        case "-":
                           output += temp_pop;
                           RPStop = true;
                           break;
                        case "(":
                           RPStop = false;
                           break;
                        case "":
                           RPStop = false;
                           break;
                        default:
                           return 0;
                     }
                  } while (RPStop);
                  break;
               // Opening Parenthesis
               case "(":
                  isValid = 0;
                  MStack.Push("(");
                  break;
               // Some number
               case string someVal when new Regex(@"(\d*(\.\d+)?)").IsMatch(someVal.ToString()):
                  isValid = 1;
                  output += someVal;
                  output += " ";
                  break;
               // Default case
               default:
                  return 0;
            }
         }
         if (isValid == 1)
         {
            bool Finalstop = false;
            do
            {
               try { temp_pop = MStack.Pop(); } catch (InvalidOperationException) { temp_pop = ""; }
               switch (temp_pop)
               {
                  case "*":
                  case "/":
                  case "+":
                  case "-":
                     output += temp_pop;
                     Finalstop = true;
                     break;
                  case "":
                     isValid = 1;
                     Finalstop = false;
                     break;
                  default:
                     return 0;
               }
            } while (Finalstop);
         }
         if (isValid == 1)
            parsedEquation = output;
         return isValid;
      }
   }
}
