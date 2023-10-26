using System;
using ChatApp.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Model
{
    class ContactModel
    {
        public string Username { get; set; }
        public string UID { get; set; }
        public List<string> Users { get; set; }
        public string ImageSource { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public string LastMessage => Messages.Last().Message;
        public string LastMessageUser => Messages.Last().Username;


    }
}
