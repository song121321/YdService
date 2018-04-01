using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YdService.Model
{
   public class PollingStaMonth : PollingStaYear
    {
        public int month { set; get; }

        public PollingStaMonth()
        {
            tWidth = 32;
            init();
        }
    }
}
