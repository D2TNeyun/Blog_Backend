using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Src.Data;

namespace Src.Services
{
    public class StatisticalService(ApplicationDBContext context)
    {
        public readonly ApplicationDBContext _context = context;
        

        
    }
}