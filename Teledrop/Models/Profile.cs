namespace Teledrop.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public byte[] ProfileImage { get; set; }
        public string ProfileImageBase64 { get; set; }
        public string Email { get; set; }
        public string Account { get; set; }
        public string EvmAddress { get; set; }
    }
}
