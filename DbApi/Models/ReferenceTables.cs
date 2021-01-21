using System;
using System.Collections.Generic;

namespace DbApi.Models
{
    public partial class ReferenceTable
    {
        public long Id { get; set; }
        public string City { get; set; }
        public string Adcode { get; set; }
        public string Citycode { get; set; }

        public void Update(ref ReferenceTable rhs)
        {
            City = rhs.City ?? City;
            Adcode = rhs.Adcode ?? Adcode;
            Citycode = rhs.Adcode ?? Citycode;
        }

    }
}