using IBChat.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IBGChatDesctop.Service
{
    public class HttpClientService
    {
        #region fields

        private static readonly HttpClient client;

        #endregion

        #region ctor

        static HttpClientService()
        {
            client = new HttpClient { Timeout = TimeSpan.FromSeconds(60), BaseAddress = new Uri("https://localhost:44396/api/") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Get

        /// <summary>
        /// Get user chats from server async.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Return user chats if operation were success or user has any chat. Otherwise return null.</returns>
        public async Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId)
        {
            var requestUrl = client.BaseAddress + $"Users/UserChats/{userId}";

            return await GetDataAsync<IEnumerable<Chat>>(requestUrl);
        }

        /// <summary>
        /// Gets chat messages from server async.
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <returns>If operation were success returns sequence of messages. Otherwise returns null.</returns>
        public async Task<IEnumerable<Message>> GetChatMessagesAsync(Guid chatId)
        {
            var requestUrl = client.BaseAddress + $"Messages/Chat/{chatId}";

            return await GetDataAsync<IEnumerable<Message>>(requestUrl);
        }

        /// <summary>
        /// Get generic data from server async
        /// </summary>
        /// <typeparam name="T">Type of requiered data</typeparam>
        /// <param name="requestUrl">Request url</param>
        /// <returns>Data type of T</returns>
        private async Task<T> GetDataAsync<T>(string requestUrl) where T : class
        {
            try
            {
                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var result = JsonConvert.DeserializeObject(content, typeof(T)) as T;
                        return result;
                    }
                }
            }
            catch (TaskCanceledException) { }

            return null;
        }

        #endregion

        #region Post

        /// <summary>
        /// Sends message on server async.
        /// </summary>
        /// <param name="message">Sending message</param>
        /// <returns>if operation were succes it returns Message, delivered on server. Otherwise return null.</returns>
        public async Task<Message> SendMessageAsync(Message message)
        {
            var requestUrl = client.BaseAddress + "Messages/";

            if (message == null) return null;

            var result = await PostDataAsync<Message>(requestUrl, message);

            return result;
        }

        /// <summary>
        /// Add new chat.
        /// </summary>
        /// <param name="chat">Add-on chat.</param>
        /// <returns>Returns add-on chat if operation were succes. Otherwise returns null.</returns>
        public async Task<Chat> AddChatAsync(Guid ownerId, Chat chat)
        {
            var requestUrl = client.BaseAddress + $"Chats/{ownerId}";

            var result = await PostDataAsync<Chat>(requestUrl, chat);

            return result;
        }

        /// <summary>
        /// Add new member on chat
        /// </summary>
        /// <param name="member">New member</param>
        /// <returns>Returns chat for new member if operation were succes. Otherwise returns null</returns>
        public async Task<Chat> AddChatForUserAsync(ChatMember member)
        {
            var requestUrl = client.BaseAddress + "Chats/AddMember/";

            return await PostDataAsync<Chat>(requestUrl, member);
        }

        private async Task<T> PostDataAsync<T>(string requestUrl, object data) where T : class
        {
            try
            {
                var response = await client.PostAsJsonAsync(requestUrl, data);

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(T)) as T;
                }
            }
            catch (TaskCanceledException) { }

            return null;
        }

        #endregion

        /// <summary>
        /// Authorizates user by given email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="psswd">Password</param>
        /// <returns>User with given email and password if operation were succes. Otherwise returns null</returns>
        public async Task<User> AuthorizateUserAsync(string email, string psswd)
        {
            var requestUri = client.BaseAddress + "Users/Auth";

            var response = await client.PutAsJsonAsync(requestUri, new { Email = email, Password = psswd });
           
            if (response.IsSuccessStatusCode)
            {
                var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                return user;
            }
            else return null;
        }

        public async Task<bool> DeleteChat(Guid chatId)
        {
            var requestUrl = client.BaseAddress + $"Chats/{chatId}";

            var result = await client.DeleteAsync(requestUrl);

            return result.IsSuccessStatusCode;
        }
    }
}
