using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Equations
{
   class Equation
   {
      private static string equation = string.Empty;
      private static string parsedEquation = string.Empty;

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

//*********************************************************
//                Parse a statement
//*********************************************************
      public static string ParseStatement(string input)
      {
         Stack<string> MStack = new Stack<string>();
         string output = string.Empty;
         string temp_pop;
         int isValid = 0;
         Regex rgx = new Regex(@"([\(\)\*\+\-\/])|[A-Za-z]|(\d+(?:\.\d+)?)");
         foreach (Match m in rgx.Matches(input))
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
                           return "Invalid equation format";
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
                           return "Invalid equation format";
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
                           isValid = 0;
                           break;
                        default:
                           return "";
                     }
                  } while (RPStop);
                  break;
               // Opening Parenthesis
               case "(":
                  isValid = 0;
                  MStack.Push("(");
                  break;
               // Some number
               case string someNum when new Regex(@"[A-Za-z]|(\d+(?:\.\d+)?)").IsMatch(someNum.ToString()):
                  isValid = 1;
                  output += someNum;
                  break;
               // Space
               case "":
                  break;
               // Default case
               default:
                  return "Invalid equation format";
            }
            output += " ";
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
                     return "Invalid equation format";
               }
            } while (Finalstop);
         }

         return output;
      }

//*********************************************************
//                Solve a statement
//*********************************************************
      public static string Solve(string input)
      {
         Stack<string> PDA = new Stack<string>();
         Regex rgx = new Regex(@"([\(\)\*\+\-\/])|[A-Za-z]|(\d+(?:\.\d+)?)");
         string right, left = string.Empty;
         double dLeft = 0;
         double dRight = 0;
         foreach (Match m in rgx.Matches(input))
         {
            switch (m.Value)
            {
               // Some Number
               case string someNum when new Regex(@"[A-Za-z]|(\d+(?:\.\d+)?)").IsMatch(someNum.ToString()):
                  PDA.Push(someNum);
                  break;
               // Addition
               case "+":
                  right = PDA.Pop();
                  left = PDA.Pop();
                  if (Double.TryParse(left, out dLeft) && Double.TryParse(right, out dRight))
                     PDA.Push((dLeft + dRight).ToString());
                  else if (left.Contains("+") || left.Contains("-"))
                  {
                     PDA.Push(solveMidStatement(right, left, "+", "l"));
                  }
                  else
                  {
                     PDA.Push(string.Format("{0} {1} +", left, right));
                  }
                  break;
               // Subtraction
               case "-":
                  right = PDA.Pop();
                  left = PDA.Pop();
                  if (Double.TryParse(left, out dLeft) && Double.TryParse(right, out dRight))
                     PDA.Push((dLeft - dRight).ToString());
                  else if (left.Contains("+") || left.Contains("-"))
                  {
                     PDA.Push(solveMidStatement(right, left, "-", "l"));
                  }
                  else
                  {
                     PDA.Push(string.Format("{0} {1} -", left, right));
                  }
                  break;
               // Multiplication
               case "*":
                  right = PDA.Pop();
                  left = PDA.Pop();
                  if (Double.TryParse(left, out dLeft) && Double.TryParse(right, out dRight))
                     PDA.Push((dLeft * dRight).ToString());
                  else if (right.Contains("+") || right.Contains("-"))
                  {
                     PDA.Push(solveMidStatement(right, left, "*", "r"));
                  }
                  else
                  {
                     PDA.Push(string.Format("{0} {1} *", left, right));
                  }
                  break;
               // Division
               case "/":
                  right = PDA.Pop();
                  left = PDA.Pop();
                  if (Double.TryParse(left, out dLeft) && Double.TryParse(right, out dRight))
                     PDA.Push((dLeft / dRight).ToString());
                  else if (right.Contains("+") || right.Contains("-"))
                  {
                     PDA.Push(solveMidStatement(right, left, "/", "r"));
                  }
                  else
                  {
                     PDA.Push(string.Format("{0} {1} /", left, right));
                  }
                  break;
            }
            Debug.WriteLine(String.Join("|", PDA.ToArray()));
         }
         return PDA.Pop();
      }

