using System;
using System.Collections.Generic;

namespace DbApi.Models
{
    public partial class WeatherTable
    {
        public int? Id { get; set; }
        public string Site { get; set; }
        //[Newtonsoft.Json.JsonIgnore]
        public string Content { get; set; }
        public DateTime? Updatetime { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public void Update(ref WeatherTable rhs)
        {
            Site = rhs.Site??Site;
            Content = rhs.Content??Content;
            Updatetime = rhs.Updatetime ?? Updatetime;
            Province = rhs.Province ?? Province;
            City = rhs.City ?? City;
        }
    }
}
