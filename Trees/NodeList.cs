using System;
using System.Collections.ObjectModel;

namespace Trees
{
    internal class NodeList<T> : Collection<Node<T>> where T : IComparable<T>, ICloneable
    {
        public NodeList() : base() { }

        public NodeList(int initialSize)
        {
            for (int i = 0; i < initialSize; i++) {
                base.Items.Add(default(Node<T>));
            }
        }

        public Node<T> FindByValue(T value)
        {
            foreach (var node in Items) {
                if (node.Value.Equals(value)) {
                    return node;
                }
            }

            return null;
        }
    }
}
