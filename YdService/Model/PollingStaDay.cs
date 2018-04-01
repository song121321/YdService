using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YdService.Model
{
    public class PollingStaDay
    {
        public int id { set; get; }
        public int year { set; get; }
        public int month { set; get; }
        public int day { set; get; }
        public int mscid { set; get; }
        public DateTime occurtime { set; get; }
        public List<float> totalColumn { set;get; }
        public List<float> netColumn { set; get; }

        public PollingStaDay() {
            totalColumn = new List<float>();
            netColumn = new List<float>();
            for (int i = 0; i < 24; i++)
            {
                totalColumn.Add(-1f);
                netColumn.Add(-1f);
            }
        }

    }
}
