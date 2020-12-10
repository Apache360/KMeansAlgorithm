using KMeansGUI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace KMeansGUI
{
    public delegate void OnUpdateProgress(object sender, KMeansEventArgs eventArgs);
    public class KMeans
    {
        private IDistance _distance;
        private int _centroidsCount;

        public event OnUpdateProgress UpdateProgress;
        protected virtual void OnUpdateProgress(KMeansEventArgs eventArgs)
        {
            if (UpdateProgress != null)
                UpdateProgress(this, eventArgs);
            Thread.Sleep(400);
        }

        public KMeans(int k, IDistance distance)
        {
            _centroidsCount = k;
            _distance = distance;
        }

        public KMeans()
        {
        }

        public List<Centroid> centroidList;
        public Centroid[] Run(List<Item> dataSet)
        {
            centroidList = new List<Centroid>();
            for (int i=0;i<_centroidsCount;i++)
            {
                Centroid centroid = new Centroid(dataSet,Misc.centroidColors[i], centroidList.Count);
                centroidList.Add(centroid);
            }
            OnUpdateProgress(new KMeansEventArgs(centroidList,dataSet));
            while (true)
            {
                foreach (Centroid centroid in centroidList)centroid.Reset();

                for (int i = 0; i < dataSet.Count; i++)
                {
                    Item item = dataSet[i];
                    int closestIndex = -1;
                    double minDistance = Double.MaxValue;
                    for (int k = 0; k < centroidList.Count; k++)
                    {
                        double distance = _distance.Run(centroidList[k].array, item);
                        switch (k)
                        {
                            case 0:
                                distance *= 0.5;
                                break;
                            case 1:
                                distance *= 1.1;
                                break;
                            case 3:
                                distance *= 1.1;
                                break;
                            default:
                                break;
                        }
                        if (distance < minDistance)
                        {
                            closestIndex = k;
                            minDistance = distance;
                        }
                    }
                    centroidList[closestIndex].addItem(item);
                }

                foreach (Centroid centroid in centroidList)centroid.MoveCentroid();

                OnUpdateProgress(new KMeansEventArgs(centroidList, null));

                bool hasChanged = false;
                foreach (Centroid centroid in centroidList)
                    if (centroid.HasChanged())
                    {
                        hasChanged = true;
                        break;
                    }
                if (!hasChanged)
                    break;
            }
            return centroidList.ToArray();
        }
    }
}
