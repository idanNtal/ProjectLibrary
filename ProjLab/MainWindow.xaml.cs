using Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using MessageBox = System.Windows.MessageBox;

namespace ProjLab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public enum UI { Manager, Customer }
    
    public partial class MainWindow : Window
    {
        Library i;
        bool navigate = false;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Load();
            }
            catch (Exception)
            {
                Upload_DB();
            }
        }

        public MainWindow(Library i)
        {
            InitializeComponent();
            this.i = i;
        }


        #region Page Navigate
        private void btn_mng_Click(object sender, RoutedEventArgs e)
        {
            SecondWindow s1 = new SecondWindow(i, UI.Manager);
            s1.WindowState = this.WindowState;
            navigate = true;
            this.Close();
            s1.ShowDialog();
        }

        private void btn_register_login_Click(object sender, RoutedEventArgs e)
        {
            Customer tempCust = null;
            if (rdb_newCust.IsChecked == true)
            {
                try
                {
                    tempCust = i.MngCustList.Register(tb_userName.Text, tb_age.Text, tb_password.Password);
                    InboxMessage tempMessage = new InboxMessage("Library", $"Welcome {tempCust.Name}",
                        $"Congrats {tempCust.Name}, You have now Library Card !\n\nPlease keep the books in good condition.\n\nLibrary staff");
                    i.inbox.SendMessage(tempCust.inbox, tempMessage);
                    MessageBox.Show($"Hello {tempCust.Name},\nRegistration Completed Successfully");
                }
                catch (CustomerException CE)
                {
                    MessageBox.Show(CE.Message);
                }
            }
            if (rdb_oldCust.IsChecked == true)
            {
                try
                {
                    tempCust = i.MngCustList.LogIn(tb_userName.Text, tb_password.Password);
                }
                catch (CustomerException CE)
                {
                    MessageBox.Show(CE.Message);
                }
            }
            if (tempCust != null)
                MoveToCustomerwindow(tempCust);
        }

        private void MoveToCustomerwindow(Customer cust)
        {
            SecondWindow s1 = new SecondWindow(i,UI.Customer, cust);
            s1.WindowState = this.WindowState;
            navigate = true;
            this.Close();
            s1.ShowDialog();
        }
