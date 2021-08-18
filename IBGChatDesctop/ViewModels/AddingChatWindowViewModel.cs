using IBChat.Domain.Models;
using IBGChatDesctop.Commands;
using IBGChatDesctop.Service;
using IBGChatDesctop.Views;
using System.Windows.Input;

namespace IBGChatDesctop.ViewModels
{
    public class AddingChatWindowViewModel : BaseViewModel
    {
        #region fields

        private readonly HttpClientService service;

        #endregion

        #region ctor

        public AddingChatWindowViewModel()
        {
            service = new HttpClientService();

            Submit = new DelegateCommand(AddNewChat, (obj) => !string.IsNullOrEmpty(Name));
        }

        #endregion

        #region props

        public string Name { get; set; }

        public string ErrorMessage { get; set; }

        public static AddingChatWindow AddingChatWindow { get; set; }

        #endregion

        #region commadns

        public ICommand Submit { get; set; }

        #endregion

        #region command methods

        private async void AddNewChat(object obj)
        {
            var newChat = new Chat { Name = Name };

            var result = await service.AddChatAsync(MainPageViewModel.CurrentUser.Id ,newChat);

            if (result == null)
                ErrorMessage = "Unable to add chat. Please, try again later. It may cause of some server errors";

            else
            {
                MainPageViewModel.Chats.Add(result);
                AddingChatWindow.Close();
            }
               
        }

        #endregion
    }
}
