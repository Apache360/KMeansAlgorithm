using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeansGUI
{
    class Item
    {
        public string id;
        public double[] point;

        public Item(string id, double[] point)
        {
            this.id = id;
            this.point = point;
        }
    }
}
