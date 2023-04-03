using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    [Serializable]
    public class Inbox
    {
        private List<InboxMessage> _messages = new List<InboxMessage>();
        public List<InboxMessage> messages { get { return _messages; } }


        public void SendMessage(Inbox inbox, InboxMessage msg) => inbox.messages.Insert(0, msg);

        #region Delete Messages
        public void DeleteMessage(InboxMessage msg) => messages.Remove(msg);
        
        public void DeleteAll_Messages() => messages.Clear();

        public void DeleteAll_ReadMessages()
        {
            foreach (var msg in Get_ReadMessages())
                messages.Remove(msg);
        }

        public void DeleteAll_UnReadMessages()
        {
            foreach (var msg in Get_UnReadMessages())
                messages.Remove(msg);
        }
        #endregion


        public List<InboxMessage> Get_ReadMessages()
        {
            List<InboxMessage> tempList = new List<InboxMessage>();
            foreach (InboxMessage msg in _messages)
            {
                if (msg.IsRead == true)
                    tempList.Add(msg);
            }
            return tempList;
        }
        public List<InboxMessage> Get_UnReadMessages()
        {
            List<InboxMessage> tempList = new List<InboxMessage>();
            foreach (InboxMessage msg in _messages)
            {
                if (msg.IsRead == false)
                    tempList.Add(msg);
            }
            return tempList;
        }
    }

    [Serializable]
    public class InboxMessage
    {
        #region Properties
        public string sender { get; private set; }
        public string title { get; private set; }
        public string text { get; private set; }
        public DateTime dateTime { get; private set; }
        public bool IsRead { get; private set; }
        #endregion

        public InboxMessage(string sender, string title, string text)
        {
            #region Validation
            if (!ValidInput.checkString(sender))
                throw new MessageException("Invalid Sender Name");
            if (title.Length > 20)
                throw new MessageException("Title can't be over 20 chars");
            string Copytxt = text.Trim();
            if (text.Length < 1 || Copytxt.Length < 1)
                throw new MessageException("Can't Send an empty text");
            #endregion

            #region set Properties
            this.sender = sender;
            this.title = title;
            this.text = text;
            this.dateTime = DateTime.Now;
            this.IsRead = false;
            #endregion
        }
        public override string ToString() => $"{dateTime:g} | From: {sender}\n{(IsRead? "🗹 ": "☐ ")}Title: {title}";
        
        public string showMail()
        {
            if (IsRead == false)
                IsRead = true;
            return $"{dateTime:g} | From: {sender}\nTitle: {title}\n\n{text}";
        }

    }

    [Serializable]
    public class MessageException : Exception
    {
        public MessageException() : base("Error, Invalid Message"){ }
        public MessageException(string message) : base(message) { }
        
    }
}
