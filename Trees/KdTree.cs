using System;
using System.Collections.Generic;
using System.Linq;

namespace Trees
{
    public class KdTree : EnumerableBinaryTree<double[]>, ICollection<double[]>
    {
        private KdTreeNode root = null;

        public KdTree() { }
        public KdTree(ICollection<double[]> data)
        {
            if (data == null || data.Count == 0) {
                return;
            }

            var points = new List<double[]>(data);
            Random RNG = new Random();
            while (points.Count > 0) {
                int i = RNG.Next(points.Count);
                Add(points[i]);
                points.RemoveAt(i);
            }
        }

        internal override IEnumerableBinaryTreeNode<double[]> Root
        {
            get
            {
                return root;
            }
            set
            {
                root = (KdTreeNode) value;
            }
        }

        public int Dimensions
        {
            get
            {
                if (root == null) {
                    return 0;
                }
                return root.Dimensions;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return root == null;
            }
        }

        public void Add(double[] item)
        {
            if (Root == null) {
                Root = new KdTreeNode(item, 0);
            } else {
                root.Add(item);
            }
        }

        public bool Contains(double[] item)
        {
            if (Root == null) {
                return false;
            }
            return root.Contains(item);
        }


        public bool IsReadOnly
        {
            get { return false; ; }
        }

        public bool Remove(double[] item)
        {
            if (Root == null) {
                return false;
            }
            if (root.IsLeaf && root.Value.SequenceEqual(item)) {
                Root = null;
                return true;
            }
            return root.Remove(item);
        }

        public double[] GetNearestTo(double[] targetPoint)
        {
            if (Root == null || targetPoint.Length != Dimensions) {
                return null;
            }
            if (root.IsLeaf) {
                return (double[]) root.Value.Clone();
            }

            return root.GetNearestTo(targetPoint);
        }

        public void CopyTo(Array array, int index)
        {
            if ((double[][]) array == null) {
                throw new ArgumentException("The type of the source ICollection cannot be cast " +
                    "automatically to the type of the destination array.");
            }
            CopyTo((double[][]) array, index);
        }

        public void CopyTo(double[][] array, int arrayIndex)
        {
            if (Root != null) {
                root.CopyTo((double[][]) array, arrayIndex, TraversalMethods.Inorder);
            }
        }

        public void CopyTo(double[][] array, int arrayIndex, TraversalMethods method)
        {
            if (Root != null) {
                root.CopyTo(array, arrayIndex, method);
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var node in this) {
                    count++;
                }
                return count;
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        public static double GetDistanceTo(double[] sourcePoint, double[] targetPoint)
        {
            if (sourcePoint.Length != targetPoint.Length) {
                return Double.NaN;
            }

            double ret = 0.0;
            for (int i = 0; i < sourcePoint.Length; i++) {
                ret += Math.Pow(sourcePoint[i] - targetPoint[i], 2);
            }

            return ret;
        }

        public override string ToString()
        {
            return "Count = " + Count;
        }
    }
}
