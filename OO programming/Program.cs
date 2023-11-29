using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OO_programming
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }


    /// <summary>
    /// Class capture details accociated with an employee's pay slip record
    /// This class represents the data structure of the CSV file
    /// </summary>
    public class PaySlip
    {
        public int employeeID { get; set; }
        public string firstName{ get; set; }
        public string lastName{ get; set; }
        public double hoursWorked { get; set; }
        public string employeeType { get; set; }
        public double hourlyRate { get; set; }
        public bool taxThreshold { get; set; } //true if the tax-free threshold is claimed, false otherwise

        public string EmployeeDetails => $"{employeeID} - {firstName} {lastName}";
    }

    /// <summary>
    /// Base class to hold all Pay calculation functions
    /// Default class behaviour is tax calculated with tax threshold applied
    /// </summary>
    

    public class PayCalculator
    {
        protected double a, b, hourlyRate, hoursWorked;

        public PayCalculator(double a, double b, double hourlyRate, double hoursWorked)
        {
            this.a = a;
            this.b = b;
            this.hourlyRate = hourlyRate;
            this.hoursWorked = hoursWorked;
        }

        public virtual double CalculateTax()
        {
            return a * hourlyRate * hoursWorked - b;
        }

        public double CalculateGrossPay()
        {
            return hourlyRate * hoursWorked;
        }

        public double CalculateNetPay()
        {
            return CalculateGrossPay() - CalculateTax();
        }

        public double CalculateSuperannuation()
        {
            return CalculateGrossPay() * 0.11; // Assuming 11% super rate
        }
    }

    /// <summary>
    /// Extends PayCalculator class handling No tax threshold
    /// </summary>
    //public class PayCalculatorNoThreshold : PayCalculator
    //{
       
    //}

   
    public class PayCalculatorWithTaxFreeThresholdClaimed : PayCalculator
    {
        public PayCalculatorWithTaxFreeThresholdClaimed(double a, double b, double hourlyRate, double hoursWorked)
            : base(a, b, hourlyRate, hoursWorked) { }
    }
}