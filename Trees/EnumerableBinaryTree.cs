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

    internal interface IEnumerableBinaryTreeNode<T> : IEnumerable<T>
    {
        T Value { get; set; }
        IEnumerableBinaryTreeNode<T> Left { get; set; }
        IEnumerableBinaryTreeNode<T> Right { get; set; }

        IEnumerator<T> GetEnumerator(TraversalMethods method);

        IEnumerator<T> Preorder { get; } 

        IEnumerator<T> Inorder { get; } 

        IEnumerator<T> Postorder { get; } 
    }

    public abstract class EnumerableBinaryTree<T> : IEnumerable<T>
    {
        internal abstract IEnumerableBinaryTreeNode<T> Root { get; set; }

        public void Clear()
        {
            Root = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            if (Root == null) {
                return Enumerable.Empty<T>().GetEnumerator();
            }
            return Root.GetEnumerator();
        }

        public virtual IEnumerator<T> GetEnumerator(TraversalMethods method)
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
    }
}
