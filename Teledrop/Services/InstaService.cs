using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using Teledrop.Exceptions;

namespace Teledrop.Services
{
    public class InstaService
    {
        private IInstaApi _instaApi;

        public async Task<(bool, string)> Login(string username, string password)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.ForUsername(username).WithPassword(password))
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            await _instaApi.SendRequestsBeforeLoginAsync();
            var result = await _instaApi.LoginAsync();

            if (!result.Succeeded || !_instaApi.IsUserAuthenticated)
            {
                throw new InstaException(result.Info.Message);
            }

            await _instaApi.SendRequestsAfterLoginAsync();

            var sessionData = _instaApi.GetStateDataAsString();
            return (true, sessionData);
        }

        public async Task Login(string sessionData)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.Empty)
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            _instaApi.LoadStateDataFromString(sessionData);

            if (!_instaApi.IsUserAuthenticated) throw new InstaException("Not Authorized");
        }

        public async Task SetBio(string mediaId)
        {
            var result = await _instaApi.AccountProcessor.SetBiographyAsync(RandomService.GetRandomMessage());
            if (!result.Succeeded)
            {
                throw new InstaException(result.Info.Message);
            }
        }

        public async Task ChangeProfilePicture(byte[] imageBytes)
        {
            var result = await _instaApi.AccountProcessor.ChangeProfilePictureAsync(imageBytes);

            if (!result.Succeeded)
            {
                throw new InstaException(result.Info.Message);
            }
        }

        public async Task Like(string mediaId)
        {
            var result = await _instaApi.MediaProcessor.LikeMediaAsync(mediaId);
            if (!result.Succeeded)
            {
                throw new InstaException(result.Info.Message);
            }
        }

        public async Task Comment(string mediaId, string text)
        {
            var result = await _instaApi.CommentProcessor.CommentMediaAsync(mediaId, text);

            if (!result.Succeeded)
            {
                throw new InstaException(result.Info.Message);
            }
        }

        public async Task Follow(long userId)
        {
            var result = await _instaApi.UserProcessor.FollowUserAsync(userId);

            if (!result.Succeeded)
            {
                throw new InstaException(result.Info.Message);
            }
        }

        public async Task<long> GetUserIdBy(string username)
        {
            var result = await _instaApi.UserProcessor.GetUserInfoByUsernameAsync(username);

            if (!result.Succeeded)
            {
                throw new InstaException(result.Info.Message);
            }

            return result.Value.Pk;
        }
    }
}
