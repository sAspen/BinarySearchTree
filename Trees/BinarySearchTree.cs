using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{

    public class BinarySearchTree<T> : BinaryTree<T> where T : IComparable<T>, ICloneable
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
                Root = new BinarySearchTreeNode<T>(data);
            } else {
                ((BinarySearchTreeNode<T>) Root).Add(data); 
            }
        }

        public override bool Remove(T data)
        {
            if (Root == null) {
                return false;
            }
            return ((BinarySearchTreeNode<T>) Root).Remove(data, !(RemoveUsingPredecessor = !RemoveUsingPredecessor));
        }

        internal virtual T Min()
        {
            if (Root == null) {
                return default(T);
            }
            return ((BinarySearchTreeNode<T>) Root).Min();
        }

        internal virtual T Max()
        {
            if (Root == null) {
                return default(T);
            }
            return ((BinarySearchTreeNode<T>) Root).Max();
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
                return ((BinarySearchTreeNode<T>) Root).Count;
            }
        }

        public override void CopyTo(Array array, int index)
        {
            if ((T[]) array == null) {
                throw new ArgumentException("The type of the source ICollection cannot be cast " +
                    "automatically to the type of the destination array.");
            }

            CopyTo((T[]) array, index);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (Root != null) {
                ((BinarySearchTreeNode<T>) Root).CopyTo(array, arrayIndex, TraversalMethods.Inorder);
            }
        }

        public void CopyTo(T[] array, int arrayIndex, TraversalMethods method)
        {
            if (Root != null) {
                ((BinarySearchTreeNode<T>) Root).CopyTo(array, arrayIndex, method);
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
