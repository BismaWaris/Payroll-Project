using NUnit.Framework;
using OO_programming;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TestProject
{

    //Data condition testing OR unit testing
    public class PayrollCalculatorTestsWithTaxApplied
    {
        private PayCalculator _payCalculator;

        // Arrange
        public double a = 0.3477, b = 64.4297, hourlyRate = 25, hoursWorked = 12;

        [SetUp]
        public void Setup()
        {
            //Initializing Pay Calulator before each test
            _payCalculator = new PayCalculator(a, b, hourlyRate, hoursWorked);

        }

        [Test]
        public void CalculateTax()
        {
            // Act
            double tax = _payCalculator.CalculateTax();

            // Assert

            double expected = 39.880300;
            double delta = 0.0001;
            Assert.AreEqual(expected, tax, delta);


        }

        [Test]
        public void CalculateGrossPay()
        {


            // Act
            double grossPay = _payCalculator.CalculateGrossPay();

            // Assert
            Assert.AreEqual(300, grossPay);

        }

        [Test]
        public void CalculateNetPay()
        {

            // Act
            double netPay = _payCalculator.CalculateNetPay();

            // Assert
            //Assert.AreEqual(260.119699, netPay);

            double expected = 260.119699;
            double delta = 0.0001;  // Define a small tolerance
            Assert.AreEqual(expected, netPay, delta);


        }

        [Test]
        public void CalculateSuperAnnuation()
        {

            // Act
            double super = _payCalculator.CalculateSuperannuation();

            // Assert
            Assert.AreEqual(33, super);


        }


    }

    public class PayrollCalculatorTestsWithTaxNotApplied
    {

        private PayCalculatorWithTaxFreeThresholdClaimed _payCalculatorWithTaxFreeThresholdClaimed;

        //ARRANGE
        public double a = 0.2100, b = 68.3465, hourlyRate = 25, hoursWorked = 24;


        [SetUp]
        public void Setup()
        {
            //Initializing Pay Calulator before each test
            _payCalculatorWithTaxFreeThresholdClaimed = new PayCalculatorWithTaxFreeThresholdClaimed(a, b, hourlyRate, hoursWorked);

        }

        [Test]
        public void CalculateTax()
        {
            // Act
            double tax = _payCalculatorWithTaxFreeThresholdClaimed.CalculateTax();

            // Assert

            double expected = 57.6534;
            double delta = 0.0001;
            Assert.AreEqual(expected, tax, delta);


        }

        [Test]
        public void CalculateGrossPay()
        {


            // Act
            double grossPay = _payCalculatorWithTaxFreeThresholdClaimed.CalculateGrossPay();

            // Assert
            Assert.AreEqual(600, grossPay);

        }

        [Test]
        public void CalculateNetPay()
        {

            // Act
            double netPay = _payCalculatorWithTaxFreeThresholdClaimed.CalculateNetPay();

            // Assert
            //Assert.AreEqual(260.119699, netPay);

            double expected = 542.3466;
            double delta = 0.0001;  // Define a small tolerance
            Assert.AreEqual(expected, netPay, delta);


        }

        [Test]
        public void CalculateSuperAnnuation()
        {

            // Act
            double super = _payCalculatorWithTaxFreeThresholdClaimed.CalculateSuperannuation();

            // Assert
            Assert.AreEqual(66, super);


        }
    }

    //Use case Testing OR Integration testing
    public class TestReadingAndWritingEmployeeDataFromCSV
    {
       // Arrange

       private Form1 _form;

       [SetUp]
       public void Setup()
       {
                _form = new Form1(); //Initializing Form1 before each test
       }

       [Test]

        public void CountCSVEntries()
        {

                // Act
                _form.PopulateEmployeeListBox();

                //Assert
                int expectedCount = 7;

                Assert.AreEqual(expectedCount, _form.paySlips.Count);

        }

        [Test]
        public void ReadCSVData()
        {

                // Arrange
                int expectedEmployeeID = 1;
                string expectedFirstName = "Marge";
                string expectedLastName = "Larkin";
                string expectedEmployeeType = "Employee";
                double expectedHourlyRate = 25;


                // Act
                _form.PopulateEmployeeListBox();

                // Assert

                Assert.AreEqual(expectedEmployeeID, _form.paySlips[0].employeeID);
                Assert.AreEqual(expectedFirstName, _form.paySlips[0].firstName);
                Assert.AreEqual(expectedLastName, _form.paySlips[0].lastName);
                Assert.AreEqual(expectedEmployeeType, _form.paySlips[0].employeeType);
                Assert.AreEqual(expectedHourlyRate, _form.paySlips[0].hourlyRate);



        }


            [Test]
            public void WriteOutputData()
            {


                //Arrange

                string expectedOutputFilePath = "expected_output.csv";
                string actualOutputFilePath = "actual_output.csv";

                // ACT
                _form.SavePayDataToCSV();

                // Assert

                Assert.IsTrue(File.Exists(actualOutputFilePath));


                CompareContentsOfCsvFiles(expectedOutputFilePath, actualOutputFilePath);



            }

            //Read Contents of both actual and expected output files
            public void CompareContentsOfCsvFiles(string expectedOutputFilePath, string actualOutputfilePath)
            {
                var expectedContent = File.ReadAllText(expectedOutputFilePath);
                var actualContent = File.ReadAllText(actualOutputfilePath);

                Assert.AreEqual(expectedContent, actualContent, "CSV file contents do not match.");
            }


           


    }

    //UI Testing - Testing if the ListBox1 is populated correctly 

    public class UITesting
    {
        private Form1 _form;
       
        [Test]
        public void TestListBoxPopulation()
        {
            // Arrange

           

           _form = new Form1();

            string expectedFirstEntry = "1, Marge, Larkin";



            // Act
             _form.PopulateEmployeeListBox();


            string actualFirstEntry = _form.listBox1.Items[0].ToString();


             // Assert
              Assert.AreEqual(expectedFirstEntry, actualFirstEntry);





        } 
   }
}

        






