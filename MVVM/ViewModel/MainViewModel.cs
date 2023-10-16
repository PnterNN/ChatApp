using ChatApp.Core;
using ChatApp.MVVM.Model;
using ChatApp.NET;
using ChatServer.NET.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }

        public RelayCommand SendMessageCommand { get; set; }

        public RelayCommand CallCommand { get; set; }

        public RelayCommand CameraCommand { get; set; }

        private const string V = "";
        private Server _server;
        public String Username { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }

        private ContactModel _selectedContact;
        public ContactModel SelectedContact
        {
            get { return _selectedContact;
            }
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }


        private void UserConnected()
        {
            string username = _server.PacketReader.ReadMessage();
            string uid = _server.PacketReader.ReadMessage();
            var user = new ContactModel
            {
                Username = username,
                ImageSource = "CornflowerBlue",
                UID = uid,
                Messages = new ObservableCollection<MessageModel>()
            };
            user.Messages.Add(new MessageModel()
            {
                Username = username,
                UsernameColor = "#409aff",
                ImageSource = "",
                Message = username + " çevrimiçi oldu",
                Time = DateTime.Now,
                IsNativeOrigin = true,
                FirstMessage = true
            });
            if (!Contacts.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Contacts.Add(user));
            }
        }
        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            var username = _server.PacketReader.ReadMessage();
            var sendedUserUID = _server.PacketReader.ReadMessage();

            var contact = Contacts.Where(x => x.UID == sendedUserUID).FirstOrDefault();
            if (contact != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    contact.Messages.Add(new MessageModel
                    {
                        Username = username,
                        UsernameColor = "#409aff",
                        ImageSource = "",
                        Message = msg,
                        Time = DateTime.Now,
                        IsNativeOrigin = false,
                        FirstMessage = true
                    });
                });
                if (_selectedContact != contact)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer("./Sounds/notification.wav");
                    player.Play();
                }
            }
        }

        private void UserDisconnected()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Contacts.FirstOrDefault(x => x.UID == uid);
            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() => Contacts.Remove(user));
            }
        }

        private void sendCommand(string[] args)
        {
            //Create a group named args[2] with usernames args[3-infinity]
            if (args[0].ToLower() == "/group" && args[1].ToLower() == "create" || args[2] != null)
            {
                try
                {
                    string invaildusers = "";
                    string users = "";
                    for (int i = 0; i < args.Length - 3; i++)
                    {
                        if (args[i + 3] != null)
                        {
                            var user = Contacts.Where(x => x.Username.ToLower() == args[i + 3].ToLower()).FirstOrDefault();
                            if (user != null)
                            {
                                users += args[i + 3].ToLower() + " ";
                            }
                            else
                            {
                                invaildusers += args[i + 3] + " ";
                            }
                        }
                    }
                    users = users.TrimEnd(' ');
                    invaildusers = invaildusers.TrimEnd(' ');
                    if (invaildusers == "")
                    {
                        _server.createGroup(args[2], users);
                    }
                    else
                    {
                        MessageBox.Show($"Kullanıcı Bulunamadığı için grup oluşturalamadı, bulanamayan kullanıcılar: ({invaildusers})");
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private void createGroup()
        {
            var groupName = _server.PacketReader.ReadMessage();
            var userUIDS = _server.PacketReader.ReadMessage();
            List<string> uids = userUIDS.Split(' ').ToList();
            List<string> usernames = new List<string>();
            foreach(string uid in uids)
            {
                if (Contacts.Where(x => x.UID == uid).FirstOrDefault() != null)
                {
                    usernames.Add(Contacts.Where(x => x.UID == uid).FirstOrDefault().Username);
                }
            }
            var user = new ContactModel
            {
                Username = groupName,
                ImageSource = "CornflowerBlue",
                UID = userUIDS,
                Users = uids,
                Messages = new ObservableCollection<MessageModel>()
            };
            user.Messages.Add(new MessageModel()
            {
                Username = groupName,
                UsernameColor = "#409aff",
                ImageSource = "",
                Message = "Uye Listesi: " + string.Join(" ", usernames),
                Time = DateTime.Now,
                IsNativeOrigin = true,
                FirstMessage = true
            });
            Application.Current.Dispatcher.Invoke(() => Contacts.Add(user));
        }

        public MainViewModel()
        {
            Contacts = new ObservableCollection<ContactModel>();
            Messages = new ObservableCollection<MessageModel>();

            CallCommand = new RelayCommand(o =>
            {
                MessageBox.Show(Username + "-> " + _selectedContact.Username + " Arama açılıyor...");
            });

            CameraCommand = new RelayCommand(o =>
            {
                MessageBox.Show(Username + "-> " + _selectedContact.Username + " Kameralı arama açılıyor...");
            });

            SendMessageCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    if (_selectedContact != null)
                    {
                        if (_selectedContact.Users == null)
                        {
                            SelectedContact.Messages.Add(new MessageModel
                            {
                                Username = Username,
                                UsernameColor = "#409aff",
                                ImageSource = "",
                                Message = Message,
                                Time = DateTime.Now,
                                IsNativeOrigin = true,
                                FirstMessage = true
                            });
                            _server.SendMessageToServer(Message, SelectedContact.UID);
                        }
                        else
                        {
                            string users = "";
                            foreach(string userıd in SelectedContact.Users)
                            {
                                users += userıd + " ";
                            }
                            users.TrimEnd(' ');
                            _server.SendMessageToGroup(Message, SelectedContact.UID);
                        }
                }
                else
                {
                    string[] args = Message.Split(' ');
                    sendCommand(args);
                }
                Message = "";
            }
            });

            _server = new Server();

            _server.connectedEvent += UserConnected;
            _server.messageReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += UserDisconnected;
            _server.groupCreatedEvent += createGroup;

            ConnectToServerCommand = new RelayCommand(o =>
            {
                _server.ConnectToServer(Username);
            });
        }
    }
}
