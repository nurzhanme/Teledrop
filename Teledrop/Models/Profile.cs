namespace Teledrop.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Account { get; set; }
        public string EvmAddress { get; set; }
        public string YoutubeChannelId { get; set; }
        public string DiscordUsername { get; set; }

        public YoutubeAuth YoutubeAuth { get; set; }
    }
}
