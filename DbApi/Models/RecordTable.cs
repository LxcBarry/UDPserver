using System;
using System.Collections.Generic;

namespace DbApi.Models
{
    public partial class RecordTable
    {
        public long? Id { get; set; }
        public string Username { get; set; }
        public string Site { get; set; }
        public DateTime? Updatetime { get; set; }

        public void Update(ref RecordTable rhs)
        {
            Username = rhs.Username??Username;
            Site = rhs.Site??Site;
            Updatetime = rhs.Updatetime??Updatetime;
        }
    }
}
