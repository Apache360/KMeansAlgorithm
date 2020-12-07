using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KMeansGUI
{
    public partial class KmeansForm : Form
    {
        public KMeans objKMeans;
        private BackgroundWorker objBackgroundWorker;
        KMeansEventArgs kmeansEA;
        private List<Item> dataSetItems;
        private string resultDataSet;
        public Centroid[] centroids;
        FileStream fileStreamInput = null;
        string inputFileName = null;

        public KmeansForm()
        {
            InitializeComponent();
        }

        private void buttonRandomRun_Click(object sender, EventArgs e)
        {
            buttonRandomRun.Enabled = false;            
            buttonRun.Enabled = false;
            richTextBox1.Text = "";

            objKMeans = new KMeans(Convert.ToInt32(numericUpDown2.Value), new EuclideanDistance());
            setRandomData();

            picImage.Invalidate();

            objBackgroundWorker = new BackgroundWorker();
            objBackgroundWorker.WorkerReportsProgress = true;
            objBackgroundWorker.DoWork += ObjBackgroundWorker_DoWork;
            objBackgroundWorker.RunWorkerCompleted += ObjBackgroundWorker_RunWorkerCompleted;
            objBackgroundWorker.ProgressChanged += ObjBackgroundWorker_ProgressChanged;

            objBackgroundWorker.RunWorkerAsync(dataSetItems);

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            buttonRun.Enabled = false;
            buttonRandomRun.Enabled = false;

            objKMeans = new KMeans(Convert.ToInt32(numericUpDown2.Value), new EuclideanDistance());
            if (fileStreamInput!=null)
            {
                fileStreamInput.Position = 0;
                setData(fileStreamInput);
            }

            picImage.Invalidate();

            objBackgroundWorker = new BackgroundWorker();
            objBackgroundWorker.WorkerReportsProgress = true;
            objBackgroundWorker.DoWork += ObjBackgroundWorker_DoWork;
            objBackgroundWorker.RunWorkerCompleted += ObjBackgroundWorker_RunWorkerCompleted;
            objBackgroundWorker.ProgressChanged += ObjBackgroundWorker_ProgressChanged;

            objBackgroundWorker.RunWorkerAsync(dataSetItems);
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
            buttonRandomRun.Enabled = true;
            if (inputFileName!=null) buttonRun.Enabled = true;

            drawLegend();
            if (numericUpDown1.Value <= 200) writeResult();
            else richTextBox1.Text = "There are more than 200 items.\nThe output is on demand.";
        }

        private void ObjBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Item> inputDataset = e.Argument as List<Item>;

            objKMeans.UpdateProgress += (x, y) => objBackgroundWorker.ReportProgress(0, y);
            centroids = objKMeans.Run(inputDataset);
            e.Result = centroids;
            setResultDataSet();
        }

        private void picImage_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (kmeansEA == null || kmeansEA.CentroidList == null) return;

            foreach(Centroid centroid in kmeansEA.CentroidList)
                centroid.DrawMe(e);

            if (kmeansEA.Dataset == null) return;

            foreach(Item item in kmeansEA.Dataset)
            {
                g.DrawEllipse(new Pen(Color.Gray, 2.0f), (float)item.point[0], 400-(float)item.point[1], 8, 8);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void openFile()
        {
            fileStreamInput = null;
            inputFileName = null;
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "All files|*.*|CSV|*.csv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    buttonRandomRun.Enabled = true;

                    inputFileName = ofd.FileName;
                    fileStreamInput = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);

                    string[] safeFileName = ofd.SafeFileName.Split(new char[] { '.' });

                    switch (safeFileName[safeFileName.Length - 1])
                    {
                        case "csv":
                            setData(fileStreamInput);
                            buttonRun.Enabled = true;
                            textBox1.Text = inputFileName;
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
            dataSetItems = new List<Item>();
            for (int i = 0; i < (int)numericUpDown1.Value; i++)
            {
                double chance1 = objRandom.NextDouble();
                //double chance1 = Math.Sin(((objRandom.NextDouble() / (Math.PI * 10)) * 90));
                double chance2 = Math.Sin(((objRandom.NextDouble() * 2 - 1) / (Math.PI * 10)) * 90) / 1.5;
                double chance3 = Math.Sin(((objRandom.NextDouble() * 2 - 1) / (Math.PI * 10)) * 90) / 1.5;
                /*double chance2 = Math.Sin(((objRandom.NextDouble() * 2 - 1) / (Math.PI * 10)) * 90);
                double chance3 = Math.Sin(((objRandom.NextDouble() * 2 - 1) / (Math.PI * 10)) * 90);*/

                double shift1, shift2;
                if (chance1<0.5)
                {
                    shift1 = chance1 * chance2;
                    shift2 = chance1 * chance3;
                }
                else
                {
                    shift1 = (1 - chance1) * chance2;
                    shift2 = (1 - chance1) * chance3;
                }

                double[] point = new double[2];
                point[0] = Math.Round(3 * (chance1 + shift1), 3);
                point[1] = Math.Round(100 * ((1 - chance1) + shift2), 3);
                //point[0] = Misc.GenerateRandomDouble(chance2, 0, 3);
                //point[1] = Misc.GenerateRandomDouble(chance3, 0, 100);
                Item item = new Item(i.ToString(),point);
                dataSetItems.Add(item);
            }
            normalizeData();
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
            normalizeData();
        }

        private void normalizeData()
        {
            int width = picImage.Width;
            int height = picImage.Height;
            maxX1 = maxX2 = 0;
            for (int i = 0; i < dataSetItems.Count; i++)
            {
                if (dataSetItems[i].point[0]>maxX1)
                {
                    maxX1 = dataSetItems[i].point[0];
                }
                if (dataSetItems[i].point[1] > maxX2)
                {
                    maxX2 = dataSetItems[i].point[1];
                }
            }
            widthScale = width / (maxX1+ 0.1*maxX1);
            heightScale = height / (maxX2+0.1*maxX2);
            for (int i = 0; i < dataSetItems.Count; i++)
            {
                dataSetItems[i].point= new double[2] {dataSetItems[i].point[0]* widthScale, dataSetItems[i].point[1]* heightScale };
            }
        }

        private void setResultDataSet()
        {
            for (int i = 0; i < centroids.Length; i++)
            {
                for (int j = 0; j < centroids[i].closestItemsList.Count; j++)
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); 
                    resultDataSet += i + ","
                        + (centroids[i].closestItemsList[j].id) + ","
                        + (centroids[i].closestItemsList[j].point[0]/widthScale) + ","
                        + (centroids[i].closestItemsList[j].point[1] / heightScale) + "\n";
                }
            }
        }

        private void writeResult()
        {
            richTextBox1.Clear();
            resultDataSet = "";
            for (int i = 0; i < centroids.Length; i++)
            {
                for (int j = 0; j < centroids[i].closestItemsList.Count; j++)
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    string str = i + ","
                        + (centroids[i].closestItemsList[j].id) + ","
                        + (centroids[i].closestItemsList[j].point[0] / widthScale) + ","
                        + (centroids[i].closestItemsList[j].point[1] / heightScale) + "\n";

                    int start = richTextBox1.Text.Length;
                    int length = str.Length;
                    richTextBox1.AppendText(str);
                    resultDataSet += str;
                    richTextBox1.Select(start, length);
                    richTextBox1.SelectionColor = centroids[i].color;
                }
            }
            richTextBox1.Update();
        }

        private void saveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files|*.*|CSV files|*.csv";
            bool correctFormatSelected = true;
            if (correctFormatSelected)
            {
                if (sfd.ShowDialog() == DialogResult.OK && sfd.FileName.Length > 0)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    writeData(sfd.FileName);

                    Cursor.Current = Cursors.Default;
                    Form formSucces = new Form();
                    formSucces.StartPosition = FormStartPosition.CenterParent;
                    formSucces.Size = new System.Drawing.Size(250, 100); ;
                    formSucces.MinimumSize = formSucces.MaximumSize = formSucces.Size;
                    Label labelSucces = new Label();
                    labelSucces.Location = new Point(40, 25);
                    labelSucces.Size = new System.Drawing.Size(200, 25); ;
                    formSucces.Controls.Add(labelSucces);
                    labelSucces.Text = "Process is succesfully finised!";
                    formSucces.ShowDialog();
                }
            }
        }

        private void writeData(string file)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(file+".csv", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(resultDataSet);
                }
                Console.WriteLine("Запись выполнена");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void buttonResult_Click(object sender, EventArgs e)
        {
            writeResult();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private double widthScale = 0, heightScale = 0, maxX1 = 0, maxX2 = 0;
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
            int picHeight = picImage.Height;
            int picWidth = picImage.Width;

            g.DrawLine(penAdditional, new Point(0, picHeight), new Point(picWidth, picHeight));
            g.DrawLine(penAdditional, new Point(0, picHeight), new Point(0, 0));
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(penAdditional, new Point(0, i* picHeight / 11), new Point(10, i * picHeight / 11));
                g.DrawLine(penAdditional, new Point(i * picWidth / 11, picHeight - 10), new Point(i * picWidth / 11, picHeight));
                g.DrawLine(penDotted, new Point(0, i * picHeight / 11), new Point(picWidth, i * picHeight / 11));
                g.DrawLine(penDotted, new Point(i * picWidth / 11, 0), new Point(i * picWidth / 11, picHeight));
                if (i!=0&&i!=11)
                {
                    g.DrawString(Math.Round((maxX1 / 10)*i,2).ToString(), drawFontValue, drawBrush, new Point((i * picWidth / 11)-5, picHeight - 25));
                    g.DrawString(Math.Round((maxX2 / 10)*i,2).ToString(), drawFontValue, drawBrush, new Point(15, ((11-i) * picHeight / 11)-10));
                }
                g.DrawString("X1", drawFontValueBold, drawBrush, new Point(picWidth-20, picHeight - 25));
                g.DrawString("X2", drawFontValueBold, drawBrush, new Point(3,3));
            }
        }
    }
}
