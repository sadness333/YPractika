using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPractika
{
    public class Client
    {
        public static int UserId;

        private string Fullname { get; set; }
        public string Role { get; }
        private string Email { get; set; }

        public Client(int userid,string fullname, string role, string email)
        {
            UserId = userid;
            Fullname=fullname;
            Role = role;
            Email = email;
        }
    }
}
