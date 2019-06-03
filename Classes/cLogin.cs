using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAdvRoomFutureVacancy.Classes
{
    public class cLogin
    {
    }
    public class LoginCamelot
    {

        public string success { get; set; }
        public string message { get; set; }
        public string sessionID { get; set; }
        public string language { get; set; }
        public string labellanguage { get; set; }
        public string error { get; set; }
        public string errorId { get; set; }
        public string fullName { get; set; }
        public string role { get; set; }
        public string contact { get; set; }
        public string RLS_WHERE { get; set; }
        public string userEmail { get; set; }
    }

    public class userLogin
    {

        public string method { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class userLogout
    {
        public string method { get; set; }
        public string sessionID { get; set; }
    }
}
