using KMeansGUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KMeansGUI
{
    public class Centroid
    {
        public double[] array;
        private List<Item> _oldItemsList;
        public List<Item> closestItemsList;
        public Color color;
        private static Random random = new Random();
        private KmeansForm kmeansForm = new KmeansForm();

        public void DrawMe(PaintEventArgs e)
        {
            KmeansForm form = new KmeansForm();
            Graphics g = e.Graphics;
            form.drawLegend();

            foreach (Item item in closestItemsList)
            {
                g.DrawEllipse(new Pen(color, 2.0f), (float)item.point[0], kmeansForm.getHeight()-10 - (float)item.point[1], 6, 6);
            }
            g.FillEllipse(new SolidBrush(color), (float)array[0], kmeansForm.getHeight() - (float)array[1], 15, 15);
            g.DrawEllipse(new Pen(Color.Black, 2.0f), (float)array[0], kmeansForm.getHeight() - (float)array[1], 15, 15);
        }

        public void addItem(Item closestArray)
        {
            closestItemsList.Add(closestArray);
        }

        public int centroidId;
        public Centroid(List<Item> dataSet, Color color, int centroidId)
        {
            this.color = color;
            this.centroidId = centroidId;
            List<Tuple<double, double>> minMaxPoints = Misc.GetMinMaxPoints(dataSet);

            array = new double[minMaxPoints.Count];
            int height = kmeansForm.getHeight();
            int width = kmeansForm.getWidth();
            switch (centroidId)
            {
                case 0:
                    array = new double[2] { random.NextDouble() * width / 3, 2 * height / 3 + random.NextDouble() * height / 3 };
                    break;
                case 1:
                    array = new double[2] { 2 * width / 3 + random.NextDouble() * width / 3, random.NextDouble() * height / 3 };
                    break;
                case 2:
                    array = new double[2] { 1 * width / 3 + random.NextDouble() * width / 3, 1 * height / 3 + random.NextDouble() * height / 3 };
                    break;
                default:
                    array = new double[2] { random.NextDouble() * width, random.NextDouble() * height };
                    break;
            }
            _oldItemsList = new List<Item>();
            closestItemsList = new List<Item>();

            /*int i = 0;
            foreach (Tuple<double, double> point in minMaxPoints)
            {
                double minimum = 0;
                double maximum = 0;
                switch (centroidId)
                {
                    case 0:
                        minimum = 0;
                        maximum = 200;
                        break;
                    case 1:
                        minimum = 350;
                        maximum = 400;
                        break;
                    case 2:
                        minimum = point.Item1;
                        maximum = point.Item2;
                        break;
                    default:
                        break;
                }
                Console.WriteLine(centroidId+"  "+minimum + "  "+ maximum);
                double element = random.NextDouble() * (maximum - minimum) + minimum;

                array[i] = element;
                i++;
            }*/

        }

        public void MoveCentroid()
        {
            List<double> resultVector = new List<double>();

            if (closestItemsList.Count == 0) return;

            for(int j = 0; j < closestItemsList[0].point.GetLength(0); j++)
            {
                double sum = 0.0;
                for(int i = 0; i < closestItemsList.Count; i++)
                {
                    sum += closestItemsList[i].point[j];
                }
                Console.WriteLine(sum);
                sum /= closestItemsList.Count;
                resultVector.Add(sum);
            }

            array = resultVector.ToArray();
        }

        public bool HasChanged()
        {
            bool result = true;

            if (_oldItemsList.Count != closestItemsList.Count) return true;
            if (_oldItemsList.Count == 0 || closestItemsList.Count == 0) return false;

            for(int i=0; i < closestItemsList.Count; i++)
            {
                double[] oldPoint = _oldItemsList[i].point;
                double[] currentPoint = closestItemsList[i].point;

                for(int j=0;j<oldPoint.Length;j++)
                    if (oldPoint[j] != currentPoint[j])
                    {
                        result = false;
                        break;
                    }
            }
            return !result;
        }

        public void Reset()
        {
            _oldItemsList = Misc.Clone(closestItemsList);
            closestItemsList.Clear();
        }

        public override string ToString()
        {
            return String.Join(",", array);
        }
    }
}
