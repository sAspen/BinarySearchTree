using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    public class BinarySearchTree<T> : BinaryTree<T> where T : IComparable<T>, IEquatable<T>
    {
        private bool RemoveUsingPredecessor = true;

        public BinarySearchTree() : base() { }

        public delegate object VisitNode(params object[] arguments);
        //arguments[0]: input data
        //arguments[1]: null or node containing arguments[0]
        //arguments[2]: null or parent of arguments[1]
        //arguments[3]: true if arguments[1] is Left child of arguments[2]
        public struct VisitNodeArgument
        {
            public const int Data = 0,
            Node = 1,
            Parent = 2,
            WentLeft = 3;
        }

        public static VisitNode VisitReturnTrue =
                (object[] arguments) => {
                    return true;
                };

        public static VisitNode VisitReturnFalse =
                (object[] arguments) => {
                    return false;
                };

        public static VisitNode VisitReturnNull =
                (object[] arguments) => {
                    return null;
                };

        public virtual object FindAndVisit(T data, VisitNode onSuccess, VisitNode onFailure)
        {
            bool wentLeft = false;
            object[] arguments = { data, Root, null, false };
            BinaryTreeNode<T> previous = null;

            for (BinaryTreeNode<T> current = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Node];
                current != null;
                arguments[VisitNodeArgument.Node] = current) {
                int result = current.Value.CompareTo(data);

                if (result == 0) {
                    return onSuccess(data, current, previous, wentLeft);
                } else if (result > 0) {
                    arguments[VisitNodeArgument.WentLeft] = true;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = current.Left;
                } else {
                    arguments[VisitNodeArgument.WentLeft] = false;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = current.Right;
                }
            }

            return onFailure(arguments);
        }

        /*public virtual void TraverseIn(VisitNode visit)
        {
            var nodes = new Stack<BinaryTreeNode<T>>();
            bool wentLeft = false;
            object[] arguments = { null, Root, null, false };
            BinaryTreeNode<T> previous = null;

            for (BinaryTreeNode<T> current = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Node];
                current != null;
                arguments[VisitNodeArgument.Node] = current) {
                int result = current.Value.CompareTo(data);

                if (result == 0) {
                    return onSuccess(data, current, previous, wentLeft);
                } else if (result > 0) {
                    arguments[VisitNodeArgument.WentLeft] = true;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = current.Left;
                } else {
                    arguments[VisitNodeArgument.WentLeft] = false;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = current.Right;
                }
            }

            while (nodes.Count > 0 || current != null) {
                if (current != null) {
                    nodes.Push(current);
                    current = current.Left
                } else {
                    current = nodes.Pop();
                    visit()
                }
            }

        }*/

        public override bool Contains(T data)
        {
            return (bool) FindAndVisit(data, VisitReturnTrue, VisitReturnFalse);
        }

        public virtual BinaryTreeNode<T> Find(T data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return arguments[VisitNodeArgument.Node];
                };

            return (BinaryTreeNode<T>) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        public virtual BinaryTreeNode<T> GetParentOf(T data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return arguments[VisitNodeArgument.Parent];
                };

            return (BinaryTreeNode<T>) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        public override void Add(T data)
        {
            VisitNode onFailure =
                (object[] arguments) => {
                    T leafData = (T) arguments[VisitNodeArgument.Data];
                    var leaf = new BinaryTreeNode<T>(leafData);

                    var parent = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Parent];
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    if (parent == null) {
                        Root = leaf;
                    } else if (wentLeft) {
                        parent.Left = leaf;
                    } else {
                        parent.Right = leaf;
                    }

                    return true;
                };

            FindAndVisit(data, VisitReturnFalse, onFailure);
        }



        public override bool Remove(T data)
        {
            VisitNode onSuccessUsingPredecessor =
                 (object[] arguments) => {
                     var node = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Node];
                     var parent = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Parent];
                     var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                     if (node.Left == null) {
                         if (parent == null) {
                             Root = node.Right;
                         } else if (!wentLeft) {
                             parent.Right = node.Right;
                         } else {
                             parent.Left = node.Right;
                         }
                     } else {
                         BinaryTreeNode<T> predecessorParent, predecessor = GetPredecessorOf(node, out predecessorParent);

                         predecessorParent.Right = predecessor.Left;

                         predecessor.CopyChildrenOf(node);

                         if (parent == null) {
                             Root = predecessor;
                         } else if (!wentLeft) {
                             parent.Right = predecessor;
                         } else {
                             parent.Left = predecessor;
                         }
                     }

                     return true;
                 };
            VisitNode onSuccessUsingSuccessor =
                (object[] arguments) => {
                    var node = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Node];
                    var parent = (BinaryTreeNode<T>) arguments[VisitNodeArgument.Parent];
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    if (node.Right == null) {
                        if (parent == null) {
                            Root = node.Left;
                        } else if (wentLeft) {
                            parent.Left = node.Left;
                        } else {
                            parent.Right = node.Left;
                        }
                    } else {
                        BinaryTreeNode<T> successorParent, successor = GetSuccessorOf(node, out successorParent);

                        successorParent.Left = successor.Right;

                        successor.CopyChildrenOf(node);

                        if (parent == null) {
                            Root = successor;
                        } else if (wentLeft) {
                            parent.Left = successor;
                        } else {
                            parent.Right = successor;
                        }
                    }

                    return true;
                };

            return (bool) FindAndVisit(data,
                !(RemoveUsingPredecessor = !RemoveUsingPredecessor) ? onSuccessUsingPredecessor : onSuccessUsingSuccessor,
                VisitReturnFalse);
        }

        public virtual BinaryTreeNode<T> Min()
        {
            return Min(Root);
        }

        public static BinaryTreeNode<T> Min(BinaryTreeNode<T> tree)
        {
            BinaryTreeNode<T> dummy;
            return Min(tree, out dummy);
        }

        public static BinaryTreeNode<T> Min(BinaryTreeNode<T> tree, out BinaryTreeNode<T> successorParent)
        {
            successorParent = null;
            var node = tree;
            if (node != null) {
                while (node.Left != null) {
                    successorParent = node;
                    node = node.Left;
                }
            }
            return node;
        }

        public virtual BinaryTreeNode<T> Max()
        {
            return Max(Root);
        }

        public static BinaryTreeNode<T> Max(BinaryTreeNode<T> tree)
        {
            BinaryTreeNode<T> dummy;
            return Max(tree, out dummy);
        }

        public static BinaryTreeNode<T> Max(BinaryTreeNode<T> tree, out BinaryTreeNode<T> successorParent)
        {
            successorParent = null;
            var node = tree;
            if (node != null) {
                while (node.Right != null) {
                    successorParent = node;
                    node = node.Right;
                }
            }
            return node;
        }


        public static BinaryTreeNode<T> GetPredecessorOf(BinaryTreeNode<T> node)
        {
            BinaryTreeNode<T> dummy;
            return GetPredecessorOf(node, out dummy);
        }

        public static BinaryTreeNode<T> GetPredecessorOf(BinaryTreeNode<T> node, out BinaryTreeNode<T> successorParent)
        {
            successorParent = null;
            if (node == null || node.Right == null) {
                return null;
            }
            return Max(node.Left, out successorParent);
        }

        public static BinaryTreeNode<T> GetSuccessorOf(BinaryTreeNode<T> node)
        {
            BinaryTreeNode<T> dummy;
            return GetSuccessorOf(node, out dummy);
        }

        public static BinaryTreeNode<T> GetSuccessorOf(BinaryTreeNode<T> node, out BinaryTreeNode<T> successorParent)
        {
            successorParent = null;
            if (node == null || node.Left == null) {
                return null;
            }
            return Min(node.Right, out successorParent);
        }

        public override int Count
        {
            get
            {
                return ToList().Count;
            }
        }

        public List<BinaryTreeNode<T>> ToList()
        {
            var nodes = new List<BinaryTreeNode<T>>();
            var parents = new Stack<BinaryTreeNode<T>>();
            Action<BinaryTreeNode<T>> descendLeft = delegate(BinaryTreeNode<T> node) {
                while (node != null) {
                    parents.Push(node);
                    node = node.Left;
                }
            };

            BinaryTreeNode<T> current = Root;
            descendLeft(current);
            while (parents.Count > 0) {
                current = parents.Pop();
                nodes.Add(current);
                descendLeft(current.Right);
            }

            return nodes;
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) {
                throw new ArgumentNullException("array is null.");
            }
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException("arrayIndex is less than 0.");
            }

            var nodes = ToList();

            if (array.Length - arrayIndex < nodes.Count) {
                throw new ArgumentException("The number of elements in the source ICollection<T> is greater" +  
                    "than the available space from arrayIndex to the end of the destination array.");
            }


            for (int i = 0; i + arrayIndex < array.Length && i < nodes.Count; i++) {
                array[i + arrayIndex] = nodes[i].Value;
            }
        }


        protected override IEnumerator<T> GenericEnumerable_GetEnumerator()
        {
            var nodes = ToList();

            foreach (var node in nodes) {
                yield return node.Value;
            }
        }
    }
}