//*********************************************************
//                Solve an equation
//*********************************************************
      public static double Solve(string left, string right)
      {
         Stack<double> PDA = new Stack<double>();
         Regex rgx = new Regex(@"([\(\)\*\+\-\/])|[A-Za-z]|(\d+(?:\.\d+)?)");
         double temp = 0;
         foreach (Match m in rgx.Matches(left))
         {
            switch (m.Value)
            {
               // Some Number
               case string someNum when new Regex(@"(\d+(?:\.\d+)?)").IsMatch(someNum.ToString()):
                  PDA.Push(double.Parse(someNum));
                  break;
               // Addition
               case "+":
                  temp = PDA.Pop();
                  PDA.Push(PDA.Pop() + temp);
                  break;
               // Subtraction
               case "-":
                  temp = PDA.Pop();
                  PDA.Push(PDA.Pop() - temp);
                  break;
               // Multiplication
               case "*":
                  temp = PDA.Pop();
                  PDA.Push(PDA.Pop() * temp);
                  break;
               // Division
               case "/":
                  temp = PDA.Pop();
                  PDA.Push(PDA.Pop() / temp);
                  break;
            }
         }
         return PDA.Pop();
      }

      private static string solveMidStatement(string right, string left, string oper, string side)
      {
         string expression = string.Empty,
                temp = string.Empty,
                rgx = @"((\d+(?:\.\d+)?) [A-Za-z] [*/])|([A-Za-z] (\d+(?:\.\d+)?) [*/])|[A-Za-z]";
         double dLeft = 0,
                dRight = 0;
         switch()
         {
            case "l":
               dRight = double.Parse(right);
               switch (oper)
               {
                  case "+":
                  case "-":
                     temp = Regex.Match(left, rgx).Value;
                     Debug.WriteLine("ss");
                     Debug.WriteLine(temp);
                     expression = Regex.Replace(left, rgx, "~");
                     if(expression.StartsWith("~"))
                     {
                        dLeft = double.Parse((expression.Split(' ')[2] == "-" ? "-" : "") + expression.Split(' ')[1]);
                        expression = temp + " " + (oper == "-" ? string.Format("{0}", dLeft - dRight) : string.Format("{0}", dLeft + dRight)) + " " + expression.Split(' ')[2];
                     }
                     else
                     {
                        dLeft = double.Parse(expression.Split(' ')[0]);
                        Debug.WriteLine(dLeft);
                        expression = (oper == "-" ? string.Format("{0}", dLeft - dRight) : string.Format("{0}", dLeft + dRight)) + " " + temp + " " + expression.Split(' ')[2];
                     }
                     break;
                  case "*":
                  case "/":
                     expression = right.Replace(" ", " " + left + " " + oper);
                     break;
               }
               break;
            case "r":
               dLeft = double.Parse(left);
               switch (oper)
               {
                  case "+":
                  case "-":
                     temp = Regex.Match(right, rgx).Value;
                     Debug.WriteLine("ss");
                     Debug.WriteLine(temp);
                     expression = Regex.Replace(right, rgx, "~");
                     if (expression.StartsWith("~"))
                     {
                        dRight = double.Parse((expression.Split(' ')[2] == "-" ? "-" : "") + expression.Split(' ')[1]);
                        expression = temp + " " + (oper == "-" ? string.Format("{0}", dLeft - dRight) : string.Format("{0}", dLeft + dRight)) + " " + expression.Split(' ')[2];
                     }
                     else
                     {
                        dRight = double.Parse(expression.Split(' ')[0]);
                        Debug.WriteLine(dLeft);
                        expression = (oper == "-" ? string.Format("{0}", dLeft - dRight) : string.Format("{0}", dLeft + dRight)) + " " + temp + " " + expression.Split(' ')[2];
                     }
                     break;
                  case "*":
                  case "/":
                     string tLeft  = left + " " + right.Split(' ')[0] + " " + oper,
                            tRight = left + " " + right.Split(' ')[1] + " " + oper;
                     
                     break;
               }
               break;
         }
         Debug.WriteLine(expression);
         Debug.WriteLine("ee");
         return expression;
      }
   }
}
