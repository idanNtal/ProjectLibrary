using System;
using System.Diagnostics;

namespace Logic
{
    [Serializable]
    public class Jurnal : Item
    {
        public Jurnal(string title, string publisherComp, double loanPrice, DateTime published, Genre genre)
            : base(title, publisherComp, loanPrice, published, genre)
        { }

        public override Jurnal Clone()
        {
            Jurnal temp = new Jurnal(this.ItemTitle, this.PublisherComp, this.loanPrice, this.published, this.genre);
            temp.Loan(new DateTime());
            return temp;
        }
    }
}
