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

        public static User CurrentUser { get; set; }

        public ObservableCollection<Chat> Chats { get; private set; }

        #endregion

        #region ctor

        public MainPageViewModel()
        {
            service = new HttpClientService();

            SetUserChats();
        }

        #endregion

        #region methods

        private async void SetUserChats()
        {
            var userChats = await service.GetUserChatsAsync(CurrentUser.Id);

            if (userChats != null)
                Chats = new ObservableCollection<Chat>(userChats);

            else Chats = new ObservableCollection<Chat>();
        }

        #endregion
    }
}
