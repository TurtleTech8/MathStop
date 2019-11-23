﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathStop
{
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }
      void Append_Input(object sender, RoutedEventArgs e)
      {
         Input.AppendText((String)((Button)sender).Tag);
         Input.Focus();
         if (Input.Text.Contains("("))
         {
            Input.SelectionStart = Input.Text.Length - 1;
         }
      }
      void Calculate_Equation(object sender, RoutedEventArgs e)
      {
         string eq = Input.Text;
         MathStop.Equation newEquation = new MathStop.Equation();

         newEquation.AddEquation(eq);
         if (newEquation.ParseEquation() == 1)
         {
            Output.Text = newEquation.GetParsedEquation();
         }
         else
         {
            Output.Text = "Invalid equation format";
         }
         //string equation = Input.Text;
         //foreach(char ch in equation)
         //{
         //   if(char.IsDigit(ch))
         //   {

         //   }
         //   else
         //   {
         //      //IdentifySymbol()
         //   }
         //}
      }
   }
}
