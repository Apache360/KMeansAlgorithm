using System;
using System.Collections.Generic;
using System.Drawing;

namespace KMeansGUI
{
    public static class Misc
    {
        public static Color[] centroidColors;

        static Misc()
        {
            centroidColors = new Color[3];
            centroidColors[0] = Color.Red;
            centroidColors[1] = Color.LimeGreen;
            centroidColors[2] = Color.DodgerBlue;
        }

        public static List<Item> Clone(List<Item> array)
        {
            List<Item> resultList = new List<Item>();
            foreach (Item item in array)
            {
                resultList.Add(new Item(item.id, item.point));
            }
            return resultList;
        }

        public static List<Tuple<double, double>> GetMinMaxPoints(List<Item> dataSet)
        {
            List<Tuple<double, double>> result = new List<Tuple<double, double>>();

            for (int j = 0; j < dataSet[0].point.Length; j++)
            {
                double min = Double.MaxValue;
                double max = Double.MinValue;
                for (int i = 0; i < dataSet.Count; i++)
                {
                    double element = dataSet[i].point[j];
                    if (element < min)
                        min = element;
                    if (element > max)
                        max = element;
                }
                result.Add(new Tuple<double, double>(min, max));
            }
            return result;
        }

        public static double GenerateRandomDouble(Double random, double minimum, double maximum)
        {
            return Math.Round( random,3) * (maximum - minimum) + minimum;
        }
    }
}
