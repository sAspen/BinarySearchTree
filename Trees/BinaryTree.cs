using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    public enum TraversalMethods
    {
        Preorder,
        Inorder,
        Postorder
    }

    public abstract class BinaryTree<T> : ICollection, ICollection<T> where T : IComparable<T>
    {
        private BinaryTreeNode<T> root = null;

        public BinaryTree() { }

        public virtual void Clear()
        {
            root = null;
        }

        internal virtual BinaryTreeNode<T> Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return root == null;
            }
        }

        public abstract void Add(T item);

        public abstract bool Contains(T item);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract int Count
        {
            get;
        }

        public abstract bool IsReadOnly
        {
            get;
        }

        public abstract bool Remove(T item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public abstract IEnumerator<T> GetEnumerator();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }
    }
}
