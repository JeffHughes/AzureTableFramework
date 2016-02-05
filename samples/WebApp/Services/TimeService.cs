using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class TimeService
    {
        public TimeService()
        {
            Ticks = DateTime.Now.Ticks.ToString();
        }

        public String Ticks { get; set; }
    }
}