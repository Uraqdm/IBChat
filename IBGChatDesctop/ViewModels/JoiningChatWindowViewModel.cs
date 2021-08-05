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

            thisWindow = new JoiningInChatWindow();
        }

        #endregion

        #region fields

        private readonly HttpClientService service;
        private readonly Window thisWindow;

        #endregion

        #region props

        public string ChatGuid { get; set; }
        public string ErrorMessage { get; private set; }

        #endregion

        #region commands

        public ICommand Submit { get; }

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

            var chat = new Chat { Id = guid };
            var user = MainPageViewModel.CurrentUser;

            var newMember = new ChatMember { Chat = chat, User = user };

            var newChat = await service.AddChatForUserAsync(newMember);

            if (newChat == null)
            {
                ErrorMessage = "Chat not found. Please, try again";
                return;
            }

            thisWindow.Close();
        }

        #endregion
    }
}
