using System;
using System.Collections.Generic;

namespace Trees
{
    internal class BinarySearchTreeNode<T> : BinaryTreeNode<T> where T : IComparable<T>, ICloneable
    {
        private bool RemoveUsingPredecessor = false;

        public BinarySearchTreeNode() : base() { }
        public BinarySearchTreeNode(T data) : base(data, null) { }
        public BinarySearchTreeNode(T data, BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
            : base(data, new NodeList<T> { left, right }) { }

        public new BinarySearchTreeNode<T> Left
        {
            get
            {
                if (base.Neighbors == null) {
                    return null;
                } else {
                    return (BinarySearchTreeNode<T>) base.Neighbors[0];
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

        public new BinarySearchTreeNode<T> Right
        {
            get
            {
                if (base.Neighbors == null) {
                    return null;
                } else {
                    return (BinarySearchTreeNode<T>) base.Neighbors[1];
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

        public delegate object VisitNode(params object[] arguments);
        //arguments[0]: input data
        //arguments[1]: null or node containing arguments[0]
        //arguments[2]: traceback stack of parents
        //arguments[3]: true if arguments[1] is Left child of arguments[2]
        public struct VisitNodeArgument
        {
            public const int Data = 0,
            Node = 1,
            Parents = 2,
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
            object[] arguments = { data, this, new Stack<BinarySearchTreeNode<T>>(), false };
            BinarySearchTreeNode<T> previous = null;

            for (BinaryTreeNode<T> current = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Node];
                current != null;
                arguments[VisitNodeArgument.Node] = current) {
                int result = current.Value.CompareTo(data);

                if (result == 0) {
                    return onSuccess(data, current, previous, wentLeft);
                } else {
                    ((Stack<BinarySearchTreeNode<T>>)
                        arguments[VisitNodeArgument.Parents]).
                        Push((BinarySearchTreeNode<T>) current);

                    if (result > 0) {
                        arguments[VisitNodeArgument.WentLeft] = true;
                        current = (BinarySearchTreeNode<T>) current.Left;
                    } else {
                        arguments[VisitNodeArgument.WentLeft] = false;
                        current = (BinarySearchTreeNode<T>) current.Right;
                    }
                }
            }

            return onFailure(arguments);
        }

        public virtual bool Contains(T data)
        {
            return (bool) FindAndVisit(data, VisitReturnTrue, VisitReturnFalse);
        }

        internal virtual BinarySearchTreeNode<T> Find(T data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return arguments[VisitNodeArgument.Node];
                };

            return (BinarySearchTreeNode<T>) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        internal virtual Stack<BinarySearchTreeNode<T>> GetParentOf(T data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return ((Stack<BinarySearchTreeNode<T>>) arguments[VisitNodeArgument.Parents]);
                };

            return (Stack<BinarySearchTreeNode<T>>) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        public virtual void Add(T data)
        {
            VisitNode onFailure =
                (object[] arguments) => {
                    T leafData = (T) arguments[VisitNodeArgument.Data];
                    var leaf = new BinarySearchTreeNode<T>(leafData);

                    var parent = ((Stack<BinarySearchTreeNode<T>>) arguments[VisitNodeArgument.Parents]).Pop();
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    if (wentLeft) {
                        parent.Left = leaf;
                    } else {
                        parent.Right = leaf;
                    }

                    return true;
                };

            FindAndVisit(data, VisitReturnFalse, onFailure);
        }


        public virtual bool Remove(T data)
        {
            return Remove(data, !(RemoveUsingPredecessor = !RemoveUsingPredecessor));
        }

        public bool Remove(T data, bool removeUsingPredecessor)
        {
            VisitNode onSuccessUsingPredecessor =
                 (object[] arguments) => {
                     var node = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Node];
                     var parent = ((Stack<BinarySearchTreeNode<T>>) arguments[VisitNodeArgument.Parents]).Pop();
                     var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                     if (node.Left == null) {
                         if (!wentLeft) {
                             parent.Right = node.Right;
                         } else {
                             parent.Left = node.Right;
                         }
                     } else {
                         BinarySearchTreeNode<T> predecessorParent, predecessor = node.GetPredecessorNode(out predecessorParent);

                         predecessorParent.Right = predecessor.Left;

                         predecessor.CopyChildrenOf(node);

                         if (!wentLeft) {
                             parent.Right = predecessor;
                         } else {
                             parent.Left = predecessor;
                         }
                     }

                     return true;
                 };
            VisitNode onSuccessUsingSuccessor =
                (object[] arguments) => {
                    var node = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Node];
                    var parent = ((Stack<BinarySearchTreeNode<T>>) arguments[VisitNodeArgument.Parents]).Pop();
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    if (node.Right == null) {
                        if (wentLeft) {
                            parent.Left = node.Left;
                        } else {
                            parent.Right = node.Left;
                        }
                    } else {
                        BinarySearchTreeNode<T> successorParent, successor = node.GetSuccessorNode(out successorParent);

                        successorParent.Left = successor.Right;

                        successor.CopyChildrenOf(node);

                        if (wentLeft) {
                            parent.Left = successor;
                        } else {
                            parent.Right = successor;
                        }
                    }

                    return true;
                };

            return (bool) FindAndVisit(data,
                removeUsingPredecessor ? onSuccessUsingPredecessor : onSuccessUsingSuccessor,
                VisitReturnFalse);
        }

        internal virtual T Min()
        {
            BinarySearchTreeNode<T> dummy;
            return Min(out dummy);
        }

        internal T Min(out BinarySearchTreeNode<T> parent)
        {
            return MinNode(out parent).Value;
        }

        internal BinarySearchTreeNode<T> MinNode(out BinarySearchTreeNode<T> parent)
        {
            parent = null;
            var node = this;
            while (node.Left != null) {
                parent = node;
                node = (BinarySearchTreeNode<T>) node.Left;
            }
            return node;
        }

        internal virtual T Max()
        {
            BinarySearchTreeNode<T> dummy;
            return Max(out dummy);
        }

        internal T Max(out BinarySearchTreeNode<T> parent)
        {
            return MaxNode(out parent).Value;
        }

        internal BinarySearchTreeNode<T> MaxNode(out BinarySearchTreeNode<T> parent)
        {
            parent = null;
            var node = this;
            while (node.Right != null) {
                parent = node;
                node = (BinarySearchTreeNode<T>) node.Right;
            }
            return node;
        }

        internal T GetPredecessor()
        {
            if (Left == null) {
                return default(T);
            }
            return Left.Max();
        }

        internal BinarySearchTreeNode<T> GetPredecessorNode()
        {
            BinarySearchTreeNode<T> dummy;
            return GetPredecessorNode(out dummy);
        }

        internal BinarySearchTreeNode<T> GetPredecessorNode(out BinarySearchTreeNode<T> parent)
        {
            if (Left == null) {
                parent = null;
                return null;
            }
            return Left.MaxNode(out parent);
        }

        internal T GetSuccessor()
        {
            if (Right == null) {
                return default(T);
            }
            return Right.Min();
        }

        internal BinarySearchTreeNode<T> GetSuccessorNode()
        {
            BinarySearchTreeNode<T> dummy;
            return GetSuccessorNode(out dummy);
        }

        internal BinarySearchTreeNode<T> GetSuccessorNode(out BinarySearchTreeNode<T> parent)
        {
            if (Right == null) {
                parent = null;
                return null;
            }
            return Right.MinNode(out parent);
        }
    }
}
