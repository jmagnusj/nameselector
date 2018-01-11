using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.XPath;

namespace NameSelector
{
    public partial class MainWindow : Window
    {
        private static string XMLFileLocationString { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            LoadDefaultSettingsFromConfigurationFile();

            LoadYearsFromNamesXMLFile();
        }

        /*
         * Method LoadDefaultSettingsFromConfigurationFile loads the application's default settings from 
         * the application configuration file (App.config).
         */
        private void LoadDefaultSettingsFromConfigurationFile()
        {
            XMLFileLocationString = ConfigurationManager.AppSettings.Get("DefaultXMLLocation");

            //Added some trivial changes.
        }

        /*
         * Method LoadYearsFromNamesXMLFile handles the filling of the GUI years ComboBox with years 
         * from the names XML file.
         */
        private void LoadYearsFromNamesXMLFile()
        {
            List<int> yearsList = new List<int>();

            PopulateYearsList(yearsList);

            LoadYearsComboBox(yearsList);
        }

        /*
         * Method PopulateYearsList goes through the XML file and adds to the yearsList each year found.
         * 
         * Input:   List<int> (a list of years, initially empty)
         * 
         * Output:  void
         *          The years list will have been changed and should now include the years found in the 
         *          XML file. If adding the years failed at any point, the years list should be empty.
         */
        private void PopulateYearsList(List<int> iYearsList)
        {
            XPathDocument namesXPathDocument;

            try
            {
                namesXPathDocument = new XPathDocument(XMLFileLocationString);
                XPathNavigator navigator = namesXPathDocument.CreateNavigator();

                XPathNodeIterator nodes = navigator.Select("/names/name/yearstats/year");                

                int thisYear;

                foreach (XPathNavigator item in nodes)
                {
                    thisYear = Convert.ToInt32(item.Value);
                    if (!iYearsList.Contains(thisYear))
                    {
                        iYearsList.Add(thisYear);
                    }
                }
            }
            catch
            {
                iYearsList.Clear();
            }
        }

        /*
         * Method LoadYearsComboBox loads the GUI years ComboBox with the years present in the list of years.
         * 
         * Input:   List<int> (the years list)
         * 
         * Output:  void
         *          The GUI years ComboBox will have been changed and now includes all the years 
         *          found in the XML file.
         */
        private void LoadYearsComboBox(List<int> iYearsList)
        {
            ComboBoxItem defaultItem = new ComboBoxItem { Content = "Any" };

            yearComboBox.Items.Add(defaultItem);

            iYearsList.Sort();

            foreach (int year in iYearsList)
            {
                ComboBoxItem newItem = new ComboBoxItem { Content = year.ToString() };

                yearComboBox.Items.Add(newItem);
            }
            yearComboBox.SelectedIndex = 0;
        }

        /*
         * Method ShowNamesButton_Click handles clicks on the "Show name(s)" GUI button.
         */
        private void ShowNamesButton_Click(object sender, RoutedEventArgs e)
        {
            int numberOfNamesToShow = GetNumberOfNamesToShow();

            if(numberOfNamesToShow > 0)
            {
                char genderChoice = GetGenderChoice();
                int yearChoice = GetYearChoice();

                showNamesTextBox.Text = GenerateNamesOutput(numberOfNamesToShow, genderChoice, yearChoice);
            }
        }

        /*
         * Method GetNumberOfNamesToShow returns the number of names selected in the GUI number 
         * of names ComboBox.
         * 
         * Output:  int (the selected number, 0 if no number selected)
         */
        private int GetNumberOfNamesToShow()
        {
            if (numberOfNamesComboBox.SelectedItem != null)
            {
                ComboBoxItem selectedNumberOfNames = (ComboBoxItem)numberOfNamesComboBox.SelectedItem;
                return Convert.ToInt32(selectedNumberOfNames.Content);
            }
            return 0;
        }

