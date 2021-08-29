using IBChat.Domain.Models;
using IBGChatDesctop.Commands;
using IBGChatDesctop.Service;
using IBGChatDesctop.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace IBGChatDesctop.ViewModels
{
    public class JoiningChatWindowViewModel : BaseViewModel
    {
        #region ctor

        public JoiningChatWindowViewModel()
        {
            service = new HttpClientService();

            Submit = new DelegateCommand(SubmitGuid, (obj) => !string.IsNullOrEmpty(ChatGuid));
            Cancel = new DelegateCommand(CancelJoining);
        }

        #endregion

        #region events

        public static event ChatAdded ChatJoined;

        #endregion

        #region fields

        private readonly HttpClientService service;

        #endregion

        #region props

        public static Window ThisWindow { get; set; }

        public string ChatGuid { get; set; }
        public string ErrorMessage { get; private set; }

        #endregion

        #region commands

        public ICommand Submit { get; }
        public ICommand Cancel { get; }

        #endregion

        #region command methods

        /// <summary>
        /// Parse ChatGuid from string to Guid and sends it to server.
        /// </summary>
        /// <param name="obj">plug</param>
        private async void SubmitGuid(object obj)
        {
            Guid guid;

            try
            {
                guid = new Guid(ChatGuid);
            }
            catch(FormatException)
            {
                ErrorMessage = "Incorrect input format";
                return;
            }

            var user = MainPageViewModel.CurrentUser;

            var newChat = await service.AddChatForUserAsync(guid, user);

            if (newChat == null)
            {
                ErrorMessage = "Chat not found. Please, try again";
                return;
            }

            ChatJoined?.Invoke(this, newChat);

            ThisWindow.Visibility = Visibility.Collapsed;
        }

        private void CancelJoining(object obj)
        {
            ThisWindow.Close();
        }

        #endregion
    }
}
