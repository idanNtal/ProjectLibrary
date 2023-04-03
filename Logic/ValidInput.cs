using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logic
{
    [Serializable]
    public static class ValidInput
    {
        public static StringBuilder sbErrors = new StringBuilder();
        
        public static bool checkAll(string title, string publisherComp, string loanPrice, DateTime published, Genre g, string author = "a")
        {
            sbErrors.Clear();

            bool check = true; 
            if (!checkStringWithNumbers(title))
            {
                sbErrors.AppendLine("• Invalid Title Name");
                check = false;
            }
            if (!checkStringWithNumbers(publisherComp))
            {
                sbErrors.AppendLine("• Invalid Company Name");
                check = false;
            }
            if (!checkInt(loanPrice))
            {
                sbErrors.AppendLine("• Invalid Price Number");
                check = false; 
            }
            if (published == new DateTime())
            {
                sbErrors.AppendLine("• No Date Selected");
                check = false;
            }
            if (!checkGenre(g))
            {
                sbErrors.AppendLine("• No Genre Selected");
                check = false;
            }
            if (!checkString(author))
            {
                sbErrors.AppendLine("• Invalid Author Name");
                check = false;
            }

            return check;
        }
        
        public static bool checkGenre(Genre g)
        {
            if (g > 0)
                return true;
            return false;

        }
        public static bool checkString(string name)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z ]+$") || name.Length < 2)
                return false;
            if (!CheckSpaces(name))
                return false;
            return true;
        }
        public static bool checkStringWithNumbers(string name)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9 ]+$") || name.Length < 2)
                return false;
            if (!CheckSpaces(name))
                return false;
            return true;
        }
        public static bool CheckSpaces(string name)
        {
            if (name[0] == ' ' || name[name.Length - 1] == ' ')
                return false;
            for (int i = 0; i < name.Length - 1; i++)
            {
                if (name[i] == ' ' && name[i + 1] == ' ')
                    return false;
            }
            return true;
        }
        public static bool checkInt(string @int)
        {
            if (!double.TryParse(@int, out double p) || p < 0)
                return false;
            return true;

        }



    }
}