        /*
         * Method GetGenderChoice returns the gender of the names to be shown in the output text box.
         * 
         * Output:  char ('B' for both female and male, 'F' for female only, 'M' for male only)
         */
        private char GetGenderChoice()
        {
            if (genderComboBox.SelectedItem != null)
            {
                ComboBoxItem selectedGender = (ComboBoxItem)genderComboBox.SelectedItem;
                if ((string)selectedGender.Content == "Female only")
                {
                    return 'F';
                }
                else if ((string)selectedGender.Content == "Male only")
                {
                    return 'M';
                }
            }
            return 'B'; // default; will return this if "Any" is chosen or no ComboBox item is selected
        }

        /*
         * Method GetYearChoice() returns the year selected in the GUI gender ComboBox.
         * 
         * Output:  int (the selected year; Int32.MaxValue if "Any" or no ComboBox item is selected)
         */
        private int GetYearChoice()
        {
            if (yearComboBox.SelectedItem != null)
            {
                ComboBoxItem selectedYear = (ComboBoxItem)yearComboBox.SelectedItem;
                if ((string)selectedYear.Content != "Any")
                {
                    return Convert.ToInt32(selectedYear.Content);
                }
            }
            return Int32.MaxValue;
        }

        /*
         * Method GenerateNamesOutput generates a string to be shown in the GUI output TextBox.
         */
        private string GenerateNamesOutput(int numberOfNamesToShow, char genderChoice, int yearChoice)
        {
            int defaultNameOutputLength = 10; // Based on assumption about the average length of names, with some margin
            StringBuilder namesOutput = new StringBuilder(numberOfNamesToShow * defaultNameOutputLength);

            Random randomNumber = new Random();

            try
            {
                XPathDocument namesXPathDocument = new XPathDocument(XMLFileLocationString);
                XPathNavigator namesFileXPathNavigator = namesXPathDocument.CreateNavigator();
                XPathNodeIterator nodes = namesFileXPathNavigator.Select(CreateXPathSelectionString(genderChoice, yearChoice));

                for (int i = 0; i < numberOfNamesToShow; i++)
                {
                    if (i > 0)
                    {
                        namesOutput.Append("\n");
                    }
                    //namesOutput.Append($"Name #{ i + 1 }: ");
                    namesOutput.Append(GetNameFromXPathNodeIterator(randomNumber, nodes));
                }
            }
            catch (Exception e)
            {
                namesOutput.Append(e.Message);
            }
            return namesOutput.ToString();            
        }

        /*
         * Method GetNameFromXPathNodeIterator gets the name at one randomly selected node.
         */
        private string GetNameFromXPathNodeIterator(Random iRandom, XPathNodeIterator iXPNodeIterator)
        {
            int thisRandomNumber = iRandom.Next(0, iXPNodeIterator.Count);

            int numberOfNamesIterated = 0;

            foreach (XPathNavigator item in iXPNodeIterator)
            {
                if (numberOfNamesIterated == thisRandomNumber)
                {
                    return item.Value;
                }
                numberOfNamesIterated++;
            }

            return ""; // Default, should only be reached if empty set of nodes.
        }

        /*
         * Method ClearButton_Click handles clicks on the "Clear" GUI button. Removes all text in GUI TextBox.
         */
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            showNamesTextBox.Clear();
        }

        /*
         * Method ExitButton_Click handles clicks on the "Exit" GUI button. Closes the application.
         */
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*
         * Method CreateXPathSelectionString creates an XPath selection string based on gender and year.
         * There are four possibilities, each handled as a separate case:
         * - gender not specified, year not specified
         * - gender not specified, year specified
         * - gender specified, year not specified
         * - gender specified, year specified
         */
        private string CreateXPathSelectionString(char iGender, int iYear)
        {
            // gender not specified, year not specified
            if (iGender == 'B' && iYear == Int32.MaxValue)
            {
                return "/names/name/givenname";
            }

            // gender not specified, year specified
            if (iGender == 'B' && iYear != Int32.MaxValue)
            {
                return $"/names/name[yearstats/year='{ iYear }']/givenname";
            }

            // gender specified, year not specified
            if (iGender != 'B' && iYear == Int32.MaxValue)
            {
                return $"/names/name[gender='{ iGender }']/givenname";
            }

            // gender specified, year specified
            return $"/names/name[gender='{ iGender }' and yearstats/year='{ iYear }']/givenname";
        }        
    }
}
