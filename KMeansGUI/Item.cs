using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeansGUI
{
    public class Item
    {
        public string id;
        public double[] point;

        public Item(string id, double[] point)
        {
            this.id = id;
            this.point = point;
        }

        public override string ToString()
        {
            string str=id+": ";
            for (int i = 0; i < point.Length; i++)
            {
                str += point[i].ToString()+" ";
            }
            return str;
        }
    }
}
