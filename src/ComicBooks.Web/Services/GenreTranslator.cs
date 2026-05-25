namespace ComicBooks.Web.Services;

/// <summary>
/// Janr/Teg nomlarini o'zbek tiliga tarjima qilish.
/// DB'da inglizcha saqlanadi, UI'da o'zbekcha ko'rsatiladi.
/// </summary>
public static class GenreTranslator
{
    private static readonly Dictionary<string, string> _map =
        new(StringComparer.OrdinalIgnoreCase)
    {
        // Janrlar
        { "Action",        "Jangari" },
        { "Adventure",     "Sarguzasht" },
        { "Comedy",        "Komediya" },
        { "Drama",         "Drama" },
        { "Fantasy",       "Fantastik olam" },
        { "Horror",        "Qo'rqinchli" },
        { "Isekai",        "Isekai" },
        { "Martial Arts",  "Jang san'ati" },
        { "Murim",         "Murim" },
        { "Mystery",       "Sirli" },
        { "Psychological", "Psixologik" },
        { "Romance",       "Romantika" },
        { "School Life",   "Maktab hayoti" },
        { "Sci-Fi",        "Ilmiy fantastika" },
        { "Supernatural",  "G'ayritabiiy" },
        { "Thriller",      "Triller" },

        // Teglar
        { "Cultivation",   "Kultivatsiya" },
        { "Dungeon",       "Zindon" },
        { "Overpowered",   "Juda kuchli" },
        { "Regression",    "O'tmishga qaytish" },
        { "Reincarnation", "Qayta tug'ilish" },
        { "Strong MC",     "Kuchli bosh qahramon" },
        { "System",        "Tizim" },
    };

    public static string Translate(string english)
    {
        if (string.IsNullOrWhiteSpace(english)) return english ?? "";
        return _map.TryGetValue(english.Trim(), out var uz) ? uz : english;
    }

    public static IEnumerable<string> TranslateMany(IEnumerable<string>? items)
        => (items ?? Enumerable.Empty<string>()).Select(Translate);
}
