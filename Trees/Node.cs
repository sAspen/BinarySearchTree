using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    public class Node<T> where T : IComparable<T>, IEquatable<T>
    {
        private T data;
        protected NodeList<T> Neighbors = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
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
    }
}
