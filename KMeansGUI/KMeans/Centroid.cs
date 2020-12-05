using KMeansGUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KMeansProject
{
    public class Centroid
    {
        public double[] array;
        private List<double[]> _oldPointsList;
        private List<double[]> _closestPointsList;
        private Color _color;
        private static Random random = new Random();

        public void DrawMe(PaintEventArgs e)
        {
            KmeansForm form = new KmeansForm();
            Graphics g = e.Graphics;
            form.drawLegend();
            g.FillEllipse(new SolidBrush(_color),(float)array[0],400-(float)array[1], 15, 15);

            foreach(double[] point in _closestPointsList)
            {
                g.DrawEllipse(new Pen(_color, 2.0f), (float)point[0], 400-(float)point[1], 10, 10);
            }

        }

        public void addPoint(double[] closestArray)
        {
            _closestPointsList.Add(closestArray);
        }

        public Centroid(double[][] dataSet, Color color)
        {
            _color = color;

            List<Tuple<double, double>> minMaxPoints = Misc.GetMinMaxPoints(dataSet);

            array = new double[minMaxPoints.Count];
            int i = 0;
            foreach (Tuple<double, double> tuple in minMaxPoints)
            {
                double minimum = tuple.Item1;
                double maximum = tuple.Item2;
                double element = random.NextDouble() * (maximum - minimum) + minimum;
                array[i] = element;
                i++;
            }
           
            _oldPointsList = new List<double[]>();
            _closestPointsList = new List<double[]>();
        }

        public void MoveCentroid()
        {
            List<double> resultVector = new List<double>();

            if (_closestPointsList.Count == 0) return;

            for(int j = 0; j < _closestPointsList[0].GetLength(0); j++)
            {
                double sum = 0.0;
                for(int i = 0; i < _closestPointsList.Count; i++)
                {
                    sum += _closestPointsList[i][j];
                }
                sum /= _closestPointsList.Count;
                resultVector.Add(sum);
            }

            array = resultVector.ToArray();
        }

        public bool HasChanged()
        {
            bool result = true;

            if (_oldPointsList.Count != _closestPointsList.Count) return true;
            if (_oldPointsList.Count == 0 || _closestPointsList.Count == 0) return false;

            for(int i=0; i < _closestPointsList.Count; i++)
            {
                double[] oldPoit = _oldPointsList[i];
                double[] currentPoint = _closestPointsList[i];

                for(int j=0;j<oldPoit.Length;j++)
                    if (oldPoit[j] != currentPoint[j])
                    {
                        result = false;
                        break;
                    }
            }

            return !result;
        }

        public void Reset()
        {
            _oldPointsList = Misc.Clone(_closestPointsList);
            _closestPointsList.Clear();
        }

        public override string ToString()
        {
            return String.Join(",", array);
        }
    }
}
