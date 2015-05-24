using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    public abstract class BinaryTree<T> : ICollection<T> where T : IComparable<T>, IEquatable<T>
    {
        private BinaryTreeNode<T> root;

        public BinaryTree()
        {
            root = null;
        }

        public virtual void Clear()
        {
            root = null;
        }

        public BinaryTreeNode<T> Root
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

        public virtual int Count
        {
            get
            {
                return 0;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public abstract bool Remove(T item);


        protected abstract IEnumerator<T> GenericEnumerable_GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GenericEnumerable_GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GenericEnumerable_GetEnumerator();
        }
    }
}
