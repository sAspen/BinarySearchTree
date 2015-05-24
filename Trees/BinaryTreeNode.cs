using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    public class BinaryTreeNode<T> : Node<T> where T : IComparable<T>, IEquatable<T>
    {
        public BinaryTreeNode() : base() { }
        public BinaryTreeNode(T data) : base(data, null) { }
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

        public virtual void CopyChildrenOf(BinaryTreeNode<T> node)
        {
            Neighbors = node.Neighbors;
        }
    }
}
