using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GandalfTheGrey
{
    public partial class Histogram : Form
    {
        private Dictionary<string, int[]> _rgbhisto = new Dictionary<string, int[]>();
        private Imagehandler _imgHdlr;
        private Image _histogram;




        public Histogram(Imagehandler imgHdlr)
        {
            InitializeComponent();
            this._imgHdlr = imgHdlr;
            pictureBox1.Image = DrawHisto(_imgHdlr.int_freqs);

        }

        private Image DrawHisto(int[] pixelFreqs)
        {
            int height = pictureBox1.Height;
            int width = pictureBox1.Width;
            int widthPerFrequency = width / 256;
            int histWidth = widthPerFrequency * 256; //Width of actual hist we will center horizontally.
            int spaceLeft = width - histWidth;
            int startX = spaceLeft / 2;
            // Define the brushes/pens we will use:
            var axisPen = new System.Drawing.Pen(System.Drawing.Brushes.DarkGray, 2);
            var gridPen = new System.Drawing.Pen(System.Drawing.Brushes.DarkGray, 0.5f);
            var linGrBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(startX, height - 10),
                new Point(startX + histWidth, height - 10),
                System.Drawing.Color.FromArgb(0, 0, 0),
                System.Drawing.Color.FromArgb(255, 255, 255)
                );
            var bluePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(51, 153, 255), 2);
            var redPen = new System.Drawing.Pen(System.Drawing.Color.OrangeRed, 2);
            var polyBrush = new SolidBrush(System.Drawing.Color.FromArgb(120, 51, 153, 255));
            // Make canvas and get bitmap gfx handler g
            Bitmap hist = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(hist);
            // Paint the canvas white, add gradient bar
            g.FillRectangle(System.Drawing.Brushes.White, new Rectangle(0, 0, width, height));
            g.FillRectangle(linGrBrush, startX, height - 20, histWidth, 10);
            // Draw the axis
            g.DrawLine(axisPen, startX, 10, startX, height - 30); //y
            g.DrawLine(axisPen, startX, height - 30, startX + histWidth, height - 30); //x
            // Draw meaningless grid marks just beacause it looks nice
            // Draw them every 10th of an x length
            int seperatingPixel = histWidth / 10;
            for (int x = startX; x < startX + histWidth; x += seperatingPixel)
            {
                g.DrawLine(gridPen, x, 10, x, height - 30);
            }
            for (int y = height - 30; y > 10; y -= seperatingPixel)
            {
                g.DrawLine(gridPen, startX, y, startX + histWidth, y);
            }
            int biggestValue = pixelFreqs.Max();
            Point[] polygon = new Point[256];
            // Add a point for each and every pixel frequency
            for (int i = 0; i < 256; i++)
            {
                float percent = (float)pixelFreqs[i] / (float)biggestValue;
                float pixel_percent = percent * (height - 40);
                int percInt = (int)pixel_percent;
                Point addedPoint = new Point((startX + widthPerFrequency * i), (height - 30 - percInt));
                polygon[i] = addedPoint;
            }
            //polygon[256] = new Point(widthPerFrequency * 256, height - 30); //Add a line taking us back to the axis
            // polygon[257] = new Point(startX, height - 30); //and back to the origin
            g.DrawLines(bluePen, polygon);
            //g.FillPolygon(polyBrush, polygon);
            // Draw a line between the min and max values we are showing - insert your resampled values here if you have them
            return hist;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void drawHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Building Histogram...";

            toolStripStatusLabel1.Text = "Histogram Ready.";
        }
    }
}
