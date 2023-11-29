using System;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OO_programming
{
    public partial class Form1 : Form
    {
        
        //declared a variable of type PaySlip
        public List<PaySlip> paySlips; 
        public Form1()
        {
            InitializeComponent();
           

            // Add code below to complete the implementation to populate the listBox
            // by reading the employee.csv file into a List of PaySlip objects, then binding this to the ListBox.
            // CSV file format: <employee ID>, <first name>, <last name>, <hourly rate>,<taxthreshold>

            PopulateEmployeeListBox();

            //SavePayDataToCSV();

        }

        public void PopulateEmployeeListBox()
        {
            // Assuming employee.csv is in the format: employeeID, firstName, lastName, hourlyRate, taxThreshold

            //Change the file to 'employee_test' when testing the project
            
            using (var reader = new StreamReader("employee.csv"))   //employee_test.csv when testing the project
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                paySlips = new List<PaySlip>();
                while (csv.Read())
                {
                    var paySlip = new PaySlip
                    {
                        employeeID = csv.GetField<int>(0),
                        firstName = csv.GetField<string>(1),
                        lastName = csv.GetField<string>(2),
                        employeeType = csv.GetField<string>(3),
                        hourlyRate = csv.GetField<double>(4),
                        taxThreshold = csv.GetField<string>(5).ToLower() == "y"
                    };
                    paySlips.Add(paySlip);
                }
            }

            foreach (var paySlip in paySlips)
            {
                listBox1.Items.Add($"{paySlip.employeeID}, {paySlip.firstName}, {paySlip.lastName}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Add code below to complete the implementation to populate the
            // payment summary (textBox2) using the PaySlip and PayCalculatorNoThreshold
            // and PayCalculatorWithThresholds classes object and methods.

            // Retrieve and validate user input for hours worked
            if (!double.TryParse(textBox1.Text, out double hoursWorked) || hoursWorked <= 0)
            {
                MessageBox.Show("Please enter a valid number for hours worked.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var selectedPaySlip = paySlips[listBox1.SelectedIndex];

            double weeklyEarnings = selectedPaySlip.hourlyRate * hoursWorked;

            bool isTaxFreeThresholdClaimed = selectedPaySlip.taxThreshold; 

            var coefficients = GetTaxCoefficients(weeklyEarnings, isTaxFreeThresholdClaimed);

            var payCalculator = new PayCalculatorWithTaxFreeThresholdClaimed(coefficients.a, coefficients.b, selectedPaySlip.hourlyRate, hoursWorked);
            textBox2.Text = $"Employee ID: {selectedPaySlip.employeeID}\n" +
                            $"Name: {selectedPaySlip.firstName} {selectedPaySlip.lastName}\n" +
                            $"Hours Worked: {hoursWorked}\n" +
                            $"Hourly Rate: {selectedPaySlip.hourlyRate}\n" +
                            $"Tax Threshold: {selectedPaySlip.taxThreshold}\n" +
                            $"Gross Pay: {payCalculator.CalculateGrossPay()}\n" +
                            $"Tax: {payCalculator.CalculateTax()}\n" +
                            $"Net Pay: {payCalculator.CalculateNetPay()}\n" +
                            $"Superannuation: {payCalculator.CalculateSuperannuation()}";
        }






        // Add code below to complete the implementation for saving the
        // calculated payment data into a csv file.
        // File naming convention: Pay_<full name>_<datetimenow>.csv
        // Data fields expected - EmployeeId, Full Name, Hours Worked, Hourly Rate, Tax Threshold, Gross Pay, Tax, Net Pay, Superannuation

        /// <summary>
        /// Method for generating payslip and saving it in Desktop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button2_Click(object sender, EventArgs e)
        {
            // Ensure a PaySlip is selected from the listBox
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an employee first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedPaySlip = paySlips[listBox1.SelectedIndex];

            
            // Ensure hours worked is parsed and used in calculations
             
            if (!double.TryParse(textBox1.Text, out double hoursWorked) || hoursWorked <= 0)
            {
                MessageBox.Show("Please enter a valid number for hours worked.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isTaxFreeThresholdClaimed = selectedPaySlip.taxThreshold;


            double weeklyEarnings = selectedPaySlip.hourlyRate * hoursWorked;

            var coefficients = GetTaxCoefficients(weeklyEarnings, isTaxFreeThresholdClaimed);

            var payCalculator = new PayCalculatorWithTaxFreeThresholdClaimed(coefficients.a, coefficients.b, selectedPaySlip.hourlyRate, hoursWorked);

            // Construct the filename
            string filename = $"Pay_{selectedPaySlip.firstName}_{selectedPaySlip.lastName}_{DateTime.Now:yyyyMMddHHmmss}.csv";

            // Construct the full path
            string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);

            // Prepare the data to be saved
            var data = new
            {
                EmployeeId = selectedPaySlip.employeeID,
                FullName = $"{selectedPaySlip.firstName} {selectedPaySlip.lastName}",
                HoursWorked = hoursWorked,
                HourlyRate = selectedPaySlip.hourlyRate,
                TaxThreshold = selectedPaySlip.taxThreshold,
                GrossPay = payCalculator.CalculateGrossPay(),
                Tax = payCalculator.CalculateTax(),
                NetPay = payCalculator.CalculateNetPay(),
                Superannuation = payCalculator.CalculateSuperannuation()
            };

            // Save/Write the data to a CSV file
            try
            {
                using (var writer = new StreamWriter(filepath))
                using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecord(data);
                }

                // Notify the user
                MessageBox.Show($"Payment data saved to {filename} on your Desktop.", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        /// <summary>
        /// This class 'EmployeeData' is created for testing purposes; to hard-code the objects of the payslip to test whether CSVHelper is saving the file or not
        /// </summary>

        //public class EmployeeData
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public double HoursWorked { get; set; }
        //    public double HourlyRate { get; set; }
        //    public double Tax { get; set; }
        //    public double GrossPay { get; set; }
        //    public double NetPay { get; set; }
        //    public double Superannuation { get; set; }
        //}

        /// <summary>
        /// Method to save test data to CSV for testing purposes
        /// </summary>
        //public void SavePayDataToCSV()
        //{
            // Construct the filename
            //string filename = $"Pay_{selectedPaySlip.firstName}_{selectedPaySlip.lastName}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            
            // Construct the full path
            //string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "actual_output.csv");

            //string folderPath = @"C:\Users\sma04\source\repos\Cl_OOProgramming_AE_Pro_Appx\Part 3 application files";
            //string fileName = "actual_output.csv";
            //string filepath = Path.Combine(folderPath, fileName);

            //hard-coding the values 
            //var employees = new List<EmployeeData>
            //{
            //    new EmployeeData
            //    {
            //       EmployeeId = 1,
            //       FullName = "Sara Khan",
            //       HoursWorked = 40,
            //       HourlyRate= 25,
            //       Tax=200,
            //       GrossPay= 1000,
            //       NetPay = 800,
            //       Superannuation = 100

            //    },
            //};
            
            //try
            //{
            //    using (var writer = new StreamWriter(filepath))
        //        using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
        //        {
        //            csv.WriteRecords(employees);
        //        }

        //        // Notify the user
        //        MessageBox.Show($"Payment data saved.", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }   
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"An error occurred while saving the data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //}

        /// <summary>
        /// Method for obtaining the values of coefficients 'a' and 'b'
        /// </summary>
        /// <param name="weeklyEarnings"> The amount earned in a week </param>
        /// <param name="isTaxFreeThresholdClaimed"> Returns a boolean value of True or False  </param>
        /// <returns></returns>

        public (double a, double b) GetTaxCoefficients(double weeklyEarnings, bool isTaxFreeThresholdClaimed)
        {
            double a, b;

            if (isTaxFreeThresholdClaimed)
            {
                // Schedule 2: Employee has claimed the tax-free threshold
                if (weeklyEarnings < 359) { a = 0; b = 0; }
                else if (weeklyEarnings < 438) { a = 0.1900; b = 68.3462; }
                else if (weeklyEarnings < 548) { a = 0.2900; b = 112.1942; }
                else if (weeklyEarnings < 721) { a = 0.2100; b = 68.3465; }
                else if (weeklyEarnings < 865) { a = 0.2190; b = 74.8369; }
                else if (weeklyEarnings < 1282) { a = 0.3477; b = 186.2119; }
                else if (weeklyEarnings < 2307) { a = 0.3450; b = 182.7504; }
                else if (weeklyEarnings < 3461) { a = 0.3900; b = 286.5965; }
                
                else { a = 0.3900; b = 286.5965; } // Default to the highest range if no match
            }
            else
            {
                // Schedule 1: Tax-free threshold is not claimed by employee    
                if (weeklyEarnings < 88) { a = 0.1900; b = 0.1900; }
                else if (weeklyEarnings < 371) { a = 0.2348; b = 3.9639; }
                else if (weeklyEarnings < 515) { a = 0.2190; b = -1.9003; }
                else if (weeklyEarnings < 515) { a = 0.3477; b = 64.4297; }
                else if (weeklyEarnings < 932) { a = 0.2190; b = -1.9003; }
                else if (weeklyEarnings < 1957) { a = 0.3477; b = 64.4297; }
                else if (weeklyEarnings < 3461) { a = 0.3450; b = 61.9132; }
                else if (weeklyEarnings < 3461) { a = 0.4700; b = 563.5196; }
                
                
                else { a = 0.4700; b = 563.5196; } // Default to the highest range if no match
            }

            return (a, b);
        }

    }
}
