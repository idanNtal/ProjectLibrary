using Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjLab
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class SecondWindow : Window
    {
        Library i;
        Customer customer;
        UI ui;
        bool navigate = false;
        public SecondWindow(Library i, UI ui, Customer cust = null)
        {
            InitializeComponent();
            this.i = i; this.ui = ui;
            if (cust != null) this.customer = cust;
            StartPage();
            show_Buttons();
        }

        private void StartPage()
        {
            if (ui == UI.Manager)
            {
                LibraryInfoPage lip = new LibraryInfoPage(i);
                MainFrame.Content = lip;
                this.Title = "ManagerFace";
                this.Icon = new BitmapImage(new Uri("images/managerFace.png", UriKind.Relative));
                MarkButton(SP_Menu, bnt_libraryInfo);
            }
            else if (ui == UI.Customer)
            {
                CustomerInfoPage cip = new CustomerInfoPage(i, customer);
                MainFrame.Content = cip;
                this.Title = "UserFace";
                this.Icon = new BitmapImage(new Uri("images/userFace.png", UriKind.Relative));
                MarkButton(SP_Menu, btn_CustomerArea);
                int newMails = customer.inbox.Get_UnReadMessages().Count;
                btn_CustomerInbox.Content = $"📫 Inbox {(newMails>0?$" ({newMails})":"")}";
            }
        }

        private void show_Buttons()
        {
            if (ui == UI.Manager)
            {
                SP_ManagerButtons.Visibility = Visibility.Visible;
                SP_CustomerButtons.Visibility = Visibility.Collapsed;
            }
            else if (ui == UI.Customer)
            {
                SP_CustomerButtons.Visibility = Visibility.Visible;
                SP_ManagerButtons.Visibility = Visibility.Collapsed;
            }

        }

        private void bnt_back_Click(object sender, RoutedEventArgs e)
        {
            navigate = true;
            MainWindow mainWindow = new MainWindow(i);
            this.Close();
            mainWindow.WindowState = this.WindowState;
            mainWindow.ShowDialog();
        }
        
        private void MarkButton(StackPanel SP , Button b1)
        {
            foreach (var btn in SP.Children)
            {
                if (btn is Button)
                    ((Button)btn).Background = new SolidColorBrush(Colors.LightGray);
            }
            b1.Background = new SolidColorBrush(Colors.RoyalBlue);
        }

        #region Manager Buttons
        private void bnt_createItem_Click(object sender, RoutedEventArgs e)
        {
            CreateItemPage cip = new CreateItemPage(i);
            MainFrame.Content = cip;
            MarkButton(SP_ManagerButtons, bnt_createItem);
        }

        private void bnt_searchItem_Click(object sender, RoutedEventArgs e)
        {
            SearchPage sp = new SearchPage(i, UI.Manager);
            MainFrame.Content = sp;
            MarkButton(SP_ManagerButtons, bnt_searchItem);
        }

        private void bnt_libraryInfo_Click(object sender, RoutedEventArgs e)
        {
            LibraryInfoPage lip = new LibraryInfoPage(i);
            MainFrame.Content = lip;
            MarkButton(SP_ManagerButtons, bnt_libraryInfo);
        }
        
        private void bnt_CustomersInfo_Click(object sender, RoutedEventArgs e)
        {
            LibraryCustomersDetails LCD = new LibraryCustomersDetails(i);
            MainFrame.Content = LCD;
            MarkButton(SP_ManagerButtons, bnt_CustomersInfo);
        }
        #endregion

        #region Customer Buttons
        private void btn_rent_Click(object sender, RoutedEventArgs e)
        {
            SearchPage sp = new SearchPage(i, UI.Customer, customer, btn_CustomerInbox);
            MainFrame.Content = sp;
            MarkButton(SP_CustomerButtons, btn_rent);
        }

        private void btn_CustomerArea_Click(object sender, RoutedEventArgs e)
        {
            CustomerInfoPage cip = new CustomerInfoPage(i, customer);
            MainFrame.Content = cip;
            MarkButton(SP_CustomerButtons, btn_CustomerArea);
        }

        private void btn_CustomerInbox_Click(object sender, RoutedEventArgs e)
        {
            CustomerInbox ci = new CustomerInbox(customer, btn_CustomerInbox);
            MainFrame.Content = ci;
            MarkButton(SP_CustomerButtons, btn_CustomerInbox);
        }
        #endregion

        #region Exit and Save
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
                var result = MessageBox.Show("Are you sure you want to Exit ?", "Confirmation", MessageBoxButton.YesNo);
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
    }
}
