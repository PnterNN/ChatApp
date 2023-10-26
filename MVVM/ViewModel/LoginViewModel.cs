using ChatApp.Core;
using ChatApp.MVVM.Model;
using ChatApp.NET;
using System;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    class LoginViewModel : ObservableObject
    {

        public static SqlServer SqlServer;

        private string _error;
        public string Error 
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged();
            }
        }

        private string _username;
        public string Username 
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoginToServer { get; set; }

        private void isValidUser()
        {

            var isvaliduser = SqlServer.PacketReader.ReadMessage();

            if (isvaliduser == "True")
            {
                Error = "Kullanıcı Bulundu";
            }
            else
            {
                Error = "Kullanıcı bulunamadı";
                SqlServer._sqlclient.Close();
                SqlServer = new SqlServer();
            }
        }

        public LoginViewModel()
        {
            SqlServer = new SqlServer();
            LoginToServer = new RelayCommand(o =>
            {
                if(_username != null && _password != null)
                {
                    if (_username.Length > 3 && _password.Length > 3)
                    {
                        
                        SqlServer.loginToSqlServer(_username, _password);
                    }
                    else
                    {
                        Error = "Kullanıcı adı ya da şifre 3 harfden fazla olmalı!";
                    }
                }
                else
                {
                    Error = "Kullanıcı adı ya da şifre boş bırakılamaz!";
                }
            });
            SqlServer.isValidUserEvent += isValidUser;
        }
    }
}
