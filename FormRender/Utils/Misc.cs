using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormRender
{
    public static class Misc
    {
#if DEBUG
        public const string API = "http://192.168.2.101/api/histo";
        public const string imgPath = "http://192.168.2.101/img/histo/";
#else
        public const string API = "http://190.109.192.61/api/histo";
        public const string imgPath = "http://190.109.192.61/img/histo/";
#endif
    }
}