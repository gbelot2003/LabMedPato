using System.Windows;

namespace FormRender
{
    public static class Config
    {
#if Local
#if DEBUG
        public const string API = "http://192.168.2.101/api/histo";
        public const string imgPath = "http://192.168.2.101/img/histo/";
        public const string usrPath = "http://192.168.2.101/api/user/";
#else
        public const string API = "http://10.172.3.106/api/histo";
        public const string imgPath = "http://10.172.3.106/img/histo/";
        public const string usrPath = "http://10.172.3.106/api/user/";
#endif
#else
        public const string API = "http://190.109.192.61/api/histo";
        public const string imgPath = "http://190.109.192.61/img/histo/";
        public const string usrPath = "http://190.109.192.61/api/user/";
#endif
    }
    public enum Language
    {
        Spanish,
        English
    }
    public static class PageSizes
    {
        public static Size Carta => new Size(8.5, 11);
    }
}