﻿using KIT206_RAP.Entites;
using KIT206_RAP.Controll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RAP.Controll;
using RAP.Entities;
using System.Linq.Expressions;



// This is a comment, welcom to the word of git
namespace RAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Publication> selectedResearcherPublications;
        private ObservableCollection<Researcher> researchers;
        private List<Publication> publicaitonList;
        private List<Researcher> resList;

        public BitmapImage ImageData { get; set; }


        // select a researcher
        private void researcherListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (researcherListView.SelectedItem != null)
            {
                Researcher selectedResearcher = (Researcher)researcherListView.SelectedItem;
                ResearcherControl.DisplayResearcherDetails(selectedResearcher, resList);
                selectedResearcher.Pubs.Clear();
                publicaitonList = PublicationsControl.FetchPublications(selectedResearcher);

                selectedResearcherPublications.Clear();

                foreach (var publication in publicaitonList)
                {
                    selectedResearcherPublications.Add(publication);
                }

                // researcher details
                name.Text = "Name: " + selectedResearcher.FirstName + " " + selectedResearcher.LastName;
                //title not displaying at all, mustn't be pulling from db?
                resTitle.Text = "Title: " + selectedResearcher.Title;
                unit.Text = "Unit: " + selectedResearcher.SchoolUnit;
                Enum campusPr = selectedResearcher.Camp;
                campus.Text = "Campus: " + campusPr.ToString();
                email.Text = "Email: " + selectedResearcher.Email;
                //current job title not working, not sure its calculating?
                job.Text = "Job: " + selectedResearcher.Job_Title;
                
                commencedInt.Text = "Commenced with institution: " + selectedResearcher.CommencedWithInstitution.ToString("d");
                commencedCurr.Text = "Commenced current job: " + selectedResearcher.CommenceCurrentPosition.ToString("d");

                prevPos.Text = "Previous positions: " + selectedResearcher;
                tenure.Text = "Tenure: ";

                publi.Text = "Publications: " + selectedResearcher.Pubs.Count;
                if (selectedResearcher is Student student)
                {
                    degree.Text = "Degree: " + student.Degree;
                    supervisions.Text = "Supervisions: N/A";
                    supervisor.Text = "Supervisor: " + student.Supervisor;
                }
                else if (selectedResearcher is Staff staff)
                {
                    supervisor.Text = "Supervisor: N/A";
                    threeYearAvg.Text = "3-year-average: " + staff.ThreeYearAverage;
                    performanceFund.Text = "Performance by Funding: " + "$" + (Math.Round(staff.FundingRecieved / ((DateTime.Now - staff.CommencedWithInstitution).TotalDays / 365), 1)).ToString();
                    performancePub.Text = "Performance by Publication: " + String.Format("{0:0.0}", Math.Round(staff.ThreeYearAverage / staff.ExpectedNoPubs * 100, 1) + "%");
                    supervisions.Text = "Supervisions: " + staff.Supervisions.Count; 
                }

                ImageData = new BitmapImage(new Uri(selectedResearcher.PhotoURL));


                Console.WriteLine("image URL " + selectedResearcher.PhotoURL);

                Console.WriteLine("Selected researcher: " + selectedResearcher.FirstName);
            }
        }
        private void tempButton_Click(object sender, RoutedEventArgs e)
        {
            Researcher selectedResearcher = (Researcher)researcherListView.SelectedItem;
            if (selectedResearcher != null)
            {
                PerformanceDetailsWindow PdetailsView = new PerformanceDetailsWindow();
                ResearcherControl.ControllTheDeetails(selectedResearcher);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(selectedResearcher.PhotoURL);
                image.EndInit();
                PdetailsView.profilePic.Source = image;

                PdetailsView.DataContext = selectedResearcher;
                PdetailsView.Show();
            }
            else
            {
                MessageBox.Show("No researcher selected");
            }
        }

        private void PublicationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PublicationListView.SelectedItem != null)
            {
                Publication selectedPublication = (Publication)PublicationListView.SelectedItem;
                DateTime now = DateTime.Now;

                
                DOI.Text = "Title : " + selectedPublication.DOI;
                pubTitle.Text = "Publication Title: " + selectedPublication.Title;
                authors.Text = "Authors: " + selectedPublication.Authors;
                //pubYear.Text = "Publication Year: " + selectedPublication.year;
                ranking.Text = "Ranking: " + selectedPublication.Ranking;
                pubType.Text = "Publication Type: " + selectedPublication.Type;
                citeAS.Text = "Cite As: " + selectedPublication.CiteAs;
                avaDate.Text = "Availability Date: " + selectedPublication.AvailabilityDate;

                pubAge.Text = "Publication Age: " + selectedPublication.Age;
             }

        }

        public MainWindow()
        {
            resList = ResearcherControl.FetchResearchers();
            researchers= new ObservableCollection<Researcher>(resList);
            selectedResearcherPublications = new ObservableCollection<Publication>();
            InitializeComponent();
            researcherListView.ItemsSource = researchers;
            PublicationListView.ItemsSource = selectedResearcherPublications;
            
        }
        private void LevelFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            string selectedLevel = selectedItem.Content.ToString();

            researcherListView.ItemsSource = ResearcherControl.FilterLevel(selectedLevel, researchers);
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                researcherListView.ItemsSource =  ResearcherControl.FilterList(researchers, SearchBox.Text);
            }
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(FirstNumberTextBox.Text, out int firstNumber) &&
                    int.TryParse(SecondNumberTextBox.Text, out int secondNumber))
            {
                publicaitonList = PublicationsControl.FilterByYear(firstNumber, secondNumber, publicaitonList);

                selectedResearcherPublications.Clear();
                foreach (var publication in publicaitonList)
                {
                    selectedResearcherPublications.Add(publication);
                }
            }
            else
            {
                Console.WriteLine("Please enter valid integers.");
            }
        }

        private void PublicationDateColumnHeader_Click(object sender, RoutedEventArgs e)
        {

                publicaitonList = PublicationsControl.invert_sort(publicaitonList);

                selectedResearcherPublications.Clear();
                foreach (var publication in publicaitonList)
                {
                    selectedResearcherPublications.Add(publication);
                }
        }

        private void Button_Copy_Email_Click(object sender, RoutedEventArgs e)
        {
            string copiedEmail;
            Researcher selectedResearcher = (Researcher)researcherListView.SelectedItem;
            if(selectedResearcher != null){
                copiedEmail = selectedResearcher.Email;
                Clipboard.SetText(copiedEmail);
                MessageBox.Show("Text copied to clipboard." + copiedEmail);
            }
            else
            {
                MessageBox.Show("No researcher selected");
            }
        }

        private void Report_Button_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Researcher> researchers = new ObservableCollection<Researcher>(ResearcherControl.FetchResearchers());
            ReportsView reports = new ReportsView(researchers);
            reports.Show();

        }
    }
}
