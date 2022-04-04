using Microsoft.Extensions.Options;
using TdLib;
using Teledrop.Configurations;
using Teledrop.Enums;
using static TdLib.TdApi;
using static TdLib.TdApi.InputChatPhoto;
using static TdLib.TdApi.InputFile;

namespace Teledrop.Services
{
    public class TelegramService
    {
        private readonly TdClient _client;
        private readonly TelegramConfiguration _configuration;
        private const string OK_Response = "ok";

        public TelegramService(IOptions<TelegramConfiguration> configuration)
        {
            _client = new TdClient();

            _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration)); 
        }

        public async Task<TelegramAuthorizationState> Auth(string phonenumber)
        {
            await _client.SetTdlibParametersAsync(new TdlibParameters
            {
                ApiId = _configuration.AppId,
                ApiHash = _configuration.ApiHash,
                ApplicationVersion = _configuration.ApplicationVersion,
                DeviceModel = _configuration.DeviceModel,
                SystemLanguageCode = _configuration.SystemLanguageCode,
                SystemVersion = _configuration.SystemVersion,
                DatabaseDirectory = $"{phonenumber}"

            });

            await _client.CheckDatabaseEncryptionKeyAsync(null);

            var authState = await _client.GetAuthorizationStateAsync();

            if (authState.GetType() == typeof(AuthorizationState.AuthorizationStateWaitPhoneNumber))
            {
                await _client.SetAuthenticationPhoneNumberAsync(phonenumber);
                return TelegramAuthorizationState.WaitCode;
            }

            if (authState.GetType() == typeof(AuthorizationState.AuthorizationStateWaitCode))
            {
                return TelegramAuthorizationState.WaitCode;
            }

            if (authState.GetType() == typeof(AuthorizationState.AuthorizationStateReady))
            {
                return TelegramAuthorizationState.Ready;
            }

            return TelegramAuthorizationState.Unknown;
        }

        public async Task<bool> EnterCode(string phonenumber, string code)
        {
            var authState = await Auth(phonenumber);

            if (authState == TelegramAuthorizationState.Ready)
            {
                return true;
            }

            if (authState == TelegramAuthorizationState.WaitCode)
            {
                var res = await _client.CheckAuthenticationCodeAsync(code);

                return string.Compare(res.DataType, OK_Response) == 0;
            }

            return false;
        }

        public async Task<bool> JoinChat(string phonenumber, string chatName)
        {
            await Auth(phonenumber);

            var checkChatInviteInfo = await _client.SearchPublicChatAsync(chatName);

            var chatId = checkChatInviteInfo.Id;

            var joinInfo = await _client.JoinChatAsync(chatId);

            return string.Compare(joinInfo.DataType, OK_Response) == 0;
        }

        public async Task<bool> SetUsername(string phonenumber)
        {
            await Auth(phonenumber);

            var username = $"t{phonenumber.Substring(2)}";
         
            var usernameResult = await _client.SetUsernameAsync(username);
          
            return string.Compare(usernameResult.DataType, OK_Response) == 0;
        }

        public async Task<bool> SetBio(string phonenumber)
        {
            await Auth(phonenumber);
                        
            var bioResult = await _client.SetBioAsync(RandomService.GetRandomMessage());

            return string.Compare(bioResult.DataType, OK_Response) == 0;
        }

        public async Task<bool> SetProfilePicture(string phonenumber, byte[] byteArray)
        {
            await Auth(phonenumber);

            var filename = "image.jpg";

            await System.IO.File.WriteAllBytesAsync(filename, byteArray);


            var inputFileLocal = new InputFileLocal();
            inputFileLocal.Path = filename;

            var inputChatPhoto = new InputChatPhotoStatic();
            inputChatPhoto.Photo = inputFileLocal;

            var res = await _client.SetProfilePhotoAsync(inputChatPhoto);

            System.IO.File.Delete(filename);

            return string.Compare(res.DataType, OK_Response) == 0;
        }
    }
}
