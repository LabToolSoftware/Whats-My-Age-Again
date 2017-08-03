using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GandalfTheGrey
{
    class KMeansCluster
    {
        private int _k = 5;
        private double[][] _pixelsData;
        public KMeansCluster()
        {

        }

        public int[] Cluster(double[][] data)
        {
            int numClusters = _k;
            //double[][] data = Normalized(rawData);
            bool changed = true; bool success = true;
            int[] clustering = InitClusters(data, numClusters);
            double[][] means = Allocate(numClusters, data[0].Length);
            int maxCount = data.Length * 10;
            int ct = 0;
            while (changed == true && success == true && ct < maxCount)
            {
                ++ct;
                success = UpdateMeans(data, clustering, means);
                changed = UpdateClustering(data, clustering, means);
            }
            return clustering;
        }

        private int[] InitClusters(double[][] data, int numClusters)
        {
            Random randnum = new Random();
            int[] clustering = new int[data.Length + numClusters];
            double[][] initmeans = new double[numClusters][];
            for (int i = 0; i < numClusters; i++)
            {
                clustering[i] = i;
            }
            double[,] distances = new double[data.Length, numClusters];
            double[] D2 = new double[data.Length];
            initmeans[0] = new double[3];
            initmeans[0] = data[randnum.Next(0, data.Length - 1)];

            for (int i = 0; i < initmeans.Length - 1; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    distances[j, i] = Distance(data[j], initmeans[i]);
                    D2[j] = distances[j, i];
                }
                initmeans[i + 1] = new double[3];
                initmeans[i + 1] = data[GetIndexofnextk(D2)];
            }

            for (int j = 0; j < data.Length; j++)
            {
                distances[j, 4] = Distance(data[j], initmeans[4]);
                D2[j] = distances[j, 4];
            }

            for (int i = 0; i < distances.GetLength(0); i++)
            {
                double min = distances[i, 0];
                int minindex = 0;
                for (int j = 0; j < numClusters; j++)
                {
                    if (distances[i, j] < min)
                    {
                        min = distances[i, j];
                        minindex = j;
                    }
                }
                clustering[i + numClusters] = minindex;

            }
            return clustering;
        }

        private int GetIndexofnextk(double[] distances)
        {
            double[] PDF = new double[distances.Length];
            double Sum_D2 = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                Sum_D2 += distances[i] * distances[i];
            }
            double totP = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                PDF[i] = (distances[i] * distances[i]) / Sum_D2;
            }

            Random rand = new Random();
            double cumulative = 0;
            int result = 0;
            double p = rand.NextDouble();
            int ii = 0;

            while (cumulative < p)
            {
                cumulative += PDF[ii];
                ++ii; // next candidate
                result = ii;
            }

            return result;
        }

        private static bool UpdateMeans(double[][] data, int[] clustering, double[][] means)
        {
            int numClusters = means.Length;
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] = 0.0;

            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i];
                for (int j = 0; j < data[i].Length; ++j)
                    means[cluster][j] += data[i][j]; // accumulate sum
            }

            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] /= clusterCounts[k]; // danger of div by 0
            return true;
        }
        private static double[][] Allocate(int numClusters, int numColumns)
        {
            double[][] result = new double[numClusters][];
            for (int k = 0; k < numClusters; ++k)
                result[k] = new double[numColumns];
            return result;
        }

        private static bool UpdateClustering(double[][] data, int[] clustering, double[][] means)
        {
            int numClusters = means.Length;
            bool changed = false;

            int[] newClustering = new int[clustering.Length];
            Array.Copy(clustering, newClustering, clustering.Length);

            double[] distances = new double[numClusters];

            for (int i = 0; i < data.Length; ++i)
            {
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(data[i], means[k]);

                int newClusterID = MinIndex(distances);
                if (newClusterID != newClustering[i])
                {
                    changed = true;
                    newClustering[i] = newClusterID;
                }
            }

            if (changed == false)
                return false;

            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = newClustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            Array.Copy(newClustering, clustering, newClustering.Length);
            return true; // no zero-counts and at least one change
        }
        private static double Distance(double[] tuple, double[] mean)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Length; ++j)
                sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2);
            return Math.Sqrt(sumSquaredDiffs);
        }
        private static int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }
    }
}
