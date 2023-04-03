using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateItemPage.xaml
    /// </summary>
    public partial class CreateItemPage : Page
    {
        Library i;
        StringBuilder sbErrors = ValidInput.sbErrors;

        public CreateItemPage(Library i)
        {
            InitializeComponent();
            this.i = i;

            foreach (Genre genre in (Genre[])Enum.GetValues(typeof(Genre)))
                lb_genre.Items.Add(genre);
        }

        #region Clickers
        private void rdb_book_Checked(object sender, RoutedEventArgs e)
        {
            txt_author.Visibility = Visibility.Visible;
            tb_author.Visibility = Visibility.Visible;
        }

        private void rdb_jurnal_Checked(object sender, RoutedEventArgs e)
        {
            txt_author.Visibility = Visibility.Collapsed;
            tb_author.Visibility = Visibility.Collapsed;
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            sbErrors.Clear();
            txt_errors.Text = "";
            string tempAuthor, tempTitle = tb_title.Text, tempCompany = tb_comp.Text, tempLoanPrice = tb_price.Text;
            Genre tempGenre = setGenre();
            DateTime tempT = SetDate();
            bool Created = false;
            if (rdb_book.IsChecked == true)
            {
                tempAuthor = tb_author.Text;
                if (ValidInput.checkAll(tempTitle, tempCompany, tempLoanPrice, tempT, tempGenre, tempAuthor))
                {
                    Book a = new Book(tempAuthor, tempTitle, tempCompany, double.Parse(tempLoanPrice), tempT, tempGenre);
                    i.AddNewItem(a);
                    Created = LibraryItemAdded_String(a);
                }
            }
            else if (rdb_jurnal.IsChecked == true)
            {
                tempAuthor = "Noauthor";
                if (ValidInput.checkAll(tempTitle, tempCompany, tempLoanPrice, tempT, tempGenre, tempAuthor))
                {
                    Jurnal a = new Jurnal(tempTitle, tempCompany, double.Parse(tempLoanPrice), tempT, tempGenre);
                    i.AddNewItem(a);
                    Created = LibraryItemAdded_String(a);
                }
            }
            else
                sbErrors.AppendLine("• No Item Selected");

            ErrorText_Color(Created);
        }
        #endregion


        private bool LibraryItemAdded_String(Item a)
        {
            sbErrors.Append($"• New {a.GetType().Name} added to the Library");
            ClearAll();
            return true;
        }
        private async void ErrorText_Color(bool Created)
        {
            if (Created)
            {
                txt_errors.Foreground = new SolidColorBrush(Colors.LightGreen);
                txt_errors.Text = sbErrors.ToString();
            }
            else
            {
                txt_errors.Foreground = new SolidColorBrush(Colors.DarkRed);
                txt_errors.Text = sbErrors.ToString();
            }
            await Task.Delay(5000);
            txt_errors.Text = "";
        }
        private Genre setGenre()
        {
            List<Genre> GenreList = new List<Genre>();
            Genre g = 0;
            foreach (object obj in lb_genre.SelectedItems)
                GenreList.Add((Genre)obj);
            foreach (Genre genre in GenreList)
                g |= genre;

            return g;
        }
        private DateTime SetDate()
        {
            DateTime? test = dp.SelectedDate;
            if (test != null && test <= DateTime.Now)
            {
                //txt_date.Text = test.ToString();   // self test
                return (DateTime)test;
            }
            sbErrors.AppendLine("• Select a valid Date");
            return new DateTime();
        }

        private void ClearAll()
        {
            rdb_book.IsChecked = false;
            rdb_jurnal.IsChecked = false;
            SP_TextBoxs.Children.Clear();
            lb_genre.UnselectAll();
            dp.SelectedDate = null;
        }

    }
}
