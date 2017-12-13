using System.Windows;

namespace FormRender
{
    public static class Misc
    {
#if Local
#if DEBUG
        public const string API = "http://192.168.2.101/api/histo";
        public const string imgPath = "http://192.168.2.101/img/histo/";
#else
        public const string API = "http://10.172.3.106/api/histo";
        public const string imgPath = "http://10.172.3.106/img/histo/";
#endif
#else
        public const string API = "http://190.109.192.61/api/histo";
        public const string imgPath = "http://190.109.192.61/img/histo/";
#endif
    }
    internal enum Language
    {
        Spanish,
        English
    }
    public static class PageSizes
    {
        public static Size Carta => new Size(8.5, 11);
    }
}