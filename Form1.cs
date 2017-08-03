using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GandalfTheGrey
{
    public partial class Form1 : Form
    {
        private Imagehandler imgHdlr = new Imagehandler();

        private KMeansCluster kmeans;

        public Form1()
        {
            InitializeComponent();
            ParametersMenu();
            kmeans = new KMeansCluster();

            toolStripStatusLabel1.Text = "Ready...";
        }

        private void ParametersMenu()
        {

            int height = pictureBox2.Height;
            int width = pictureBox2.Width;
            int startX = 0;
            var linGrBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(startX, height),
                new Point(width, height),
                System.Drawing.Color.FromArgb(0, 0, 0),
                System.Drawing.Color.FromArgb(255, 255, 255)
                );

            Bitmap hist = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(hist);

            g.FillRectangle(linGrBrush, startX, height - 20, width, height);

            pictureBox2.Image = hist;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog imagefile = new OpenFileDialog())
            {
                if (imagefile.ShowDialog() == DialogResult.OK)
                {
                    Bitmap image = null;
                    if (image != null)
                    {
                        image.Dispose();
                    }
                    try
                    {
                        toolStripStatusLabel1.Text = "Opening Image...";
                        image = (Bitmap)Image.FromFile(imagefile.FileName, true);
                        imgHdlr.SetImage(image);
                        pictureBox1.Image = image;
                        toolStripStatusLabel1.Text = "Image Loaded Successfully...";

                    }
                    catch (Exception)
                    {
                        toolStripStatusLabel1.Text = "Failed to Load Image...";
                        MessageBox.Show("File not at image.");
                    }
                }


            }
        }

        private void viewIntensityHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Histogram histForm = new Histogram(imgHdlr);
            histForm.ShowDialog();
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Click down to begin cropping. Hold Ctrl + Cick to adjust crop field width.";
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Ready...";

        }

        private void button1_Click(object sender, EventArgs e)
        {

            List<int> clusterids = new List<int>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    switch (checkedListBox1.Items[i].ToString())
                    {
                        case "Green":
                            clusterids.Add(0);
                            break;
                        case "Blue":
                            clusterids.Add(1);
                            break;
                        case "Red":
                            clusterids.Add(2);
                            break;
                        case "Yellow":
                            clusterids.Add(3);
                            break;
                        case "Purple":
                            clusterids.Add(4);
                            break;
                    }
                }

            }

            textBox1.Text = imgHdlr.GetGreyness(clusterids, (trackBar1.Value * 23)).ToString();
        }

        private void processImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            imgHdlr.clusterImage();
            pictureBox1.Image = imgHdlr.showClusters(checkedListBox1.Items.Count);
            foreach (var item in imgHdlr._clusters.Distinct())
            {
                switch (item)
                {
                    case 0:
                        checkedListBox1.Items.Add("Green");
                        //cluster_image.SetPixel(column, row, Color.Green);
                        break;
                    case 1:
                        checkedListBox1.Items.Add("Blue");

                        //cluster_image.SetPixel(column, row, Color.Blue);
                        break;
                    case 2:
                        checkedListBox1.Items.Add("Red");

                        //cluster_image.SetPixel(column, row, Color.Red);
                        break;
                    case 3:
                        checkedListBox1.Items.Add("Yellow");

                        //cluster_image.SetPixel(column, row, Color.Yellow);
                        break;
                    case 4:
                        checkedListBox1.Items.Add("Purple");
                        break;

                }
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Show Selection")
            {
                List<int> clusterids = new List<int>();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        switch (checkedListBox1.Items[i].ToString())
                        {
                            case "Green":
                                clusterids.Add(0);
                                break;
                            case "Blue":
                                clusterids.Add(1);
                                break;
                            case "Red":
                                clusterids.Add(2);
                                break;
                            case "Yellow":
                                clusterids.Add(3);
                                break;
                            case "Purple":
                                clusterids.Add(4);
                                break;
                        }
                    }

                }
                pictureBox1.Image = imgHdlr.showSelectedClusters(clusterids);
                button2.Text = "Show Original";
            }
            else if (button2.Text == "Show Original")
            {
                pictureBox1.Image = imgHdlr.showClusters(5);
                button2.Text = "Show Selection";


            }
        }
    }
}
