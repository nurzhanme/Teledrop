namespace Teledrop.Models
{
    public class YoutubeAuth
    {
        public int Id { get; set; }
        public string Token { get; set; }


        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
