using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;


namespace Logic
{
    [Serializable]
    public class Library
    {

        #region Items Lists (inStock/Renter/All)
        List<Item> instockItems = new List<Item>();
        public List<Item> InStockitemsList { get { return instockItems; }}

        List<Item> loanedItems = new List<Item>();
        public List<Item> LoanedItemList { get { return loanedItems; }}

        // Get List of all the Items
        public List<Item> GetAllItems()
        {
            List<Item> FullList = new List<Item>();
            foreach (var item in instockItems)
                FullList.Add(item);
            foreach (var item in LoanedItemList)
                FullList.Add(item);
            return FullList;
        }
        #endregion


        #region Inbox
        Inbox _inbox = new Inbox();
        public Inbox inbox { get { return _inbox; } }
        #endregion


        #region CustomersList
        ManageCustomerList ManagerCustomerList = new ManageCustomerList();
        public ManageCustomerList MngCustList { get { return ManagerCustomerList; }}
        #endregion


        #region Single Item Functions 
        public string DisplayItem(Item item)
        {
            if (item != null)
                return item.info();
            throw new ArgumentException("Book not found"); // Create Exception
        }
        public void EditItem(Item item, string newTitle, string publisherComp, double loanPrice, DateTime published, Genre genre, string author = null)
        {
            if (newTitle != null) item.ChangeTitle(newTitle);
            if (publisherComp != null) item.ChangePublisherComp(publisherComp);
            if (loanPrice > 0) item.ChangeLoanPrice(loanPrice);
            if (published != new DateTime()) item.ChangePublishedDate(published);
            if (genre > 0) item.ChangeGenre(genre);

            if (item is IAuthorable)
            {
                if (author != null)
                {
                    IAuthorable authorable = (IAuthorable)item;
                    authorable.Author = author;
                }
            }
        }
        public void AddNewItem(Item item)
        {
            if (item == null || instockItems.Contains(item))
                throw new ArgumentException("Error");       // Create Exception
            instockItems.Add(item);
        }
        public void AddExsistItem(Item item)
        {
            if (item == null)
                throw new ArgumentException("Error"); // create Exception
            Item copy = item.Clone();
            instockItems.Add(copy);
        }
        public void RemoveItem(Item item)
        {
            if (item == null)          // make a self Exception
                throw new ArgumentNullException("The item is null");
            else if (instockItems.Contains(item))
                instockItems.Remove(item);

            else
                throw new ArgumentException("The item isn't in the Items List");
        }
#endregion


        #region Search By Category (For customer)
        public List<Item> SearchByTitle(string title, List<Item> list)
        {
            if (title != null)
            {
                List<Item> results = new List<Item>();
                foreach (Item item in list)
                {
                    if (item.ItemTitle.ToLower().Contains(title.ToLower()))
                        results.Add(item);
                }
                if (results.Count > 0) 
                    return results;
            }
            throw new Exception("No Book Found");
        }
        public List<Item> SearchByAuthor(string name, List<Item> list)
        {
            if (name != null)
            {
                List<Item> results = new List<Item>();
                foreach (Item item in list)
                {
                    if (item is IAuthorable)
                    {
                        IAuthorable temp = (IAuthorable)item;
                        if (temp.Author.ToLower().Contains(name.ToLower()))
                            results.Add(item);
                    }
                }
                if (results.Count > 0) return results;
            }
            throw new Exception("No Book Found");
        }
        public List<Item> SearchByGenre(Genre genre, List<Item> list)
        {
            List<Item> Results = new List<Item>();
            foreach (Item item in list)
            {
                if (item.genre == (genre | item.genre))
                    Results.Add(item);
            }
            if (Results.Count >= 1) return Results;
            throw new Exception("No Book Found");
        }
        public List<Item> SearchByPublisherComp(string Publisher, List<Item> list)
        {
            if (Publisher != null)
            {
                List<Item> Results = new List<Item>();

                foreach (Item item in list)
                {
                    if (item.PublisherComp.ToLower().Contains(Publisher.ToLower()))
                        Results.Add(item);
                }
                if (Results.Count >= 1) return Results;
            }
            throw new Exception("No Book Found");
        }
        public List<Item> SearchByDateTime(DateTime dateTime, List<Item> list)
        {
            List<Item> Results = new List<Item>();
            if (dateTime != new DateTime())
            {
                foreach (Item item in list)
                {
                    if (item.published.Year == dateTime.Year)
                        Results.Add(item);
                }
                if (Results.Count >= 1) return Results;
            }
            throw new Exception("No Book Found");
        }
        #endregion


        #region Loan methodes
        public void LoanItem(Item item, Customer customer)
        {
            if (item == null || !instockItems.Contains(item))
                throw new Exception();
            else if (item.loanDate != new DateTime())
                throw new Exception("Item is unavilable");
            else
            {
                item.Loan(DateTime.Now);
                customer.LoanItem(item);
                loanedItems.Add(item);
                instockItems.Remove(item);
            }
        }
        public void ReturnItem(Item item, Customer customer)
        {
            if (item == null && !loanedItems.Contains(item))
                throw new Exception();
            else if (item.loanDate == DateTime.Now)
                throw new Exception("Item is unavilable");
            else
            {
                item.Loan(new DateTime());
                customer.ReturnItem(item);
                loanedItems.Remove(item);
                instockItems.Add(item);
            }
        }
        public string LateReturn()
        {
            Dictionary<string, int> dict = CheckReturnTime(14);

            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
                sb.AppendLine($"{item.Key}: {item.Value}");
            if (sb.Length > 0)
                return sb.ToString();
            return "The List is empty";
        }
        private Dictionary<string, int> CheckReturnTime(short lateInDays = 14)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var item in loanedItems)
            {
                DateTime returnDate = item.loanDate.AddDays(lateInDays);
                if (returnDate < DateTime.Now)
                {
                    var diffOfDates = DateTime.Now - returnDate;
                    dict.Add(item.ItemTitle, diffOfDates.Days);
                }
            }
            return dict;
        }

        public int Check_ItemReturn_Time(Item item, int lateInDays = 14)
        {
            DateTime returnDate = item.loanDate.AddDays(lateInDays);
            var diffOfDates = returnDate - DateTime.Now;
            return diffOfDates.Days;
        }
        #endregion


        #region Count per Type of Item
        public Dictionary<Type, int> ItemCount()
        {
            Dictionary<Type, int> dict = new Dictionary<Type, int>();

            foreach (Item item in GetAllItems())
            {
                if (!dict.ContainsKey(item.GetType()))
                    dict.Add(item.GetType(), 1);
                else if (dict.ContainsKey(item.GetType()))
                    dict[item.GetType()] += 1;
            }
            return dict;
        }
        #endregion
    }
}
