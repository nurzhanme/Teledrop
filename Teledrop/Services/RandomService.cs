namespace Teledrop.Services
{
    public class RandomService
    {
        private static List<string> _messages = new List<string>
        {
            "❤️", "😘", "💋", "😍", "🥰", "😚", "Live", "Love", "Life", "Freedom"
        };

        private static List<string> _names = new List<string>
        {
            "Sasha",
            "Alex",
            "Riley",
            "Jessie",
            "Marion",
            "Jackie",
            "Alva",
            "Ollie",
            "Jodie",
            "Cleo",
            "Kerry",
            "Frankie",
            "Guadalupe",
            "Carey",
            "Tommie",
            "Angel",
            "Hollis",
            "Sammie",
            "Jamie",
            "Kris",
            "Robbie",
            "Tracy",
            "Merrill",
            "Noel",
            "Rene",
            "Johnnie",
            "Ariel",
            "Jan",
            "Casey",
            "Jackie",
            "Kerry",
            "Jodie",
            "Finley",
            "Skylar",
            "Justice",
            "Rene",
            "Darian",
            "Frankie",
            "Oakley",
            "Robbie",
            "Remy",
            "Milan",
            "Jaylin",
            "Devan",
            "Armani",
            "Charlie",
            "Stevie",
            "Channing",
            "Gerry",
            "Monroe",
            "Kirby",
            "Azariah",
            "Santana"
        };

        public static string GetRandomMessage()
        {
            int n = _messages.Count;

            int random = new Random().Next(n - 1);

            return _messages[random];
        }

        public static string GetRandomName()
        {
            int n = _names.Count;

            int random = new Random().Next(n - 1);

            return _names[random];
        }
    }
}
