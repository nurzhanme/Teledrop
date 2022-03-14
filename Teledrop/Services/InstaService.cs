using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;

namespace Teledrop.Services
{
    public class InstaService
    {
        private IInstaApi _instaApi;

        public async Task<string> Login(string username, string password)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.ForUsername(username).WithPassword(password))
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            await _instaApi.SendRequestsBeforeLoginAsync();
            var result = await _instaApi.LoginAsync();

            if (!result.Succeeded)
            {
                if (result.Info.ResponseType == ResponseType.CheckPointRequired
                    || result.Info.ResponseType == ResponseType.ChallengeRequired
                    || result.Info.Message == "Challenge is required")
                {
                    throw new Exception("Инста просит проверку логина " + username);
                }

                throw new Exception("не получилось авторизоваться" + result.Info.Message);
            }

            if (!_instaApi.IsUserAuthenticated)
            {
                throw new AccessViolationException(result.Info?.Message);
            }

            await _instaApi.SendRequestsAfterLoginAsync();

            var sessionData = _instaApi.GetStateDataAsString();
            return sessionData;
        }

        public async Task Login(string sessionData)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.Empty)
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            _instaApi.LoadStateDataFromString(sessionData);

            if (!_instaApi.IsUserAuthenticated) throw new AccessViolationException("Не авторизовался");
        }

        public async Task SetBio(string mediaId)
        {
            var result = await _instaApi.AccountProcessor.SetBiographyAsync(RandomService.GetRandomMessage());
            if (!result.Succeeded)
            {
                throw new Exception(result.Info.Message);
            }
        }

        public async Task Like(string mediaId)
        {
            var result = await _instaApi.MediaProcessor.LikeMediaAsync(mediaId);
            if (!result.Succeeded)
            {
                throw new Exception(result.Info.Message);
            }
        }

        public async Task Comment(string mediaId, string text)
        {
            var result = await _instaApi.CommentProcessor.CommentMediaAsync(mediaId, text);

            if (!result.Succeeded)
            {
                throw new Exception(result.Info.Message);
            }
        }

        public async Task Follow(long userId)
        {
            var result = await _instaApi.UserProcessor.FollowUserAsync(userId);

            if (!result.Succeeded)
            {
                throw new Exception(result.Info.Message);
            }
        }

        public async Task<long> GetUserIdBy(string username)
        {
            var result = await _instaApi.UserProcessor.GetUserInfoByUsernameAsync(username);

            if (!result.Succeeded)
            {
                throw new Exception(result.Info.Message);
            }

            return result.Value.Pk;
        }
    }
}
