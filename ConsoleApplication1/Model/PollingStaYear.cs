using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YdService.Model
{
   public class PollingStaYear
    {
        public int tWidth { set; get; }
        public int id { set; get; }
        public int year { set; get; }
        public int mscid { set; get; }
        public DateTime occurtime { set; get; }
        public List<float> totalColumn { set;get; }
        public List<float> netColumn { set; get; }
        public int fcid { set; get; }
        public byte haveData { set; get; }


        public PollingStaYear()
        {
            tWidth = 12;
            init();
        }

        protected void init() {
            totalColumn = new List<float>();
            netColumn = new List<float>();
            for (int i = 0; i < tWidth; i++)
            {
                totalColumn.Add(-1f);
                netColumn.Add(-1f);
            }
        }
    }
}
