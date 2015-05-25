using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{

    internal class KdTreeNode : IEnumerableBinaryTreeNode<double[]>
    {
        private double[] data;
        internal readonly int Axis;
        private KdTreeNode LeftChild, RightChild;
        //internal bool Ignore;

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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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
                return LeftChild;
            }
            set
            {
                LeftChild = (KdTreeNode) value;
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
            Parent = 2,
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



        public virtual object FindAndVisit(double[] data, VisitNode onSuccess, VisitNode onFailure)
        {
            bool wentLeft = false;
            object[] arguments = { data, this, null, false };
            KdTreeNode previous = null;

            for (KdTreeNode current = (KdTreeNode) arguments[VisitNodeArgument.Node];
                current != null;
                arguments[VisitNodeArgument.Node] = current) {
                int result = current.Value[current.Axis].CompareTo(data);

                if (result == 0) {
                    if (!current.Value.SequenceEqual(data)) {
                        throw new SystemException();
                    }
                    return onSuccess(data, current, previous, wentLeft);
                } else if (result > 0) {
                    arguments[VisitNodeArgument.WentLeft] = true;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = (KdTreeNode) current.Left;
                } else {
                    arguments[VisitNodeArgument.WentLeft] = false;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = (KdTreeNode) current.Right;
                }
            }

            return onFailure(arguments);
        }
        public virtual bool Contains(double[] data)
        {
            return (bool) FindAndVisit(data, VisitReturnTrue, VisitReturnFalse);
        }

        internal virtual KdTreeNode Find(double[] data)
        {
            KdTreeNode dummy;
            return Find(data, out dummy);
        }

        internal virtual KdTreeNode Find(double[] data, out KdTreeNode parent)
        {
            parent = null;
            KdTreeNode parentNode = null;

            VisitNode onSuccess =
                (object[] arguments) => {
                    parentNode = (KdTreeNode) arguments[VisitNodeArgument.Parent];
                    return arguments[VisitNodeArgument.Node];
                };

            
            var ret = (KdTreeNode) FindAndVisit(data, onSuccess, VisitReturnNull);
            parent = parentNode;
            return ret;
        }

        internal virtual KdTreeNode GetParentOf(double[] data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return arguments[VisitNodeArgument.Parent];
                };

            return (KdTreeNode) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        public virtual void Add(double[] data)
        {
            VisitNode onFailure =
                (object[] arguments) => {
                    double[] leafData = (double[]) arguments[VisitNodeArgument.Data];
                    var parent = (KdTreeNode) arguments[VisitNodeArgument.Parent];
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    var leaf = new KdTreeNode(leafData, parent.Axis + 1);

                    if (wentLeft) {
                        parent.Left = leaf;
                    } else {
                        parent.Right = leaf;
                    }

                    return true;
                };

            FindAndVisit(data, VisitReturnFalse, onFailure);
        }

        public bool Remove(double[] data)
        {
            var node = Find(data);
            if (node == null) {
                return false;
            }

            while (!node.IsLeaf) {
                var replacement = node.GetSuccessorNode();
                if (replacement != null) {
                    replacement = node.GetPredecessorNode();
                }
                node.Value = (double[]) replacement.Value.Clone();
                node = replacement;
            }

            var parent = GetParentOf(node.Value);
            if (parent.Left == node) {
                parent.Left = null;
            } else {
                parent.Right = null;
            }

            return true;
        }

        public List<KdTreeNode> SortBy(int whichAxis)
        {
            var nodes = new List<KdTreeNode>(Count);

            var parents = new Stack<KdTreeNode>();
            var current = this;
            while (parents.Count > 0 || current != null) {
                if (current != null) {
                    nodes.Add(current);
                    if (whichAxis != current.Axis && current.Right != null) {
                        parents.Push((KdTreeNode) current.Right);
                    }
                    current = (KdTreeNode) current.Left;
                } else {
                    current = parents.Pop();
                }
            }

            return nodes.OrderBy(n => n.Value[whichAxis]).ToList();
        }

        internal virtual double[] Min()
        {
            return Min(Axis);
        }

        internal virtual double[] Min(int whichAxis)
        {
            return SortBy(whichAxis).First().Value;
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
            return SortBy(whichAxis).Last().Value;
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

            return ((KdTreeNode) Left).MaxNode(Axis, out parent);
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

            return ((KdTreeNode) Right).MinNode(Axis, out parent);
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

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
