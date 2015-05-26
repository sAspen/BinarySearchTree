using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trees
{

    internal class KdTreeNode : IEnumerableBinaryTreeNode<double[]>
    {
        private double[] data;
        internal readonly int Axis;
        private KdTreeNode LeftChild, RightChild;

        public KdTreeNode() { }
        public KdTreeNode(double[] data, int axis) : this(data, axis, null, null) { }
        public KdTreeNode(double[] data, int axis, KdTreeNode left, KdTreeNode right)
        {
            Value = (double[]) data.Clone();
            Axis = axis % Dimensions;
            Left = left;
            Right = right;
        }

        public double[] Value
        {
            get
            {
                return data;
            }
            set
            {
                data = (double[]) value.Clone();
            }
        }

        public int Dimensions
        {
            get
            {
                return data.Length;
            }
        }

        public IEnumerableBinaryTreeNode<double[]> Left
        {
            get
            {
                return LeftChild;
            }
            set
            {
                LeftChild = (KdTreeNode) value;
            }
        }

        public IEnumerableBinaryTreeNode<double[]> Right
        {
            get
            {
                return RightChild;
            }
            set
            {
                RightChild = (KdTreeNode) value;
            }
        }
        public delegate object VisitNode(params object[] arguments);
        //arguments[0]: input data
        //arguments[1]: null or node containing arguments[0]
        //arguments[2]: null or parent of arguments[1]
        //arguments[3]: true if arguments[1] is Left child of arguments[2]
        public struct VisitNodeArgument
        {
            public const int Data = 0,
            Node = 1,
            Parents = 2,
            WentLeft = 3;
        }

        public static VisitNode VisitReturnTrue =
                (object[] arguments) => {
                    return true;
                };

        public static VisitNode VisitReturnFalse =
                (object[] arguments) => {
                    return false;
                };

        public static VisitNode VisitReturnNull =
                (object[] arguments) => {
                    return null;
                };

        public virtual object FindAndVisit(double[] targetPoint, VisitNode onSuccess, VisitNode onFailure)
        {
            var previous = new Stack<KdTreeNode>();
            object[] arguments = { targetPoint, this, previous, false };

            for (KdTreeNode current = (KdTreeNode) arguments[VisitNodeArgument.Node];
                current != null;
                arguments[VisitNodeArgument.Node] = current) {
                if (current.Value.SequenceEqual(targetPoint)) {
                    return onSuccess(arguments);
                } else {
                    int result = current.Value[current.Axis].CompareTo(targetPoint[current.Axis]);

                    previous.Push((KdTreeNode) current);

                    if (result > 0) {
                        arguments[VisitNodeArgument.WentLeft] = true;
                        current = (KdTreeNode) current.Left;
                    } else {
                        arguments[VisitNodeArgument.WentLeft] = false;
                        current = (KdTreeNode) current.Right;
                    }
                }
            }

            return onFailure(arguments);
        }

        public virtual bool Contains(double[] targetPoint)
        {
            return (bool) FindAndVisit(targetPoint, VisitReturnTrue, VisitReturnFalse);
        }

        internal virtual KdTreeNode Find(double[] targetPoint)
        {
            KdTreeNode dummy;
            return Find(targetPoint, out dummy);
        }

        internal virtual KdTreeNode Find(double[] targetPoint, out KdTreeNode parent)
        {
            parent = null;
            KdTreeNode parentNode = null;

            VisitNode onSuccess =
                (object[] arguments) => {
                    var parents = (Stack<KdTreeNode>) arguments[VisitNodeArgument.Parents];
                    if (parents.Count > 0) {
                        parentNode = parents.Pop();
                    }
                    return arguments[VisitNodeArgument.Node];
                };


            var ret = (KdTreeNode) FindAndVisit(targetPoint, onSuccess, VisitReturnNull);
            parent = parentNode;
            return ret;
        }

        internal virtual Stack<KdTreeNode> GetParentOf(double[] targetPoint)
        {
            VisitNode onVisit =
                (object[] arguments) => {
                    return ((Stack<KdTreeNode>) arguments[VisitNodeArgument.Parents]);
                };

            var ret = (Stack<KdTreeNode>) FindAndVisit(targetPoint, onVisit, onVisit);
            if (ret.Count == 0) {
                ret.Push(this);
            }
            return ret;
        }

        public virtual void Add(double[] targetPoint)
        {
            VisitNode onFailure =
                (object[] arguments) => {
                    double[] leafData = (double[]) arguments[VisitNodeArgument.Data];
                    var parent = ((Stack<KdTreeNode>) arguments[VisitNodeArgument.Parents]).Pop();
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    var leaf = new KdTreeNode(leafData, parent.Axis + 1);

                    if (wentLeft) {
                        parent.Left = leaf;
                    } else {
                        parent.Right = leaf;
                    }

                    return true;
                };

            FindAndVisit(targetPoint, VisitReturnFalse, onFailure);
        }

        public bool Remove(double[] targetPoint)
        {
            KdTreeNode parent = null, replacement = null;
            KdTreeNode node = Find(targetPoint, out parent);
            if (node == null) {
                return false;
            }

            if (!node.IsLeaf) {
                do {
                    replacement = node.GetSuccessorNode(out parent);
                    if (replacement == null) {
                        replacement = node.GetPredecessorNode(out parent);
                    }
                    node.Value = (double[]) replacement.Value.Clone();
                    node = replacement;
                } while (!node.IsLeaf);
            }

            if (parent.Left == node) {
                parent.Left = null;
            } else if (parent.Right == node) {
                parent.Right = null;
            } else {
                throw new SystemException();
            }

            return !Contains(targetPoint);
        }

        internal virtual double[] Min()
        {
            return Min(Axis);
        }

        internal virtual double[] Min(int whichAxis)
        {
            var nodes = new List<KdTreeNode>(Count);

            double[] currentMin = Enumerable.Repeat(Double.PositiveInfinity, Dimensions).ToArray();
            var parents = new Stack<KdTreeNode>();
            var current = this;
            while (parents.Count > 0 || current != null) {
                if (current != null) {
                    nodes.Add(current);
                    if (whichAxis != current.Axis && current.Right != null) {
                        parents.Push((KdTreeNode) current.Right);
                    }
                    if (current.Value[whichAxis] < currentMin[whichAxis]) {
                        currentMin = (double[]) current.Value.Clone();
                    }
                    current = (KdTreeNode) current.Left;
                } else {
                    current = parents.Pop();
                }
            }

            return nodes.OrderBy(n => n.Value[whichAxis]).First().Value;
        }

        internal virtual KdTreeNode MinNode(int whichAxis)
        {
            KdTreeNode dummy;
            return MinNode(Axis, out dummy);
        }

        internal virtual KdTreeNode MinNode(out KdTreeNode parent)
        {
            return MinNode(Axis, out parent);
        }

        internal KdTreeNode MinNode(int whichAxis, out KdTreeNode parent)
        {
            var minimum = Min(whichAxis);

            return Find(minimum, out parent);
        }

        internal virtual double[] Max()
        {
            return Max(Axis);
        }

        internal virtual double[] Max(int whichAxis)
        {
            var nodes = new List<KdTreeNode>(Count);

            double[] currentMax = Enumerable.Repeat(Double.NegativeInfinity, Dimensions).ToArray();
            var parents = new Stack<KdTreeNode>();
            var current = this;
            while (parents.Count > 0 || current != null) {
                if (current != null) {
                    nodes.Add(current);
                    if (whichAxis != current.Axis && current.Left != null) {
                        parents.Push((KdTreeNode) current.Left);
                    }
                    if (current.Value[whichAxis] > currentMax[whichAxis]) {
                        currentMax = (double[]) current.Value.Clone();
                    }
                    current = (KdTreeNode) current.Right;
                } else {
                    current = parents.Pop();
                }
            }

            return nodes.OrderBy(n => n.Value[whichAxis]).Last().Value;
        }

        internal virtual KdTreeNode MaxNode(int whichAxis)
        {
            KdTreeNode dummy;
            return MaxNode(Axis, out dummy);
        }

        internal virtual KdTreeNode MaxNode(out KdTreeNode parent)
        {
            return MaxNode(Axis, out parent);
        }

        internal KdTreeNode MaxNode(int whichAxis, out KdTreeNode parent)
        {
            var maximum = Max(whichAxis);

            return Find(maximum, out parent);
        }

        internal double[] GetPredecessor()
        {
            if (Left == null) {
                return null;
            }
            return GetPredecessorNode().Value;
        }

        internal KdTreeNode GetPredecessorNode()
        {
            KdTreeNode dummy;
            return GetPredecessorNode(out dummy);
        }

        internal KdTreeNode GetPredecessorNode(out KdTreeNode parent)
        {
            if (Left == null) {
                parent = null;
                return null;
            }

            var ret = ((KdTreeNode) Left).MaxNode(Axis, out parent);
            if (parent == null) {
                parent = this;
            }
            return ret;
        }

        internal double[] GetSuccessor()
        {
            if (Right == null) {
                return null;
            }

            return GetSuccessorNode().Value;
        }

        internal KdTreeNode GetSuccessorNode()
        {
            KdTreeNode dummy;
            return GetSuccessorNode(out dummy);
        }

        internal KdTreeNode GetSuccessorNode(out KdTreeNode parent)
        {
            if (Right == null) {
                parent = null;
                return null;
            }

            var ret = ((KdTreeNode) Right).MinNode(Axis, out parent);
            if (parent == null) {
                parent = this;
            }
            return ret;
        }

        public double[] GetNearestTo(double[] targetPoint)
        {
            double bestDist = Double.PositiveInfinity;
            KdTreeNode current = null, currentBestNode = null;

            Stack<KdTreeNode> parents = GetParentOf(targetPoint);
            while (parents.Count > 0) {
                current = parents.Pop();

                double distance = current.GetDistanceTo(targetPoint);

                if (distance < bestDist) {
                    bestDist = distance;
                    currentBestNode = current;
                }

                int axis = current.Axis;
                if (Math.Pow(targetPoint[axis] - current.Value[axis], 2) < bestDist) {
                    Stack<KdTreeNode> nextParents = null;
                    if (targetPoint[axis] < current.Value[axis] && current.Right != null) {
                        nextParents = ((KdTreeNode) current.Right).GetParentOf(targetPoint);
                    } else if (current.Left != null) {
                        nextParents = ((KdTreeNode) current.Left).GetParentOf(targetPoint);
                    }
                    if (nextParents != null) {
                        while (nextParents.Count > 0) {
                            parents.Push(nextParents.Pop());
                        }
                    }
                }
            }

            if (currentBestNode == null) {
                currentBestNode = this;
            }

            return (double[]) currentBestNode.Value.Clone();
        }

        public double GetDistanceTo(double[] targetPoint)
        {
            if (Dimensions != targetPoint.Length) {
                return Double.NaN;
            }

            double ret = 0.0;
            for (int i = 0; i < Dimensions; i++) {
                ret += Math.Pow(data[i] - targetPoint[i], 2);
            }

            return ret;
        }

        public IEnumerator<double[]> GetEnumerator()
        {
            return GetEnumerator(TraversalMethods.Inorder);
        }
        public IEnumerator<double[]> GetEnumerator(TraversalMethods method)
        {
            switch (method) {
                case TraversalMethods.Inorder:
                    return Inorder;
                case TraversalMethods.Postorder:
                    return Postorder;
                case TraversalMethods.Preorder:
                    return Preorder;
                default:
                    throw new ArgumentException();
            }
        }

        public IEnumerator<double[]> Preorder
        {
            get
            {
                var parents = new Stack<KdTreeNode>();
                var current = this;
                while (parents.Count > 0 || current != null) {
                    if (current != null) {
                        yield return current.Value;
                        if (current.Right != null) {
                            parents.Push((KdTreeNode) current.Right);
                        }
                        current = (KdTreeNode) current.Left;
                    } else {
                        current = parents.Pop();
                    }
                }
                yield break;
            }
        }

        public IEnumerator<double[]> Inorder
        {
            get
            {
                var parents = new Stack<KdTreeNode>();
                var current = this;
                while (parents.Count > 0 || current != null) {
                    if (current != null) {
                        parents.Push(current);
                        current = (KdTreeNode) current.Left;
                    } else {
                        current = parents.Pop();
                        yield return current.Value;
                        current = (KdTreeNode) current.Right;
                    }
                }
                yield break;
            }
        }

        public IEnumerator<double[]> Postorder
        {
            get
            {
                var parents = new Stack<KdTreeNode>();
                KdTreeNode current = this, previous = null;
                while (parents.Count > 0 || current != null) {
                    if (current != null) {
                        parents.Push(current);
                        current = (KdTreeNode) current.Left;
                    } else {
                        var peek = parents.Peek();
                        if (peek.Right != null && previous != peek.Right) {
                            current = (KdTreeNode) peek.Right;
                        } else {
                            yield return peek.Value;
                            previous = parents.Pop();
                        }
                    }
                }
                yield break;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return LeftChild == null && RightChild == null;
            }
        }

        public void CopyChildrenOf(KdTreeNode node)
        {
            LeftChild = node.LeftChild;
            RightChild = node.RightChild;
        }

        public void CopyTo(Array array, int index)
        {
            if ((double[][]) array == null) {
                throw new ArgumentException("The type of the source ICollection cannot be cast " +
                    "automatically to the type of the destination array.");
            }

            CopyTo((double[][]) array, index);
        }

        public virtual void CopyTo(double[][] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, TraversalMethods.Inorder);
        }

        public void CopyTo(double[][] array, int arrayIndex, TraversalMethods method)
        {
            if (array == null) {
                throw new ArgumentNullException("array is null.");
            }
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException("arrayIndex is less than 0.");
            }
            if (array.Length - arrayIndex < Count) {
                throw new ArgumentException("The number of elements in the source ICollection<T> is greater" +
                    "than the available space from arrayIndex to the end of the destination array.");
            }

            var enumerator = GetEnumerator(method);
            for (int i = 0; enumerator.MoveNext(); i++) {
                array[arrayIndex + i++] = (double[]) enumerator.Current.Clone();
            }
        }

        public virtual int Count
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string s = "{ ";

            for (int i = 0; i < data.Length; i++) {
                s += data[i] + ", ";
            }

            s = s.Substring(0, s.Length - 2) + " }";

            return s;
        }

        public void Assert()
        {
            if (Left != null) {
                if (!(Left.Value[Axis] < Value[Axis])) {
                    throw new SystemException();
                }
                LeftChild.Assert();
            }
            if (Right != null) {
                if (!(Right.Value[Axis] >= Value[Axis])) {
                    throw new SystemException();
                }
                RightChild.Assert();
            }
        }
    }
}
