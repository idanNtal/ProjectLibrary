using Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        Library i;
        UI ui;
        Customer customer;
        Button inboxButton;
        public SearchPage(Library i, UI ui, Customer customer = null, Button b1 = null)
        {
            InitializeComponent();
            this.i = i; this.ui = ui; this.customer = customer; inboxButton = b1;
            foreach (Genre genre in (Genre[])Enum.GetValues(typeof(Genre)))
            {
                lb_genre.Items.Add(genre);
                list_genre.Items.Add(genre);
            }
            showList();
            ManagerUITools();
        }

        #region Organizer the code to Manager / Customer
        private void ManagerUITools()
        {
            if (ui == UI.Manager)
            {
                SP_Buttons.Visibility = Visibility.Visible;
                btn_Rent.Visibility = Visibility.Collapsed;
            }
            else if (ui == UI.Customer)
            {
                SP_Buttons.Visibility = Visibility.Visible;
                btn_add.Visibility = Visibility.Collapsed;
                btn_remove.Visibility = Visibility.Collapsed;
                btn_edit.Visibility = Visibility.Collapsed;
            }
        }
        private void showList()
        {
            if (ui == UI.Manager)
            {
                lb_items.Items.Clear();
                foreach (Item item in i.GetAllItems())
                    lb_items.Items.Add(item);
            }
            else if (ui == UI.Customer)
            {
                lb_items.Items.Clear();
                foreach (Item item in i.InStockitemsList)
                    lb_items.Items.Add(item);
            }
        }
        #endregion

        #region RadioButtons
        private void rdb_searchByAuthor_Checked(object sender, RoutedEventArgs e)
        {
            Show_SearchTextBox();
        }
        private void rdb_searchByTitle_Checked(object sender, RoutedEventArgs e)
        {
            Show_SearchTextBox();
        }
        private void rdb_searchByGenre_Checked(object sender, RoutedEventArgs e)
        {
            SP_Search.Visibility = Visibility.Visible;
            txt_selectGenre.Visibility = Visibility.Visible;
            lb_genre.Visibility = Visibility.Visible;
            txt_titleSearch.Visibility = Visibility.Collapsed;
            tb_searchResult.Visibility = Visibility.Collapsed;
            SP_display_edit_Fields.Visibility = Visibility.Collapsed;
        }
        private void rdb_searchByGenre_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_titleSearch.Visibility = Visibility.Visible;
            tb_searchResult.Visibility = Visibility.Visible;
            txt_selectGenre.Visibility = Visibility.Collapsed;
            lb_genre.Visibility = Visibility.Collapsed;
        }
        private void rdb_searchByCompany_Checked(object sender, RoutedEventArgs e)
        {
            Show_SearchTextBox();
        }
        private void rdb_searchByYear_Checked(object sender, RoutedEventArgs e)
        {
            Show_SearchTextBox();
        }
        #endregion

        #region EveryOne Buttons
        private void btn_seeAllList_Click(object sender, RoutedEventArgs e)
        {
            Show_SearchTextBox();
            txt_results.Text = "Results:";
            lb_items.Items.Clear();
            showList();
        }
        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            lb_items.Items.Clear();
            string input = tb_searchResult.Text;
            List<Item> tempList = new List<Item>();

            if (input != String.Empty || GetSelectedGenre(lb_genre) != 0)
            {
                try
                {
                    if (ui == UI.Manager)
                    {
                        if (rdb_searchByAuthor.IsChecked == true) { tempList = i.SearchByAuthor(input, i.GetAllItems()); txt_results.Text = $"Author: {input}"; }
                        else if (rdb_searchByTitle.IsChecked == true) { tempList = i.SearchByTitle(input, i.GetAllItems()); txt_results.Text = $"Title: {input}"; }
                        else if (rdb_searchByGenre.IsChecked == true) { tempList = i.SearchByGenre(GetSelectedGenre(lb_genre), i.GetAllItems()); txt_results.Text = $"Genre: {GetSelectedGenre(lb_genre)}"; }
                        else if (rdb_searchByYear.IsChecked == true) { tempList = i.SearchByDateTime(GetSelectedYear(input), i.GetAllItems()); txt_results.Text = $"Year: {input}"; }
                        else if (rdb_searchByCompany.IsChecked == true) { tempList = i.SearchByPublisherComp(input, i.GetAllItems()); txt_results.Text = $"Company: {input}"; }
                        else { lb_items.Items.Add("Invalid Sort Option"); txt_results.Text = "Results:"; }
                    }
                    else if (ui == UI.Customer)
                    {
                        if (rdb_searchByAuthor.IsChecked == true) { tempList = i.SearchByAuthor(input, i.InStockitemsList); txt_results.Text = $"Author: {input}"; }
                        else if (rdb_searchByTitle.IsChecked == true) { tempList = i.SearchByTitle(input, i.InStockitemsList); txt_results.Text = $"Title: {input}"; }
                        else if (rdb_searchByGenre.IsChecked == true) { tempList = i.SearchByGenre(GetSelectedGenre(lb_genre), i.InStockitemsList); txt_results.Text = $"Genre: {GetSelectedGenre(lb_genre)}"; }
                        else if (rdb_searchByYear.IsChecked == true) { tempList = i.SearchByDateTime(GetSelectedYear(input), i.InStockitemsList); txt_results.Text = $"Year: {input}"; }
                        else if (rdb_searchByCompany.IsChecked == true) { tempList = i.SearchByPublisherComp(input, i.InStockitemsList); txt_results.Text = $"Company: {input}"; }
                        else { lb_items.Items.Add("Invalid Sort Option"); txt_results.Text = "Results:"; }
                    }
                }
                catch (Exception exception)
                {
                    lb_items.Items.Add(exception.Message);
                    txt_results.Text = $"Results: {input}";
                }
                foreach (var item in tempList)
                    lb_items.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Error, Can't search an empty value");
                showList();
            }
            tb_searchResult.Text = "";
            tb_searchResult.Focus();
        }
        #endregion

        #region Manager Buttons
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (lb_items.SelectedIndex > -1)
            {
                i.AddExsistItem((Item)lb_items.SelectedItem);
                showList();
            }
            else MessageBox.Show("Error, No item selected");
        }
        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            if (lb_items.SelectedIndex > -1)
            {
                Item tempItem = (Item)lb_items.SelectedItem;
                if (i.InStockitemsList.Contains(tempItem))
                {
                    var msg = MessageBox.Show($"Are you sure you want to Delete this item ?\nSelected item {tempItem.ItemTitle}", "Confirm the Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (msg == MessageBoxResult.Yes)
                    {
                        i.RemoveItem(tempItem);
                        showList();
                    }
                }
                else if (i.LoanedItemList.Contains(tempItem))
                {
                    Customer tempCust = null;
                    foreach (Customer cust in i.MngCustList.CustList)
                    {
                        if (cust.CustItems.Contains(tempItem))
                        { 
                            tempCust = cust;
                            break;
                        } 
                    }

                    var msg = MessageBox.Show($"This item Loaned by {tempCust.Name}\nAre you sure you want to Delete this item ?\nSelected item {tempItem.ItemTitle}", "Confirm the Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (msg == MessageBoxResult.Yes)
                    {
                        i.ReturnItem(tempItem, tempCust);
                        i.RemoveItem(tempItem);
                        showList();
                        InboxMessage inboxMsg = new InboxMessage("Library", "Auto Return", $"Item {tempItem.ItemTitle} was remove from your loaned items by the Library.\nHave a Good Day :)");
                        i.inbox.SendMessage(tempCust.inbox, inboxMsg);
                    }
                }
            }
            else MessageBox.Show("Error, No item selected");
        }
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (lb_items.SelectedIndex > -1)
            {
                SP_Buttons.IsEnabled = false;
                lb_items.IsEnabled = false;
                Enable_display_edit_Fields(true);
                SP_edit_ButtonsOptions.Visibility = Visibility.Visible;

                Item item = (Item)lb_items.SelectedItem;
                CheckItem_ForAuthorVisible(item);

                tb_title.Text = item.ItemTitle;
                tb_comp.Text = item.PublisherComp;
                tb_price.Text = $"{item.loanPrice}";
                tb_publisheddate.Text = $"{item.published:d}";

                list_genre.SelectedItems.Clear();
                foreach (var val in list_genre.Items)
                    if (item.genre.ToString().Contains(val.ToString()))
                        list_genre.SelectedItem = val;
            }
            else MessageBox.Show("Error, No item selected");
        }
        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            Item item = (Item)lb_items.SelectedItem;
            if (ValidInput.checkAll(tb_title.Text, tb_comp.Text, tb_price.Text, DateTime.Parse(tb_publisheddate.Text), GetSelectedGenre(list_genre), tb_author.Text))
            {
                i.EditItem(item, tb_title.Text, tb_comp.Text, double.Parse(tb_price.Text), DateTime.Parse(tb_publisheddate.Text), GetSelectedGenre(list_genre), tb_author.Text);
                lb_items.IsEnabled = true;
                SP_edit_ButtonsOptions.Visibility = Visibility.Collapsed;
                SP_Buttons.IsEnabled = true;
                showList();
                txt_updates.Foreground = new SolidColorBrush(Colors.LightGreen);
                txt_updates.Text = "Item updated successfully !";
                Enable_display_edit_Fields(false);
            }


            else
            {
                txt_updates.Foreground = new SolidColorBrush(Colors.DarkRed);
                txt_updates.Text = "Item updated Failed !";
            }
            await Task.Delay(2000);
            txt_updates.Text = "";
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SP_Search.Visibility = Visibility.Visible;
            SP_display_edit_Fields.Visibility = Visibility.Collapsed;
            lb_items.IsEnabled = true;
            SP_edit_ButtonsOptions.Visibility = Visibility.Collapsed;
            SP_Buttons.IsEnabled = true;
        }
        #endregion

        #region Customer Buttons
        private void btn_Rent_Click(object sender, RoutedEventArgs e)
        {
            if (customer == null)
                MessageBox.Show("Error, Customer is Null");
            if (lb_items.SelectedIndex > -1)
            {
                Item tempitem = (Item)lb_items.SelectedItem;
                var msg = MessageBox.Show($"Item Selected: {tempitem}\nPrice {tempitem.loanPrice:c}\nAre you sure you want to rent this item ?", $"Confirm the rent", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (msg == MessageBoxResult.Yes)
                {
                    i.LoanItem(tempitem, customer);
                    showList();
                    string text = $"Thank you for rent the {tempitem.GetType().Name} {tempitem.ItemTitle}." +
                        $"\nRent Date: {tempitem.loanDate:D}\nReturn Date: {tempitem.loanDate.AddDays(14):D}";
                    InboxMessage inboxMsg = new InboxMessage("Library", $"Rent Item", text);
                    i.inbox.SendMessage(customer.inbox, inboxMsg);
                    int newMails = customer.inbox.Get_UnReadMessages().Count;
                    inboxButton.Content = $"📫 Inbox {(newMails > 0 ? $" ({newMails})" : "")}";
                }
            }
            else MessageBox.Show("Error, No item selected");
        }
        #endregion

        #region GetValues
        private DateTime GetSelectedYear(string str)
        {
            DateTime dt = new DateTime();
            if (str.Length == 4 && str.All(Char.IsDigit))
                dt = new DateTime(int.Parse(str), 01, 01);
            return dt;
        }

        private Genre GetSelectedGenre(ListBox listBox)
        {
            List<Genre> GenreList = new List<Genre>();
            Genre g = 0;
            foreach (object obj in listBox.SelectedItems)
                GenreList.Add((Genre)obj);
            foreach (Genre genre in GenreList)
                g |= genre;

            return g;
        }
        #endregion

        private void lb_items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // display the selected item values
            if (lb_items.SelectedIndex > -1)
            {
                Enable_display_edit_Fields(false);

                Item item = (Item)lb_items.SelectedItem;
                CheckItem_ForAuthorVisible(item);


                tb_title.Text = item.ItemTitle;
                tb_comp.Text = item.PublisherComp;
                tb_price.Text = $"{item.loanPrice:c}";
                tb_publisheddate.Text = $"{item.published:d}";

                cBox_genreToDisplay.Items.Clear();

                foreach (var val in list_genre.Items)
                    if (item.genre.ToString().Contains(val.ToString()))
                        cBox_genreToDisplay.Items.Add(val);

                if (item.loanDate != new DateTime())
                    tb_loandate.Text = $"{item.loanDate:d}";
                else
                    tb_loandate.Text = "";
            }
        }

        private void Enable_display_edit_Fields(bool IsEnable)
        {
            // methode that set the correcr fields accessibility (for display / edit)
            SP_Search.Visibility = Visibility.Collapsed;
            SP_display_edit_Fields.Visibility = Visibility.Visible;
            if (IsEnable)   // edit
            {
                tb_author.IsReadOnly = false;
                tb_title.IsReadOnly = false;
                cBox_genreToDisplay.Visibility = Visibility.Collapsed;
                list_genre.Visibility = Visibility.Visible;
                tb_comp.IsReadOnly = false;
                tb_price.IsReadOnly = false;
                tb_publisheddate.IsEnabled = true;
                txt_loandate.Visibility = Visibility.Collapsed;
                tb_loandate.Visibility = Visibility.Collapsed;

            }
            else            // display
            {
                tb_author.IsReadOnly = true;
                tb_title.IsReadOnly = true;
                list_genre.Visibility = Visibility.Collapsed;
                cBox_genreToDisplay.Visibility = Visibility.Visible;

                tb_comp.IsReadOnly = true;
                tb_price.IsReadOnly = true;
                tb_publisheddate.IsEnabled = false;
            }

        }
        
        private void Show_SearchTextBox()
        {
            SP_Search.Visibility = Visibility.Visible;
            SP_display_edit_Fields.Visibility = Visibility.Collapsed;
            tb_searchResult.Text = "";
        }
        
        private void CheckItem_ForAuthorVisible(Item item)
        {
            // methode that check if the item have an author or not and set the correct fields
            if (item is IAuthorable)
            {
                IAuthorable temp = (IAuthorable)item;
                tb_author.Text = temp.Author;
                txt_author.Visibility = Visibility.Visible;
                tb_author.Visibility = Visibility.Visible;
            }
            else
            {
                txt_author.Visibility = Visibility.Collapsed;
                tb_author.Visibility = Visibility.Collapsed;
            }
        }

        private void tb_searchResult_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // methode that make the search value field access to number only in Year search
            if (rdb_searchByYear.IsChecked == true)
            {
                int result;
                if (!int.TryParse(e.Text, out result))
                {
                    e.Handled = true;
                }
            }
        }
    }
}
