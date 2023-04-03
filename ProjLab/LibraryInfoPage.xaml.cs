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
    /// Interaction logic for LibraryInfoPage.xaml
    /// </summary>
    public partial class LibraryInfoPage : Page
    {
        Library i;
        public LibraryInfoPage(Library i)
        {
            InitializeComponent();
            this.i = i;
            uploadFirstView();
            show();
        }

        private void uploadFirstView()
        {
            foreach (Item item in i.GetAllItems())
                lb_Display.Items.Add(item);

        }

        private void show()
        {
            Dictionary<Type, int> dict = i.ItemCount();
            lv_types.Items.Add("All Items");
            lv_amount.Items.Add(i.GetAllItems().Count);
            foreach (var item in dict)
            {
                lv_types.Items.Add(item.Key.Name);
                lv_amount.Items.Add(item.Value);
            }
            lv_rentStatus.Items.Add("UnRented");
            lv_rentStatus.Items.Add("Rented");
        }

        private void lv_types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lv_rentAmount.Items.Clear();
            lv_rentStatus.UnselectAll();
            SP_DaysToReturn.Visibility = Visibility.Collapsed;
            SetRent_UnrentValues(lv_types.SelectedItem.ToString());
            showItemsOnListBoxes(lv_types.SelectedItem.ToString(), i.GetAllItems());
        }
        private void lv_rentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showStatusOnListBox(lv_rentStatus.SelectedIndex);
        }


        private void SetRent_UnrentValues(string typeName)
        {
            if (typeName == "All Items")
            {
                lv_rentAmount.Items.Add(i.InStockitemsList.Count);
                lv_rentAmount.Items.Add(i.LoanedItemList.Count);
            }
            else
            {
                lv_rentAmount.Items.Add(Count_UnRentedByType(typeName));
                lv_rentAmount.Items.Add(Count_RentedByType(typeName));
            }
        }
        private int Count_UnRentedByType(string typeName)
        {
            int cnt = 0;
            foreach (var item in i.InStockitemsList)
            {
                if (item.GetType().Name == typeName)
                    cnt++;
            }
            return cnt;
        }
        private int Count_RentedByType(string typeName)
        {
            int cnt = 0;
            foreach (var item in i.LoanedItemList)
            {
                if (item.GetType().Name == typeName)
                    cnt++;
            }
            return cnt;
        }


        private void showItemsOnListBoxes(string TypeName, List<Item> itemsList)
        {
            lb_Display.Items.Clear(); 
            lv_DaysToReturn.Items.Clear();
            foreach (Item item in itemsList)
            {
                if (TypeName == "All Items")
                    lb_Display.Items.Add(item);
                else if (TypeName == item.GetType().Name)
                    lb_Display.Items.Add(item);
            }
            foreach (Item item in lb_Display.Items)
            {
                lv_DaysToReturn.Items.Add(i.Check_ItemReturn_Time(item));
            }
        }
        private void showStatusOnListBox(int indx)
        {
            lb_Display.Items.Clear();
            lv_DaysToReturn.Items.Clear();
            if (lv_types.SelectedIndex > -1)
            {
                if (indx == 0)
                {
                    SP_DaysToReturn.Visibility = Visibility.Collapsed;
                    showItemsOnListBoxes(lv_types.SelectedItem.ToString(), i.InStockitemsList);
                }
                else if (indx == 1)
                {
                    SP_DaysToReturn.Visibility = Visibility.Visible;
                    showItemsOnListBoxes(lv_types.SelectedItem.ToString(), i.LoanedItemList);
                }
            }
        }
    }
}
