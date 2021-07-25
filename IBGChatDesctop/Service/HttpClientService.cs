﻿using IBChat.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace IBGChatDesctop.Service
{
    public class HttpClientService
    {
        private readonly HttpClient client;

        public HttpClientService()
        {
            client = new HttpClient { Timeout = TimeSpan.FromSeconds(60), BaseAddress = new Uri("https://localhost:44396/api/") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Authorizates user by email and password
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
    }
}
