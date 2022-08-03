using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bravo_Taksi.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set;}
        private User(){}
        public User(string name, string password, string email, string surname, string phone)
        {
            Name = name;
            Password = password;
            Email = email;
            Surname = surname;
            Phone = phone;
        }
        //public override string ToString() => $"{Email}\n{Password}\n{Name}\n{Surname}\n{Phone}\n";
    }
}
