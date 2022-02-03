﻿using Microsoft.Extensions.Options;
using TdLib;
using Teledrop.Configurations;
using Teledrop.Enums;
using static TdLib.TdApi;

namespace Teledrop.Services
{
    public class Telegram
    {
        private TdClient _client;
        private TelegramConfiguration _configuration;
        private string OK_Response = "ok";

        public Telegram(IOptions<TelegramConfiguration> configuration)
        {
            _client = new TdClient();

            _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration)); 
        }

        public async Task<AuthorizationStateEnum> Auth(string phonenumber)
        {
            await _client.SetTdlibParametersAsync(new TdlibParameters
            {
                ApiId = _configuration.AppId,
                ApiHash = _configuration.ApiHash,
                ApplicationVersion = _configuration.ApplicationVersion,
                DeviceModel = _configuration.DeviceModel,
                SystemLanguageCode = _configuration.SystemLanguageCode,
                SystemVersion = _configuration.SystemVersion,
                DatabaseDirectory = $"db/{phonenumber}"

            });

            await _client.CheckDatabaseEncryptionKeyAsync(null);

            var authState = await _client.GetAuthorizationStateAsync();

            if (authState.GetType() == typeof(AuthorizationState.AuthorizationStateWaitPhoneNumber))
            {
                Ok ok = await _client.SetAuthenticationPhoneNumberAsync(phonenumber);
                return AuthorizationStateEnum.WaitCode;
            }
            else if (authState.GetType() == typeof(AuthorizationState.AuthorizationStateReady))
            {
                return AuthorizationStateEnum.Ready;
            }

            return AuthorizationStateEnum.Unknown;
        }

        public async Task<bool> EnterCode(string phonenumber, string code)
        {
            var authState = await Auth(phonenumber);

            if (authState == AuthorizationStateEnum.Ready)
            {
                return true;
            }

            if (authState == AuthorizationStateEnum.WaitCode)
            {
                var res = await _client.CheckAuthenticationCodeAsync(code);

                return string.Compare(res.DataType, OK_Response) == 0;
            }

            return false;
        }

        public async Task<bool> JoinChat(string phonenumber, string chatName)
        {
            var authState = await Auth(phonenumber);

            var checkChatInviteInfo = await _client.SearchPublicChatAsync(chatName);

            var chatId = checkChatInviteInfo.Id;

            var joinInfo = await _client.JoinChatAsync(chatId);

            return string.Compare(joinInfo.DataType, OK_Response) == 0;
        }
    }
}
