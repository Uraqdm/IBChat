using IBChat.Domain.Models;
using IBGChatDesctop.Commands;
using IBGChatDesctop.Service;
using IBGChatDesctop.Views;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IBGChatDesctop.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        #region fields

        private readonly HttpClientService client = new HttpClientService();
        
        #endregion


        #region props

        public string Password { get; set; }
        public string Email { get; set; }

        public User User { get; set; }
        #endregion

        #region commands

        public ICommand Authorizate { get; }

        #endregion

        #region command methods

        private async void AuthorizateUser (object obj)
        {
            try
            {
                User = await client.AuthorizateUserAsync(Email, Password);

                if (User != null)
                    NavigationHandler.NavigationService.Navigate(new MainPage());

                else
                    MessageBox.Show("User with this email and password not found. Please, try again.");
            }
            catch(Exception)
            {
                MessageBox.Show("Unable to connect with the server.");
            }

            

        }

        #endregion

        #region ctor

        public AuthorizationViewModel()
        {
            Authorizate = new DelegateCommand(AuthorizateUser, (a) => !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email));
        }

        #endregion
    }
}
