namespace Teledrop.Services
{
    public class RandomService
    {
        private static List<string> _messages = new List<string>
        {
            "❤️", "😘", "💋", "😍", "🥰", "😚", "Live", "Love", "Life", "Freedom"
        };

        public static string GetRandomMessage()
        {
            int n = _messages.Count;

            int random = new Random().Next(n - 1);

            return _messages[random];
        }
    }
}
