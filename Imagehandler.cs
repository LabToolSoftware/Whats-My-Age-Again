using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace GandalfTheGrey
{
    public class Imagehandler
    {
        private Bitmap _image;
        Bitmap cluster_image;
        public int[] _clusters { get; private set; }
        public int[] int_freqs { get; private set; }
        public double[][] pixeldata { get; private set; }
        public double[][] newpixeldata;


        public Imagehandler()
        {

        }

        public void SetImage(Bitmap image)
        {
            this._image = image;
            SortPixels();
        }

        private void SortPixels()
        {
            Rectangle rect_image = new Rectangle(0, 0, _image.Width, _image.Height);
            BitmapData bmpData = _image.LockBits(rect_image, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * _image.Height;

            byte[] rgbVals = new byte[bytes];

            pixeldata = new double[_image.Height * _image.Width][];

            Marshal.Copy(ptr, rgbVals, 0, bytes);

            int count = 0;
            int stride = bmpData.Stride;

            for (int column = 0; column < bmpData.Height; column++)
            {
                for (int row = 0; row < bmpData.Width; row++)
                {
                    double[] pixel = new double[3];
                    //b,g,r
                    pixel[0] = (double)(rgbVals[(column * stride) + (row * 3)]);
                    pixel[1] = (double)(rgbVals[(column * stride) + (row * 3) + 1]);
                    pixel[2] = (double)(rgbVals[(column * stride) + (row * 3) + 2]);
                    pixeldata[count] = pixel;
                    count++;
                }
            }
            _image.UnlockBits(bmpData);
        }

        public int[] BuildHistogram(int[] intensities)
        {
            int[] histogram = new int[256];
            int count;

            for (int i = 0; i < 256; i++)
            {
                count = 0;

                for (int j = 0; j < intensities.Length; j++)
                {
                    if (intensities[j] == i)
                    {
                        count += 1;
                    }
                }

                histogram[i] = count;
            }

            return histogram;
        }

        public Image showClusters(int numClusters)
        {

            cluster_image = new Bitmap(_image);
            int num_rows = pixeldata.Length / _image.Width;
            int num_cols = pixeldata.Length / _image.Height;

            for (int row = 0; row < num_rows; row++)
            {
                for (int column = 0; column < num_cols; column++)
                {
                    switch (_clusters[((row * num_cols) + column) + numClusters])
                    {
                        case 0:
                            cluster_image.SetPixel(column, row, Color.Green);
                            break;
                        case 1:
                            cluster_image.SetPixel(column, row, Color.Blue);
                            break;
                        case 2:
                            cluster_image.SetPixel(column, row, Color.Red);
                            break;
                        case 3:
                            cluster_image.SetPixel(column, row, Color.Yellow);
                            break;
                        case 4:
                            cluster_image.SetPixel(column, row, Color.Purple);
                            break;
                    }
                }
            }

            return cluster_image;
        }

        public float GetGreyness(List<int> clusterindeces, int greycutoff)
        {
            List<int> averageInt = new List<int>();

            for (int i = 0; i < _clusters.Length - 5; i++)
            {
                if (clusterindeces.Contains(_clusters[i]))
                {
                    averageInt.Add(CalcAverageIntensity(pixeldata[i]));
                }
            }


            int[] normalizedavg = LinearContrastStretch(Normalizer(averageInt));

            int_freqs = BuildHistogram(normalizedavg);

            return calculatePercentage(int_freqs, greycutoff);
        }

        private int CalcAverageIntensity(double[] pixel)
        {
            int sum = 0;
            for (int i = 0; i < pixel.Length; i++)
            {
                sum += (int)pixel[i];
            }
            int average = sum / 3;
            return average;
        }

        private List<int> Normalizer(List<int> pixelstobenorm)
        {
            List<int> normalizedavg = new List<int>();
            int minval = pixelstobenorm.Min();
            for (int i = 0; i < pixelstobenorm.Count; i++)
            {
                normalizedavg.Add(pixelstobenorm[i] - minval);
            }
            return normalizedavg;
        }

        private float calculatePercentage(int[] intensityFreqs, int cutoff)
        {
            int numgreypixels = 0;
            int totalpixels = 0;
            float result;
            for (int i = 0; i < intensityFreqs.Length; i++)
            {
                totalpixels += int_freqs[i];
            }
            for (int i = cutoff; i < int_freqs.Length; i++)
            {
                numgreypixels += int_freqs[i];
            }

            result = ((float)numgreypixels / (float)totalpixels) * 100;

            return result;
        }

        public void clusterImage()
        {
            KMeansCluster kmeans = new KMeansCluster();
            _clusters = kmeans.Cluster(pixeldata);
        }

        private int[] LinearContrastStretch(List<int> pixels)
        {
            int count = 0;
            for (int i = 0; i < pixels.Count; i++)
            {
                double highest = pixels[0];
                if (pixels[i] < highest)
                {
                    count += 1;
                }
            }

            int a = 0;
            int b = 255;
            int c = 23;
            int d = 69;

            int length = pixels.Count;

            int[] stretched_int = new int[length];

            for (int i = 0; i < length; i++)
            {
                stretched_int[i] = (pixels[i] - c) * ((b - a) / (d - c)) + a;
            }
            return stretched_int;
        }

        public Image showSelectedClusters(List<int> clusterindeces)
        {

            Bitmap selected_clusters = new Bitmap(_image);
            for (int col = 0; col < cluster_image.Width; col++)
            {
                for (int row = 0; row < cluster_image.Height; row++)
                {
                    if (!clusterindeces.Contains(_clusters[(row * cluster_image.Width) + col + 5]))
                    {
                        selected_clusters.SetPixel(col, row, Color.White);
                    }
                }
            }

            return selected_clusters;
        }
    }
}
