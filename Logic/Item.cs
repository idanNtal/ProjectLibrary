using System;
using System.Reflection;

namespace Logic
{
    [Serializable]
    public abstract class Item
    {
        public Item(string title, string publisher, double loanPrice, DateTime published, Genre genre)
        {
            this.Id = Guid.NewGuid();
            this.ItemTitle = title;
            this.PublisherComp = publisher;
            this.loanPrice = loanPrice;
            this.published = published;
            this.genre = genre;
        }

        #region Properties
        public Guid Id { get; private set; }
        public string ItemTitle { get; private set; }
        public string PublisherComp { get; private set; }
        public double loanPrice { get; private set; }
        public DateTime published { get; private set; }
        public Genre genre { get; private set; }
        public DateTime loanDate { get; private set; }
        #endregion

        #region Display
        public override string ToString() => (loanDate != new DateTime() ? "® " : "") + $"({this.GetType().Name}) {ItemTitle}";

        public virtual string info() => $"Title: {ItemTitle}\nGenre: {genre}\nPublisher: {PublisherComp}\n" +
            $"Loan Price: {loanPrice:c}\nPublished: {published:d}\n" +
            (loanDate!=new DateTime()? $"Loan Date: {loanDate:d}":"");
        #endregion 

        #region Change Properties
        public void Loan(DateTime Loan) => loanDate = Loan;
        public void ChangeTitle(string title) => this.ItemTitle = title;
        public void ChangePublisherComp(string publisherComp) => this.PublisherComp = publisherComp;
        public void ChangeLoanPrice(double price) => this.loanPrice = price;
        public void ChangePublishedDate(DateTime date) => this.published = date;
        public void ChangeGenre(Genre genre) => this.genre = genre;
        #endregion

        public abstract Item Clone();
    }
}
