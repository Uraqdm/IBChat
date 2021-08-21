using IBChat.Domain.Models;
using IBGChatDesctop.Commands;
using IBGChatDesctop.Service;
using IBGChatDesctop.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace IBGChatDesctop.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region fields

        private readonly HttpClientService service;
        private Chat selectedChat;

        #endregion

        #region props

        /// <summary>
        /// Current chat, selected by user.
        /// </summary>
        public Chat SelectedChat
        {
            get => selectedChat;
            set
            {
                selectedChat = value;
                SetSelectedChatsMessages();
                OnPropertyChanged();
            }
        }

        public static User CurrentUser { get; set; }

        public ObservableCollection<Chat> Chats { get; private set; }

        public ObservableCollection<Message> SelectedChatMessages { get; private set; }

        public string SendingMessage { get; set; }

        #endregion

        #region commands

        public ICommand SendMessage { get; }
        public ICommand AddNewChat { get; }
        public ICommand JoinChat { get; }
        public ICommand DeleteChat { get; }
        public ICommand ShareChat { get; }

        #endregion

        #region ctor

        public MainPageViewModel()
        {
            service = new HttpClientService();

            SetUserChats();

            SendMessage = new DelegateCommand(SendMessageToSelectedChatAsync, (obj) => !string.IsNullOrEmpty(SendingMessage) && SelectedChat != null);
            AddNewChat = new DelegateCommand(AddNewChatAsync);
            JoinChat = new DelegateCommand(JoinChatAsync);
            DeleteChat = new DelegateCommand(DeleteChatAsync, (obj) => SelectedChat != null);
            ShareChat = new DelegateCommand(ShareSelectedChat, (obj) => SelectedChat != null);

            AddingChatWindowViewModel.ChatAdded += OnChatAdded;
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

        private async void SetSelectedChatsMessages()
        {
            if (SelectedChat == null) return;

            var messages = await service.GetChatMessagesAsync(SelectedChat.Id);

            if (messages != null)
                SelectedChatMessages = new(messages);
            else
                SelectedChatMessages = new();
        }

        #region command methods

        private async void SendMessageToSelectedChatAsync(object obj)
        {
            var msg = new Message
            {
                ChatId = SelectedChat.Id,
                SenderId = CurrentUser.Id,
                Text = SendingMessage
            };

            SendingMessage = null;

            var sendedMsg = await service.SendMessageAsync(msg);

            if (sendedMsg != null)
                SelectedChatMessages.Add(sendedMsg);
            else
                MessageBox.Show("Unable to send message on server.");
        }

        private void AddNewChatAsync(object obj)
        {
            AddingChatWindowViewModel.AddingChatWindow = new();
            AddingChatWindowViewModel.AddingChatWindow.Activate();
            AddingChatWindowViewModel.AddingChatWindow.Show();
        }

        private void JoinChatAsync(object obj)
        {
            JoiningChatWindowViewModel.ThisWindow = new JoiningInChatWindow();
            JoiningChatWindowViewModel.ThisWindow.Activate();
            JoiningChatWindowViewModel.ThisWindow.Show();
        }

        private async void DeleteChatAsync(object obj)
        {
            await service.DeleteChat(SelectedChat.Id);
            Chats.Remove(SelectedChat);
        }

        private void ShareSelectedChat(object obj)
        {
            Clipboard.SetText(SelectedChat.Id.ToString());
            MessageBox.Show("Link copied to clipboard.");
        }

        #endregion

        #region event methods

        public void OnChatAdded(object sender, Chat addedChat)
        {
            Chats.Add(addedChat);
        }

        #endregion

        #endregion
    }
}
