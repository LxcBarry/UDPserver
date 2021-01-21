using System;
using System.Collections.Generic;

namespace DbApi.Models
{
    public partial class UserTable
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public DateTime? Updatetime { get; set; }

        public void Update(ref UserTable rhs)
        {
            Username = rhs.Username ?? Username;
            Password = rhs.Password ?? Password;
            Ip = rhs.Ip ?? Ip;
            Updatetime = rhs.Updatetime ?? Updatetime;
        }
    }
}
