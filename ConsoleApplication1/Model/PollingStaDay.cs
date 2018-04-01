using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YdService.Model
{
    public class PollingStaDay : PollingStaMonth
    {
        public int day { set; get; }
        public PollingStaDay()
        {
            tWidth = 24;
            init();
        }
    }
}
