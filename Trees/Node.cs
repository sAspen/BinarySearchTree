using System;

namespace Trees
{
    internal abstract class Node<T> where T : IComparable<T>, ICloneable
    {
        private T data;
        protected NodeList<T> Neighbors = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = (T) data.Clone();
            this.Neighbors = neighbors;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        
        public bool IsLeaf
        {
            get
            {
                return Neighbors == null || Neighbors.Count == 0;
            }
        }

        public virtual void CopyChildrenOf(Node<T> node)
        {
            Neighbors = node.Neighbors;
        }
    }
}
