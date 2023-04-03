using System;
using System.Linq;
using System.Threading.Tasks;

namespace Logic
{
    [Serializable]
    public class Book : Item, IAuthorable
    {
        public string Author { get; set; }

        public Book(string author, string title, string publisherComp, double loanPrice, DateTime published, Genre genre)
            : base(title, publisherComp, loanPrice, published, genre)
        { Author = author; }


        public override string info() => $"Author: {Author}\n" + base.info();

        public override Book Clone()
        {
            Book temp = new Book(this.Author,this.ItemTitle, this.PublisherComp, this.loanPrice, this.published, this.genre);
            temp.Loan(new DateTime());
            return temp;
        }
    }
}
