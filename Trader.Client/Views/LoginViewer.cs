using ReactiveUI;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Trader.Client.Infrastucture;

namespace Trader.Client.Views
{
    public class LoginViewer : ReactiveObject
    {
        private Action<bool> connectEvent;
        private Action<string> loginEvent;
        private Action<string> EnterPressEvent;
        public Visibility AlertVisibility { get; set; }
        public string AlertText { get; set; }
        public string Username { get; set; }
        public string AlertForegroundColor { get; set; }

        public ICommand LoginButtonCommand { get; set; }
        public LoginViewer()
        {


            LoginButtonCommand = new Command(() =>
            {


            });

        }

        private void LoginButtonAction(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            string password = passwordBox.Password;
            string username = Username ?? "";
            string warningText = "";
            if (password.Length == 0)
                warningText = "Password field cannot be empty.";
            if (username.Length == 0)
                warningText = "Username field cannot be empty.";
            if (password.Length == 0 && username.Length == 0)
                warningText = "Password field cannot be empty.\nUsername field cannot be empty";
            if (!warningText.Equals(""))
            {
                setAlert(warningText, "Yellow", Visibility.Visible);
            }
            else
            {
                connectEvent += (bool result) =>
                {
                    if (result)
                    {
                        setAlert("Connection to server succesfully established.", "Green", Visibility.Visible);
                    }
                    else
                    {
                        setAlert("Connection to server could not be established.", "Red", Visibility.Visible);
                    }
                };

                loginEvent += (string result) =>
                {
                    switch (result)
                    {
                        case "AL":
                            setAlert("You are already logged in.", "Red", Visibility.Visible);
                            break;
                        case "SL":
                            setAlert("Logged in succesfully.", "Green", Visibility.Visible);
                            Thread.Sleep(100 * 5);

                            break;
                        case "WC":
                            setAlert("You entered wrong credentials.", "Red", Visibility.Visible);
                            break;
                    }
                };
                // client.Login(username, password);
            }
        }

        private void setAlert(String text, String color, Visibility visibility)
        {
            AlertVisibility = visibility;
            AlertText = text;
            AlertForegroundColor = color;
        }
    }
}
