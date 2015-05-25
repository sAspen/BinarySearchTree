using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    internal abstract class BinaryTreeNode<T> : Node<T>, ICollection where T : IComparable<T>
    {
        public BinaryTreeNode() : base() { }
        public BinaryTreeNode(T data) : base(data, null) { }
        public BinaryTreeNode(T data, NodeList<T> children)
            : base(data, children)
        {
            if (children.Count > 0) {
                var t = children[0].GetType();
                if (t.IsSubclassOf(typeof(BinaryTreeNode<T>)) ||
                    t == typeof(BinaryTreeNode<T>)) {
                    throw new ArgumentException();
                }
            }
        }

        public BinaryTreeNode(T data, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
        {
            base.Value = data;
            NodeList<T> children = new NodeList<T>(2);
            children[0] = left;
            children[1] = right;

            base.Neighbors = children;
        }

        public BinaryTreeNode<T> Left
        {
            get
            {
                if (base.Neighbors == null) {
                    return null;
                } else {
                    return (BinaryTreeNode<T>) base.Neighbors[0];
                }
            }
            set
            {
                if (base.Neighbors == null) {
                    base.Neighbors = new NodeList<T>(2);
                }

                base.Neighbors[0] = value;
            }
        }

        public BinaryTreeNode<T> Right
        {
            get
            {
                if (base.Neighbors == null) {
                    return null;
                } else {
                    return (BinaryTreeNode<T>) base.Neighbors[1];
                }
            }
            set
            {
                if (base.Neighbors == null) {
                    base.Neighbors = new NodeList<T>(2);
                }

                base.Neighbors[1] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator(TraversalMethods.Inorder);
        }

        public IEnumerator<T> GetEnumerator(TraversalMethods method)
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

        public IEnumerator<T> Preorder
        {
            get
            {
                var parents = new Stack<BinaryTreeNode<T>>();
                var current = this;
                while (parents.Count > 0 || current != null) {
                    if (current != null) {
                        yield return current.Value;
                        if (current.Right != null) {
                            parents.Push(current.Right);
                        }
                        current = current.Left;
                    } else {
                        current = parents.Pop();
                    }
                }
            }
        }

        public IEnumerator<T> Inorder
        {
            get
            {
                var parents = new Stack<BinaryTreeNode<T>>();
                var current = this;
                while (parents.Count > 0 || current != null) {
                    if (current != null) {
                        parents.Push(current);
                        current = current.Left;
                    } else {
                        current = parents.Pop();
                        yield return current.Value;
                        current = current.Right;
                    }
                }
            }
        }

        public IEnumerator<T> Postorder
        {
            get
            {
                var parents = new Stack<BinaryTreeNode<T>>();
                BinaryTreeNode<T> current = this, previous = null;
                while (parents.Count > 0 || current != null) {
                    if (current != null) {
                        parents.Push(current);
                        current = current.Left;
                    } else {
                        var peek = parents.Peek();
                        if (peek.Right != null && previous != peek.Right) {
                            current = peek.Right;
                        } else {
                            yield return peek.Value;
                            previous = parents.Pop();
                        }
                    }
                }
            }
        }


        public virtual List<T> ToList()
        {
            var items = new List<T>();

            foreach (T item in this) {
                items.Add(item);
            }

            return items;
        }

        public virtual T[] ToArray()
        {
            T[] array = new T[Count];

            CopyTo(array, 0);

            return array;
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

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, TraversalMethods.Inorder);
        }

        public void CopyTo(T[] array, int arrayIndex, TraversalMethods method)
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
                array[arrayIndex + i++] = enumerator.Current;
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

        public void CopyTo(Array array, int index)
        {
            if (array.Rank > 1) {
                throw new ArgumentException("array is multidimensional.");
            }
            if ((T[]) array == null) {
                throw new ArgumentException("The type of the source ICollection cannot be cast " + 
                    "automatically to the type of the destination array.");
            }

            CopyTo((T[]) array, index);
        }
    }
}