#endregion
        

        #region Login/Register Customer
        private void btn_cust_Click(object sender, RoutedEventArgs e)
        {
            btn_cust.Background = new SolidColorBrush(Colors.RoyalBlue);
            Storyboard sb = (Storyboard)this.FindResource("SlideIn");
            sb.Begin();

            RegistrationForm.Visibility = Visibility.Visible;

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)this.FindResource("SlideOut");
            sb.Begin();

            
            rdb_newCust.IsChecked = false;
            rdb_oldCust.IsChecked = false;
            tb_userName.Clear();
            tb_age.Clear();
            tb_password.Clear();
            SP_RegLogin.Visibility = Visibility.Collapsed;
            btn_register_login.Visibility = Visibility.Collapsed;
            rdb_newCust.Foreground = new SolidColorBrush(Colors.Black);
            rdb_oldCust.Foreground = new SolidColorBrush(Colors.Black);
            btn_cust.Background = null;

        }

        private void rdb_newCust_Click(object sender, RoutedEventArgs e)
        {
            UploadCorrectFiles(true);
            rdb_newCust.Foreground = new SolidColorBrush(Colors.RoyalBlue);
            rdb_oldCust.Foreground = new SolidColorBrush(Colors.Black);

        }
        private void rdb_oldCust_Click(object sender, RoutedEventArgs e)
        {
            UploadCorrectFiles(false);
            rdb_oldCust.Foreground = new SolidColorBrush(Colors.RoyalBlue);
            rdb_newCust.Foreground = new SolidColorBrush(Colors.Black);

        }

        private void UploadCorrectFiles(bool IsNewCustomer)
        {
            SP_RegLogin.Visibility = Visibility.Visible;
            btn_register_login.Visibility = Visibility.Visible;

            if (IsNewCustomer)
            {
                rdb_newCust.Background = new SolidColorBrush(Colors.LightBlue);
                rdb_oldCust.Background = new SolidColorBrush(Colors.LightGray);

                txt_age.Visibility = Visibility.Visible;
                tb_age.Visibility = Visibility.Visible;
                btn_register_login.Content = "Register";
            }
            else
            {
                rdb_oldCust.Background = new SolidColorBrush(Colors.LightBlue);
                rdb_newCust.Background = new SolidColorBrush(Colors.LightGray);

                txt_age.Visibility = Visibility.Collapsed;
                tb_age.Visibility = Visibility.Collapsed;
                btn_register_login.Content = "Login";
            }
        }
        #endregion


        #region Load Save Exit
        private void Upload_DB()
        {
            i = new Library();
            Book B1 = new Book("J D Salinger", "The Catcher in the Rye", "Little Brown and Company", 9.99, new DateTime(1951, 07, 16), Genre.Adventure);
            Book B2 = new Book("F Scott Fitzgerald", "The Great Gatsby", "Scribner", 15.99, new DateTime(1925, 4, 10), Genre.Historicalfiction);
            Book B3 = new Book("Harper Lee", "To Kill a Mockingbird", "Grand Central Publishing", 15.99, new DateTime(1960, 07, 11), Genre.Fantasy);
            Book B4 = new Book("Jane Austen", "Pride and Prejudice", "Penguin Classics", 8.99, new DateTime(1813, 01, 28), Genre.Romance);
            Book B5 = new Book("George Orwell", "1984", "Signet", 9.99, new DateTime(1949, 06, 08), Genre.Dystopian);
            Book B6 = new Book("idan", "TheEnd", "End Comp", 88.90, new DateTime(2007, 05, 12), Genre.Adventure);
            i.AddNewItem(B1); i.AddNewItem(B2); i.AddNewItem(B3); i.AddNewItem(B4); i.AddNewItem(B5); i.AddNewItem(B6);
            //i.LoanItem(B1);

            Jurnal j1 = new Jurnal("Science", "American Association for the Advancement of Science", 35.00, new DateTime(1880, 10, 18), Genre.Scientific);
            Jurnal j2 = new Jurnal("Nature", "Nature Publishing Group", 32.00, new DateTime(1869, 11, 04), Genre.Scientific);
            Jurnal j3 = new Jurnal("The Lancet", "Elsevier", 45.00, new DateTime(1823, 10, 05), Genre.Medical);
            Jurnal j4 = new Jurnal("The New England Journal of Medicine", "Massachusetts Medical Society", 50.00, new DateTime(1812, 01, 18), Genre.Medical);
            Jurnal j5 = new Jurnal("Harvard Business Review", "Harvard Business Press", 30.00, new DateTime(1922, 04, 02), Genre.Business);
            i.AddNewItem(j1); i.AddNewItem(j2); i.AddNewItem(j3); i.AddNewItem(j4); i.AddNewItem(j5);
            //i.LoanItem(j3); i.LoanItem(j5);
            i.MngCustList.Register("David", "34", "1234");
            i.MngCustList.Register("aaaa", "22", "aaaa");
            i.MngCustList.Register("Shoshana", "42", "1234");

            i.LoanItem(j3, i.MngCustList.LogIn("aaaa", "aaaa"));
            j3.Loan(new DateTime(2023, 01, 28));

            i.inbox.SendMessage(i.MngCustList.LogIn("aaaa", "aaaa").inbox, new InboxMessage("idan", "test1", "chekc for updates\nasd"));
            i.inbox.SendMessage(i.MngCustList.LogIn("aaaa", "aaaa").inbox, new InboxMessage("roni", "test2", "chekc for test2"));
            i.inbox.SendMessage(i.MngCustList.LogIn("aaaa", "aaaa").inbox, new InboxMessage("gal", "test3", "chekc for the spaces please\n\nasd\tsdfdf"));

        }

        public void Load()
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fs = new FileStream("TempDataBase.dat", FileMode.Open);
            i = (Library)bf.Deserialize(fs);
            fs.Close();
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fs = new FileStream("TempDataBase.dat", FileMode.Create);
            bf.Serialize(fs, i);
            fs.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!navigate)
            {
                Save();
                var result = MessageBox.Show("Are you sure you want to Exit ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion


        private void btn_ToTheTester_Click(object sender, RoutedEventArgs e)
        {
            string text = "Hey,\nWelcome to My Library Project.\nHere some info that can help you check my project:\n" +
                "1. You can find the users passwords in MainWindow.xaml.cs on \"Upload_DB\"\n   (if the password didn't change)\n" +
                "2. You can register with your own User.\n" +
                "3. You can find all the other info about my project in project Logic\"_Details.txt";

            string title = $"Info to the tester\t\t\tIdan Tal (1083)"; 

            MessageBox.Show(text, title, MessageBoxButton.OK);
        }
    }
}
