using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWappServer.Hubs
{
    public class MyUser
    {
        public string ConnectionId;
        public string Username;

    }


    public static class StaticList
    {

        public static List<MyUser> MyList = new List<MyUser>();
        /*
        static void Add(string text)
        { MyList.Add(text); }
        */

    }
}
