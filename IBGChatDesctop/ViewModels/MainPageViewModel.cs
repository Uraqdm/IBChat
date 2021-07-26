using IBChat.Domain.Models;
using IBGChatDesctop.Commands;
using IBGChatDesctop.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IBGChatDesctop.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region fields

        private readonly HttpClientService service;

        #endregion

        #region props
        /// <summary>
        /// Current chat, selected by user.
        /// </summary>
        public Chat SelectedChat { get; set; }

        public User CurrentUser { get; set; }

        public ObservableCollection<Chat> Chats { get; set; }
        #endregion

        #region Commands

        public ICommand SelectChat { get; }

        #endregion

        #region Comand methods

        private async void SelectChatAsync(object o)
        {

        }

        #endregion

        #region ctor

        public MainPageViewModel()
        {
            SelectChat = new DelegateCommand(SelectChatAsync, o => SelectedChat != null);
            service = new HttpClientService();

            SetUserChats();
        }

        #endregion

        #region methods

        private async void SetUserChats()
        {
            Chats = new ObservableCollection<Chat>(await service.GetUserChatsAsync(CurrentUser.Id));
        }

        #endregion
    }
}
