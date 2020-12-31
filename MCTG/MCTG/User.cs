using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    public class User
    {
        private string username;
        private string password;
        private string name;
        private string bio;
        private string image;
        public User() { }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        public string Bio
        {
            get { return bio; }
            set { bio = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
