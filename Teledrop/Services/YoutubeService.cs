using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Teledrop.Configurations;
using Teledrop.Services.Youtube;
using static Google.Apis.YouTube.v3.VideosResource.RateRequest;

namespace Teledrop.Services
{
    public class YoutubeService
    {
        private IClientService _clientService;
        private const string SNIPPET = "snippet";

        private readonly YoutubeConfiguration _configuration;

        public YoutubeService(IOptions<YoutubeConfiguration> configuration)
        {
            _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<(bool, string)> Auth(string accessToken = null)
        {
            var user = "user";
            var isAccessTokenUpdated = false;

            var scope = new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl, YouTubeService.Scope.Youtubepartner };

            var dataStore = new SimpleDataStore();

            var secrets = new ClientSecrets
            {
                ClientId = _configuration.ClientId,
                ClientSecret = _configuration.ClientSecret
            };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scope, user, CancellationToken.None, dataStore);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                isAccessTokenUpdated = true;
            }
            else
            {
                var oldToken = JsonConvert.DeserializeObject<TokenResponse>(accessToken);
                var newToken = await dataStore.GetAsync<TokenResponse>(user);

                //isAccessTokenUpdated = oldToken.IdToken == newToken.IdToken && oldToken.AccessToken == newToken.
            }



            _clientService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Airdropper",
            });

            return (isAccessTokenUpdated, isAccessTokenUpdated ? await dataStore.GetJsonAsync(user) : accessToken);
        }

        public async Task<string> GetMyChannelId()
        {
            var channelsResource = new ChannelsResource(_clientService);

            var request = channelsResource.List(SNIPPET);
            request.Mine = true;

            var result = await request.ExecuteAsync();

            return result.Items[0].Id;
        }

        public async Task Like(string mediaId)
        {
            var videosResource = new VideosResource(_clientService);
            var request = videosResource.Rate(mediaId, RatingEnum.Like);

            await request.ExecuteAsync();
        }

        public async Task Subscribe(string channelId)
        {
            var subscriptionsResource = new SubscriptionsResource(_clientService);

            var subscription = new Subscription
            {
                Kind = "youtube#channel",
                Id = channelId
            };

            var request = subscriptionsResource.Insert(subscription, SNIPPET);
            await request.ExecuteAsync();
        }

        public async Task<string> GetChannelIdBy(string mediaId)
        {
            var videosResource = new VideosResource(_clientService);

            var request = videosResource.List(SNIPPET);
            request.Id = mediaId;

            var result = await request.ExecuteAsync();

            return result.Items[0].Snippet.ChannelId;
        }
    }
}
