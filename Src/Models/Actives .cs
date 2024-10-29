using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models
{
    public class Actives 
    {
         public int ActivesID { get; set; }
        public string? AppUserID  { get; set; }
        public string? StatusName { get; set; }
        public string? Avata  { get; set; }

        //relationShip
        public AppUser? AppUser { get; set; }
    }
}