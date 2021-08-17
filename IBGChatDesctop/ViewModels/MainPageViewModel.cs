using IBChat.Domain.Models;
using IBGChatDesctop.Commands;
using IBGChatDesctop.Service;
using IBGChatDesctop.Views;
using System;
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

        public string SendingMessage { get; set; }

        #endregion

        #region commands

        public ICommand SendMessage { get; }
        public ICommand AddNewChat { get; }
        public ICommand JoinChat { get; }
        public ICommand DeleteChat { get; }

        #endregion

        #region command methods

        private async void SendMessageToSelectedChatAsync(object obj)
        {
            var msg = new Message
            {
                Chat = SelectedChat,
                DateTime = DateTime.Now,
                Sender = CurrentUser,
                Text = SendingMessage
            };

            SendingMessage = null;

            var sendedMsg = await service.SendMessageAsync(msg);

            if (sendedMsg != null)
                SelectedChat.Messages.Add(sendedMsg);
            else
                MessageBox.Show("Unable to send message on server.");
        }

        private async void AddNewChatAsync(object obj)
        {
            var chat = new Chat
            {
                Name = "test"
            };

            var newChat = await service.AddChatAsync(CurrentUser.Id, chat);

            if (newChat != null)
                Chats.Add(newChat);
            else
                MessageBox.Show("Unable to add chat.");
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

        #endregion

        #region ctor

        public MainPageViewModel()
        {
            service = new HttpClientService();

            SetUserChats();

            SendMessage = new DelegateCommand(SendMessageToSelectedChatAsync, (obj) => !string.IsNullOrEmpty(SendingMessage) && SelectedChat != null);
            AddNewChat = new DelegateCommand(AddNewChatAsync);
            JoinChat = new DelegateCommand(JoinChatAsync);
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
            var messages = await service.GetChatMessagesAsync(SelectedChat.Id);

            if (messages != null)
                SelectedChat.Messages = new System.Collections.Generic.List<Message>(messages);
            else
                SelectedChat.Messages = new();
        }

        #endregion
    }
}
