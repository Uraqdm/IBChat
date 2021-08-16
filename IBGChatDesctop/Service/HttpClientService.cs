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
        private static readonly HttpClient client;

        static HttpClientService()
        {
            client = new HttpClient { Timeout = TimeSpan.FromSeconds(60), BaseAddress = new Uri("https://localhost:44396/api/") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Authorizates user by given email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="psswd">Password</param>
        /// <returns>User with given email and password if operation were succes. Otherwise returns null</returns>
        public async Task<User> AuthorizateUserAsync(string email, string psswd)
        {
            var requestUri = client.BaseAddress + "Users/Auth";
            HttpResponseMessage response = new HttpResponseMessage() ;

            response = await client.PutAsJsonAsync(requestUri, new { Email = email, Password = psswd });
           
            if (response.IsSuccessStatusCode)
            {
                var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                return user;
            }
            else return null;
        }

        /// <summary>
        /// Get user chats from server async.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Return user chats if operation were success or user has any chat. Otherwise return null.</returns>
        public async Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId)
        {
            var requestUrl = client.BaseAddress + $"Chats/UserId/{userId}";

            try
            {
                var response = await client.GetAsync(requestUrl);
                string content;
                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(content = await response.Content.ReadAsStringAsync()))
                {
                    var chats = JsonConvert.DeserializeObject(content, typeof(IEnumerable<Chat>)) as IEnumerable<Chat>;
                    return chats;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    //Set timeout error here
                }
                //Set server error here
            }
            return null;
        }

        /// <summary>
        /// Sends message on server async.
        /// </summary>
        /// <param name="message">Sending message</param>
        /// <returns>if operation were succes it returns Message, delivered on server. Otherwise return null.</returns>
        public async Task<Message> SendMessageAsync(Message message)
        {
            var requestUrl = client.BaseAddress + "Messages/";

            if (message == null) return null;

            try
            {
                var response = await client.PostAsJsonAsync(requestUrl, message);

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(Message)) as Message;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested) { }
                    //handle cancellation

            }

            return null;
        }

        /// <summary>
        /// Add new chat.
        /// </summary>
        /// <param name="chat">Add-on chat.</param>
        /// <returns>Returns add-on chat if operation were succes. Otherwise returns null.</returns>
        public async Task<Chat> AddChatAsync(Guid ownerId, Chat chat)
        {
            var requestUrl = client.BaseAddress + $"Chats/{ownerId}";

            try
            {
                var result = await client.PostAsJsonAsync(requestUrl, chat);

                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync()) as Chat;
                }
            }
            catch (TaskCanceledException) { }
            return null;
        }

        /// <summary>
        /// Add new member on chat
        /// </summary>
        /// <param name="member">New member</param>
        /// <returns>Returns chat for new member if operation were succes. Otherwise returns null</returns>
        public async Task<Chat> AddChatForUserAsync(ChatMember member)
        {
            var requestUrl = client.BaseAddress + "Users/AddChat/";

            try
            {
                var result = await client.PostAsJsonAsync(requestUrl, member);

                if (result.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync()) as Chat;
            }
            catch(TaskCanceledException) { }
            return null;
        }
    }
}
