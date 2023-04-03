using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace ProjLab
{
    /// <summary>
    /// Interaction logic for CustomerInbox.xaml
    /// </summary>
    public partial class CustomerInbox : Page
    {

        Customer customer;
        Button inboxButton;
        public CustomerInbox(Customer customer, Button b1)
        {
            InitializeComponent();
            this.inboxButton = b1;
            this.customer = customer;
            chb_allmails.IsChecked = true;
        }

        private void NewEmailAlert()
        {
            int newMails = customer.inbox.Get_UnReadMessages().Count;
            inboxButton.Content = $"📫 Inbox {(newMails > 0 ? $" ({newMails})" : "")}";
        }

        private void lv_Email_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_Email.SelectedItem is InboxMessage)
            {
                InboxMessage tempMsg = (InboxMessage)lv_Email.SelectedItem;
                txt_EmailText.Text = tempMsg.showMail();

                lv_Email.Items.Refresh();
                NewEmailAlert();
            }
        }

        #region Delete Messages
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (lv_Email.SelectedItem is InboxMessage)
            {
                var msg = MessageBox.Show("Are you sure you want to delete this message ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (msg == MessageBoxResult.Yes)
                {
                    InboxMessage tempMsg = (InboxMessage)lv_Email.SelectedItem;
                    customer.inbox.DeleteMessage(tempMsg);
                    txt_EmailText.Text = "";
                    lv_Email.Items.Refresh();
                }
            }
            else MessageBox.Show("No Message Selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btn_deleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (customer.inbox.messages.Count == 0)
                MessageBox.Show("No Message in your Inbox", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (lv_Email.Items.Count == 0)
                MessageBox.Show("No Message in this Category (Read/UnRead)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var msg = MessageBox.Show("Are you sure you want to delete All those messages?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (msg == MessageBoxResult.Yes)
                {
                    DeleteAll_ByChxBox();
                    NewEmailAlert();
                }
            }
        }

        private void DeleteAll_ByChxBox()
        {
            if (chb_allmails.IsChecked == true)
            {
                customer.inbox.DeleteAll_Messages();
            }
            else if (chb_read.IsChecked == true)
            {
                customer.inbox.DeleteAll_ReadMessages();
            }
            else if (chb_unread.IsChecked == true)
            {
                customer.inbox.DeleteAll_UnReadMessages();
            }
            UploadCorrectList();
        }
        #endregion

        #region Checkboxs
        private void chb_allmails_Checked(object sender, RoutedEventArgs e)
        {
            chbox_IsCheckManager(SP_chboxs, chb_allmails);
            UploadCorrectList();
        }

        private void chb_read_Checked(object sender, RoutedEventArgs e)
        {
            chbox_IsCheckManager(SP_chboxs, chb_read);
            UploadCorrectList();
            
        }

        private void chb_unread_Checked(object sender, RoutedEventArgs e)
        {
            chbox_IsCheckManager(SP_chboxs, chb_unread);
            UploadCorrectList();
            
        }

        private void chbox_IsCheckManager(StackPanel SP, CheckBox chb)
        {
            foreach (CheckBox SPchb in SP.Children)
                if (SPchb != chb)
                    SPchb.IsChecked = false;
        }
        #endregion
        
        private void UploadCorrectList()
        {
            txt_EmailText.Text = "";
            lv_Email.ItemsSource = new List<InboxMessage>();
            if (chb_allmails.IsChecked == true)
            {
                lv_Email.ItemsSource = customer.inbox.messages;
            }
            else if (chb_read.IsChecked == true)
            {
                lv_Email.ItemsSource = customer.inbox.Get_ReadMessages();
            }
            else if (chb_unread.IsChecked == true)
            {
                lv_Email.ItemsSource = customer.inbox.Get_UnReadMessages();
            }
        }

    }
}
