using KMeansProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMeansGUI
{
    public partial class KmeansForm : Form
    {
        private KMeans objKMeans;
        private List<double[]> dataSetPoints;
        private List<double[]> dataSetRawPoints;
        private BackgroundWorker objBackgroundWorker;
        KMeansEventArgs kmeansEA;
        private List<Item> dataSetItems;
        public KmeansForm()
        {
            InitializeComponent();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            buttonRun.Enabled = false;            

            objKMeans = new KMeans(Convert.ToInt32(numericUpDown2.Value), new EuclideanDistance());

            picImage.Invalidate();

            objBackgroundWorker = new BackgroundWorker();
            objBackgroundWorker.WorkerReportsProgress = true;
            objBackgroundWorker.DoWork += ObjBackgroundWorker_DoWork;
            objBackgroundWorker.RunWorkerCompleted += ObjBackgroundWorker_RunWorkerCompleted;
            objBackgroundWorker.ProgressChanged += ObjBackgroundWorker_ProgressChanged;

            objBackgroundWorker.RunWorkerAsync(dataSetPoints.ToArray());
        }

        private void ObjBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            System.Console.WriteLine(">>> Progress Changed");
            kmeansEA = e.UserState as KMeansEventArgs;
            if (kmeansEA != null)
            {
                foreach (Centroid centroid in kmeansEA.CentroidList)
                {
                    System.Console.WriteLine("Centroid: " + centroid.ToString());
                    picImage.Invalidate();
                    //Thread.Sleep(1000);
                }
            }
        }
        
        private void ObjBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Centroid[] result = e.Result as Centroid[];
            Console.WriteLine("Work is done!");
            buttonRun.Enabled = true;
            drawLegend();
        }

        private void ObjBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            double[][] inputDataset = e.Argument as double[][];
            objKMeans.UpdateProgress +=(x,y)=> {
                objBackgroundWorker.ReportProgress(0,y); };
            e.Result = objKMeans.Run(inputDataset);
        }

        private void picImage_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (kmeansEA == null || kmeansEA.CentroidList == null) return;

            foreach(Centroid centroid in kmeansEA.CentroidList)
                centroid.DrawMe(e);

            if (kmeansEA.Dataset == null) return;

            foreach(double[] point in kmeansEA.Dataset)
            {
                g.DrawEllipse(new Pen(Color.Gray, 2.0f), (float)point[0], 400-(float)point[1], 10, 10);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void openFile()
        {
            FileStream fileStreamInput = null;
            string inputFileName = null; 
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "All files|*.*|CSV|*.csv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    buttonRun.Enabled = true;

                    inputFileName = ofd.FileName;
                    fileStreamInput = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);

                    string[] safeFileName = ofd.SafeFileName.Split(new char[] { '.' });

                    switch (safeFileName[safeFileName.Length - 1])
                    {
                        case "csv":
                            setData(fileStreamInput);
                            //setRandomData();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void setRandomData()
        {
           Random objRandom = new Random();
            dataSetPoints = new List<double[]>();
            /*for (int i = 0; i < (int)numericUpDown1.Value; i++)
            {
                double[] point = new double[2];
                for (int j = 0; j < 2; j++)
                {
                    point[j] = Misc.GenerateRandomDouble(objRandom, 0, 400);
                }
                dataSetList.Add(point);
            }
            for (int i = 0; i < dataSetList.Count; i++)
            {
                for (int j = 0; j < dataSetList[i].Length; j++)
                {
                    Console.WriteLine(dataSetList[i][j]);
                }
            }*/



            dataSetPoints.Add(new double[2] { 10, 10 });
            dataSetPoints.Add(new double[2] { 70, 10 });
            dataSetPoints.Add(new double[2] { 130, 10 });
            dataSetPoints.Add(new double[2] { 10, 70 });
            dataSetPoints.Add(new double[2] { 70, 70 });
            dataSetPoints.Add(new double[2] { 130, 70 });
            dataSetPoints.Add(new double[2] { 200, 70 });
            dataSetPoints.Add(new double[2] { 70, 130 });
            dataSetPoints.Add(new double[2] { 130, 130 });
            dataSetPoints.Add(new double[2] { 200, 130 });
            dataSetPoints.Add(new double[2] { 270, 130 });
            dataSetPoints.Add(new double[2] { 70, 200 });
            dataSetPoints.Add(new double[2] { 130, 200 });
            dataSetPoints.Add(new double[2] { 200, 200 });
            dataSetPoints.Add(new double[2] { 270, 200 });
            dataSetPoints.Add(new double[2] { 130, 270 });
            dataSetPoints.Add(new double[2] { 200, 270 });
            dataSetPoints.Add(new double[2] { 270, 270 });
            dataSetPoints.Add(new double[2] { 340, 270 });
            dataSetPoints.Add(new double[2] { 200, 340 });
            dataSetPoints.Add(new double[2] { 270, 340 });
            dataSetPoints.Add(new double[2] { 340, 340 });
            dataSetPoints.Add(new double[2] { 390, 340 });
            dataSetPoints.Add(new double[2] { 270, 390 });
            dataSetPoints.Add(new double[2] { 340, 390 });
            dataSetPoints.Add(new double[2] { 390, 390 });
        }

        private void setData(FileStream fileStreamInput)
        {
            dataSetItems = new List<Item>();

            int coma = 0;
            string id = "", x1 = "", x2 = "";
            for (int i = 0; i < fileStreamInput.Length; i++)
            {
                char ch = (char)fileStreamInput.ReadByte();
                if (ch == '\n')
                {
                    dataSetItems.Add(new Item(id, new double[2] {
                        Double.Parse(x1, System.Globalization.CultureInfo.InvariantCulture),
                        Double.Parse(x2, System.Globalization.CultureInfo.InvariantCulture)
                        }));
                    id = x1 = x2 = "";
                    coma = 0;
                }
                else
                {
                    if (ch == ',') 
                    {
                        coma++;
                    }
                    else
                    {
                        switch (coma)
                        {
                            case 0:
                                id += ch;
                                break;
                            case 1:
                                x1 += ch;
                                break;
                            case 2:
                                x2 += ch;
                                break;
                            default:
                                break;
                        }
                    }
                    
                }
            }

            dataSetRawPoints = new List<double[]>();
            for (int i = 0; i < dataSetItems.Count; i++)
            {
                dataSetRawPoints.Add(new double[2] { dataSetItems[i].point[0], dataSetItems[i].point[1] });
            }

            normalizeData();
        }

        private void normalizeData()
        {
            int width = picImage.Width;
            int height = picImage.Height;
            maxX1 = maxX2 = 0;
            for (int i = 0; i < dataSetRawPoints.Count; i++)
            {
                if (dataSetRawPoints[i][0]>maxX1)
                {
                    maxX1 = dataSetRawPoints[i][0];
                }
                if (dataSetRawPoints[i][1] > maxX2)
                {
                    maxX2 = dataSetRawPoints[i][1];
                }
            }
            widthScale = width / (maxX1+ 0.1*maxX1);
            heightScale = height / (maxX2+0.1*maxX2);
            dataSetPoints = new List<double[]>();
            for (int i = 0; i < dataSetRawPoints.Count; i++)
            {
                dataSetPoints.Add(new double[2] { dataSetRawPoints[i][0]* widthScale, dataSetRawPoints[i][1]* heightScale });
            }
        }

        private double widthScale = 0, heightScale= 0,maxX1 = 0, maxX2 = 0;
        public void drawLegend()
        {
            Graphics g = picImage.CreateGraphics();
            Pen penAdditional = new Pen(Color.Gray, 3);
            Pen penDotted = new Pen(Color.Gray, 1);
            penDotted.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            Font drawFontValue = new Font("Arial", 10);
            Font drawFontValueBold = new Font("Arial", 10, FontStyle.Bold);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            StringFormat drawFormat = new StringFormat(); 

            g.DrawLine(penAdditional, new Point(0, picImage.Height), new Point(picImage.Width, picImage.Height));
            g.DrawLine(penAdditional, new Point(0, picImage.Height), new Point(0, 0));
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(penAdditional, new Point(0, i*picImage.Height/11), new Point(10, i * picImage.Height / 11));
                g.DrawLine(penAdditional, new Point(i * picImage.Width / 11, picImage.Height-10), new Point(i * picImage.Width / 11, picImage.Height));
                g.DrawLine(penDotted, new Point(0, i * picImage.Height / 11), new Point(picImage.Width, i * picImage.Height / 11));
                g.DrawLine(penDotted, new Point(i * picImage.Width / 11, 0), new Point(i * picImage.Width / 11, picImage.Height));
                if (i!=0&&i!=11)
                {
                    g.DrawString(Math.Round((maxX1 / 10)*i,2).ToString(), drawFontValue, drawBrush, new Point((i * picImage.Width / 11)-5, picImage.Height - 25));
                    g.DrawString(Math.Round((maxX2 / 10)*i,2).ToString(), drawFontValue, drawBrush, new Point(15, ((11-i) * picImage.Height / 11)-10));
                }
                g.DrawString("X1", drawFontValueBold, drawBrush, new Point(picImage.Width-19, picImage.Height-25));
                g.DrawString("X2", drawFontValueBold, drawBrush, new Point(3,3));
            }
        }
    }
}
