using System.Collections.Generic;

namespace KMeansGUI
{
    public class KMeansEventArgs
    {
        private List<Centroid> _centroidList;
        private List<Item> _dataset;
        
        public List<Centroid> CentroidList{ get { return _centroidList; }}        
        public List<Item> Dataset{get { return _dataset; }}

        public KMeansEventArgs(List<Centroid> centroidList,List<Item> dataset)
        {
            _centroidList = centroidList;
            _dataset = dataset;
        }
    }
}
