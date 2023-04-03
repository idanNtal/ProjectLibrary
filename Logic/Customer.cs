using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    [Serializable]
    public class Customer
    {
        List<Item> custItems = new List<Item>();
        public List<Item> CustItems { get { return custItems; } }

        Inbox _inbox = new Inbox();
        public Inbox inbox { get { return _inbox; } }

        #region properties
        private string _name;
        private int _age;
        private string _password;
        public string Name { get { return _name; } set { _name = value; } }
        public int Age { get { return _age; } set { if (value > 0) _age = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        #endregion

        public Customer(string name, int age, string password)
        {
            if (!ValidInput.checkString(name))
                throw new CustomerException("Invalid Customer Name");
            if (age < 0)
                throw new CustomerException("Invalid Customer Age");

            this._name = name; this._age = age; this._password = password;
        }

        public void LoanItem(Item item) => custItems.Add(item);
        public void ReturnItem(Item item) => custItems.Remove(item);
        public override string ToString() => _name;
    }




    [Serializable]
    public class ManageCustomerList
    {
        List<Customer> customerList = new List<Customer>();
        public List<Customer> CustList { get { return customerList; } }

        private List<string> GetAllNames()
        {
            List<string> namesList = new List<string>();
            foreach (Customer customer in CustList)
                namesList.Add(customer.Name);
            return namesList;
        }
        
        public Customer Register(string name, string age, string password)
        {
            if (!ValidInput.checkString(name))
                throw new CustomerException("Invalid name");
            if (GetAllNames().Contains(name))
                throw new CustomerException("Name already exists");
            if (!ValidInput.checkInt(age) || int.Parse(age) < 14 || int.Parse(age) > 99)
                throw new CustomerException("Invalid age\nAge can't be under 14 and above 99");
            if (password.Length < 4)
                throw new CustomerException("Password can't be less than 4 chars");
            if (password.Contains(' '))
                throw new CustomerException("Password can't contain Spaces");

            Customer tempCust = new Customer(name, int.Parse(age), password);
            customerList.Add(tempCust);

            return tempCust;
        }

        public Customer LogIn(string name, string password)
        {
            if (GetAllNames().Contains(name))
            {
                foreach (var cust in customerList)
                {
                    if (cust.Name == name)
                    {
                        if (ConfirmPassword(cust.Password, password))
                            return cust;
                        else break;
                    }
                }
                throw new CustomerException("Password was UnCorrect, Please try again");
            }
            else throw new CustomerException("Name was UnCorrect, Please try again");
        }

        public void ReomveCust(Customer cust) => customerList.Remove(cust);

        private bool ConfirmPassword(string CorrectPass, string inputPass) => CorrectPass.Equals(inputPass);

        public void ChangeUserName(Customer cust, string newName)
        {
            if (!ValidInput.checkString(newName))
                throw new CustomerException("Invalid name");
            foreach (string nameFromList in GetAllNames())
            {
                if (newName == nameFromList)
                    throw new CustomerException("Name already exists");
            }
            cust.Name = newName;
        }

        public void ChagnePassword(Customer cust, string newPasse)
        {
            if (newPasse.Length < 4)
                throw new CustomerException("Password can't be less than 4 chars");
            if (newPasse.Contains(' '))
                throw new CustomerException("Password can't contain Spaces");
            if (newPasse == cust.Password)
                throw new CustomerException("Can't change to the same password");

            cust.Password = newPasse;
        }

    }

    [Serializable]
    public class CustomerException : Exception
    {
        public CustomerException() : base("Error ! Invalid values ") { }
        public CustomerException(string message) : base(message) { }
    }
}
