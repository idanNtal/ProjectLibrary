using Logic;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ProjLab
{
    /// <summary>
    /// Interaction logic for RentItemPage.xaml
    /// </summary>
    public partial class CustomerInfoPage : Page
    {
        Library i;
        Customer customer;
        public CustomerInfoPage(Library i, Customer customer)
        {
            InitializeComponent();
            this.i = i; this.customer = customer;
            Upload_UserFields();
            Upload_UserLoanedItems();
        }

        private void Upload_UserFields()
        {
            txt_welcomeUser.Text = customer.Name + " Information";
            txt_userName.Text = customer.Name;
            txt_userAge.Text = customer.Age.ToString();
            txt_userItemLoaned.Text = customer.CustItems.Count().ToString();
        }
        private void Upload_UserLoanedItems()
        {
            lb_ItemsToReturn.Items.Clear();
            lv_DaysToReturn.Items.Clear();
            foreach (Item item in customer.CustItems)
            {
                lb_ItemsToReturn.Items.Add(item);
                lv_DaysToReturn.Items.Add(i.Check_ItemReturn_Time(item));
            }
        }
        private void Manage_EditUserInfo_Fields(bool ToVisible)
        {
            tb_editInfo.Text = "";
            if (ToVisible)
            {
                txt_editInfo.Visibility = Visibility.Visible;
                tb_editInfo.Visibility = Visibility.Visible;
                btn_change.Visibility = Visibility.Visible;
            }
            else
            {
                txt_editInfo.Visibility = Visibility.Collapsed;
                tb_editInfo.Visibility = Visibility.Collapsed;
                btn_change.Visibility = Visibility.Collapsed;
            }
        }

        #region Buttons
        private void btn_editName_Click(object sender, RoutedEventArgs e)
        {
            Manage_EditUserInfo_Fields(true);
            txt_editInfo.Text = "Select New Name";
        }
        private void btn_editPass_Click(object sender, RoutedEventArgs e)
        {
            Manage_EditUserInfo_Fields(true);
            txt_editInfo.Text = "Select New Password";
        }
        private void btn_change_Click(object sender, RoutedEventArgs e)
        {

            if (txt_editInfo.Text == "Select New Name")
            {
                try
                {
                    i.MngCustList.ChangeUserName(customer,tb_editInfo.Text);
                    Upload_UserFields();
                    Manage_EditUserInfo_Fields(false);
                    show_ChageString_Status(true, "UserName changed successfully");
                }
                catch (CustomerException CE)
                {
                    show_ChageString_Status(false, CE.Message);
                }
            }
            else if (txt_editInfo.Text == "Select New Password")
            {
                try
                {
                    i.MngCustList.ChagnePassword(customer, tb_editInfo.Text);
                    Manage_EditUserInfo_Fields(false);
                    show_ChageString_Status(true, "Password changed successfully");
                }
                catch (CustomerException CE)
                {
                    show_ChageString_Status(false, CE.Message);
                }
            }
        }
        private void btn_return_Click(object sender, RoutedEventArgs e)
        {
              
            if (lb_ItemsToReturn.SelectedIndex > -1)
            {
                Item tempitem = (Item)lb_ItemsToReturn.SelectedItem;
                var msg = MessageBox.Show($"Item Selected: {tempitem}\nAre you sure you want to return this item ?", $"Confirm the rent", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (msg == MessageBoxResult.Yes)
                {
                    i.ReturnItem(tempitem, customer);
                    Upload_UserFields();
                    Upload_UserLoanedItems();
                    ClearAll_DisplayItem();
                }
            }
            else MessageBox.Show("Error, No item selected !");

        }
#endregion


        private async void show_ChageString_Status(bool IsSuccess, string Change)
        {
            if (IsSuccess)
            {
                txt_changeStaus.Foreground = new SolidColorBrush(Colors.LightGreen);
                txt_changeStaus.Text = Change;
            }
            else
            {
                txt_changeStaus.Foreground = new SolidColorBrush(Colors.DarkRed);
                txt_changeStaus.Text = Change;
            }
            await Task.Delay(3000);
            txt_changeStaus.Text = "";

        }

        private void lb_ItemsToReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_ItemsToReturn.SelectedIndex > -1)
            {
                Item tempitem = (Item)lb_ItemsToReturn.SelectedItem;

                tb_title.Text = tempitem.ItemTitle;
                tb_comp.Text = tempitem.PublisherComp;
                tb_price.Text = $"{tempitem.loanPrice:c}";
                tb_publisheddate.Text = $"{tempitem.published:d}";
                tb_loandate.Text = $"{tempitem.loanDate:d}";

                cBox_genreToDisplay.Items.Clear();
                foreach (Genre val in (Genre[])Enum.GetValues(typeof(Genre)))
                    if (tempitem.genre.ToString().Contains(val.ToString()))
                        cBox_genreToDisplay.Items.Add(val);

                if (tempitem is IAuthorable)
                {
                    txt_author.Visibility = Visibility.Visible;
                    tb_author.Visibility = Visibility.Visible;
                    IAuthorable author = (IAuthorable)tempitem;
                    tb_author.Text = author.Author;
                }
                else
                {
                    txt_author.Visibility = Visibility.Collapsed;
                    tb_author.Visibility = Visibility.Collapsed;
                }

            }
        }

        private void ClearAll_DisplayItem()
        {
            foreach (var item in SP_display_Fields.Children)
            {
                if (item is TextBox)
                    ((TextBox)item).Text = "";
            } 
        }
    }
}
