using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Config.Cloudinary
{
    public class CloudinarySettings
    {
        public string? CloudName { get; set; } 
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
    }
}