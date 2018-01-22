using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Model.Entities
{
    class PointRelation
    {
        private int analogNo;
        private int parentNo;
        private IList<int> sonList;
        PointRelation(int analogNo , int parentNo , IList<int> sonList)
        {
            this.analogNo = analogNo;
            this.parentNo = parentNo;
            this.sonList = new List<int>();
            for (int i = 0; i < sonList.Count; i++)
            {
                this.sonList.Add(sonList[i]);
            }
        }

        public int AnalogNo
        {
            get
            {
                return analogNo;
            }

            set
            {
                analogNo = value;
            }
        }

        public int ParentNo
        {
            get
            {
                return parentNo;
            }

            set
            {
                parentNo = value;
            }
        }

        public IList<int> SonList
        {
            get
            {
                return sonList;
            }

            set
            {
                sonList = value;
            }
        }
    }
}
