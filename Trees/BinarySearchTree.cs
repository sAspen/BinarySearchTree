using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{

    public class BinarySearchTree<T> : BinaryTree<T> where T : IComparable<T>
    {
        private bool RemoveUsingPredecessor = true;

        public BinarySearchTree() : base() { }
        internal BinarySearchTree(BinarySearchTreeNode<T> tree) : this()
        {
            Root = tree;
        }

        public override bool Contains(T data)
        {
            if (Root == null) {
                return false;
            }
            return ((BinarySearchTreeNode<T>) Root).Contains(data);
        }

        internal virtual BinarySearchTreeNode<T> Find(T data)
        {
            if (Root == null) {
                return null;
            }
            return ((BinarySearchTreeNode<T>) Root).Find(data);
        }

        public override void Add(T data)
        {
            if (Root == null) {
                return;
            }
            ((BinarySearchTreeNode<T>) Root).Add(data);
        }

        public override bool Remove(T data)
        {
            if (Root == null) {
                return false;
            }
            return ((BinarySearchTreeNode<T>) Root).Remove(data, !(RemoveUsingPredecessor = !RemoveUsingPredecessor));
        }

        internal virtual BinarySearchTreeNode<T> Min()
        {
            if (Root == null) {
                return null;
            }
            return ((BinarySearchTreeNode<T>) Root).Min();
        }

        internal virtual BinarySearchTreeNode<T> Max()
        {
            if (Root == null) {
                return null;
            }
            return ((BinarySearchTreeNode<T>) Root).Max();
        }

        public override IEnumerator<T> GetEnumerator()
        {
            if (Root == null) {
                return Enumerable.Empty<T>().GetEnumerator();
            }
            return Root.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator(TraversalMethods method)
        {
            if (Root == null) {
                return Enumerable.Empty<T>().GetEnumerator();
            }
            return Root.GetEnumerator(method);
        }

        public IEnumerator<T> Preorder
        { 
            get
            {
                if (Root == null) {
                    return Enumerable.Empty<T>().GetEnumerator();
                }
                return Root.GetEnumerator(TraversalMethods.Preorder);
            }
        }

        public IEnumerator<T> Inorder
        {
            get
            {
                if (Root == null) {
                    return Enumerable.Empty<T>().GetEnumerator();
                }
                return Root.GetEnumerator(TraversalMethods.Inorder);
            }
        }

        public IEnumerator<T> Postorder
        {
            get
            {
                if (Root == null) {
                    return Enumerable.Empty<T>().GetEnumerator();
                }
                return Root.GetEnumerator(TraversalMethods.Postorder);
            }
        }

        public List<T> ToList()
        {
            if (Root == null) {
                return null;
            }
            return Root.ToList();
        }

        public T[] ToArray()
        {
            if (Root == null) {
                return null;
            }
            return Root.ToArray();
        }

        public override int Count
        {
            get
            {
                if (Root == null) {
                    return 0;
                }
                return Root.Count;
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (Root != null) {
                Root.CopyTo(array, arrayIndex, TraversalMethods.Inorder);
            }
        }

        public void CopyTo(T[] array, int arrayIndex, TraversalMethods method)
        {
            if (Root != null) {
                Root.CopyTo(array, arrayIndex, method);
            }
        }
        
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }
}
