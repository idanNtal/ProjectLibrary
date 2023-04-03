using Logic;
using System;
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

namespace ProjLab
{
    /// <summary>
    /// Interaction logic for LibraryCustomersDetails.xaml
    /// </summary>
    public partial class LibraryCustomersDetails : Page
    {
        Library i;
        public LibraryCustomersDetails(Library i)
        {
            InitializeComponent();
            this.i = i;
            Upload_CustoemrList();
        }

        private void Upload_CustoemrList()
        {
            lv_userNamesList.Items.Clear();
            foreach (var cust in i.MngCustList.CustList)
                lv_userNamesList.Items.Add(cust);
        }

        private void lv_userNamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lv_customerItems.Items.Clear();
            lv_DaysToReturn.Items.Clear();
            if (lv_userNamesList.SelectedIndex > -1)
            {
                Customer tempCust = (Customer)lv_userNamesList.SelectedItem;
                txt_customerItems.Text = tempCust.Name + "'s Items";
                foreach (Item item in tempCust.CustItems)
                {
                    lv_customerItems.Items.Add(item);
                }
                foreach (Item item in lv_customerItems.Items)
                {
                    lv_DaysToReturn.Items.Add(i.Check_ItemReturn_Time(item));
                }
                Update_CustomerInfo(tempCust);
            }
        }

        #region Clear & Update Customer info
        private void Update_CustomerInfo(Customer cust)
        {
            tb_userName.Text = cust.Name;
            tb_age.Text = cust.Age.ToString();
            tb_rentedAmount.Text = cust.CustItems.Count.ToString();
            tb_lateReturn.Text = Get_LateReturn_Amount(cust).ToString();
            tb_EmailSendTo.Text = cust.Name;
        }

        private void Clear_CustomerInfo()
        {
            tb_userName.Text = "";
            tb_age.Text = "";
            tb_rentedAmount.Text = "";
            tb_lateReturn.Text = "";
            tb_EmailSendTo.Text = "";
        }
        #endregion

        #region Buttons
        private void btn_sendMsg_Click(object sender, RoutedEventArgs e)
        {
            Customer tempCust = (Customer)lv_userNamesList.SelectedItem;
            if (lv_userNamesList.SelectedItem is Customer)
            {
                try
                {
                    i.inbox.SendMessage(tempCust.inbox, new InboxMessage("Library", tb_EmailTitle.Text, tb_EmailText.Text));
                    tb_EmailTitle.Clear();
                    tb_EmailText.Clear();
                    MessageBox.Show($"Email Send to {tempCust.Name}", "Email Send", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (MessageException me)
                {
                    MessageBox.Show(me.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else MessageBox.Show("Need to select a User first", "No addressee", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btn_removeCust_Click(object sender, RoutedEventArgs e)
        {
            if (lv_userNamesList.SelectedItem is Customer)
            {
                Customer tempCust = (Customer)lv_userNamesList.SelectedItem;
                if (tempCust.CustItems.Count == 0)
                {
                    var res = MessageBox.Show($"Customer Selected: {tempCust.Name}\nAre you sure you want to delete this Customer ?", "Cusomer Delete", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        i.MngCustList.ReomveCust(tempCust);
                        Clear_CustomerInfo();
                    }
                }
                else
                {
                    var res = MessageBox.Show($"Customer Selected: {tempCust.Name}\nThis customer have {tempCust.CustItems.Count} loaned items.\nAre you sure you want to delete this Customer ?\nAll his items will return automaiclly.", "Cusomer Delete", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        Item[] items_arr = tempCust.CustItems.ToArray();
                        for (int j = 0; j < items_arr.Length; j++)
                        {
                            i.ReturnItem(items_arr[j],tempCust);
                            i.MngCustList.ReomveCust(tempCust);
                            Clear_CustomerInfo();
                        }
                    }
                }
                Upload_CustoemrList();
            }
            else MessageBox.Show("No Customer Selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        private int Get_LateReturn_Amount(Customer cust)
        {
            int cnt = 0;
            foreach (Item item in cust.CustItems)
            {
                if (i.Check_ItemReturn_Time(item) < 0)
                    cnt++;
            }
            return cnt;
        }
    }
}
