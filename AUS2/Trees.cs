
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AUS2
{
    public abstract class Tree<T> where T : IData
    {
        protected INode<T>? Root;
        protected T? DeleteParent;
        protected abstract INode<T> GetNodeFromData(T data);
        public abstract bool Delete(T data);
        public abstract bool Insert(T data);
        public abstract T? Find(T data); 
        
        public abstract T FindMax(T? node_data);
        public abstract T FindMin(T? node_data);
        public abstract List<T> FindInterval(T min, T max);
    }

    public class BST<T> : Tree<T> where T : IData
    {
        public BST()
        {

        }
        public override bool Delete(T data)
        {
            //Checks if anything inside the tree exists if not returns false
            if (Root is null)
            {
                return false;
            }

            //if (Find(data) is null) return false;

            var hlpNode = GetNodeFromData(data);
            //Checks if the node exists within the tree
            if (hlpNode is null)
            {
                return false;
            }

            if (hlpNode.GetLeftChild() is null && hlpNode.GetRightChild() is null)
            {
                var parent = hlpNode.GetParent();
                
                if (parent is null)
                {
                    Root = null;
                    return true;
                }
                else
                {
                    DeleteParent = parent.Data;
                    if (parent.GetLeftChild()?.Data.Compare(hlpNode.Data) == 0)
                    {
                        parent.SetLeftChild(null);
                        hlpNode = null;
                        return true;
                    }
                    else if (parent.GetRightChild()?.Data.Compare(hlpNode.Data) == 0)
                    {
                        parent.SetRightChild(null);
                        hlpNode = null;
                        return true;
                    }
                }

            }
            else if (hlpNode.GetRightChild() is not null)
            {

                var successor = hlpNode.GetRightChild()!;
                while (successor.GetLeftChild() is not null)
                {
                    successor = successor.GetLeftChild()!;
                }
                

                if (successor.Data.Compare(hlpNode.GetRightChild()!.Data) == 0)
                {
                    hlpNode.Data = successor.Data;
                    DeleteParent = successor.Data;
                    successor.SetParent(null);
                    hlpNode.SetRightChild(successor.GetRightChild());
                    successor.GetRightChild()?.SetParent(hlpNode);
                    successor.SetRightChild(null);
                    successor = null;
                    return true;
                }
                else
                {
                    DeleteParent = successor.GetParent().Data;
                    hlpNode.Data = successor.Data;
                    var successorParent = successor.GetParent()!;
                    successorParent.SetLeftChild(successor.GetRightChild());
                    successor.GetRightChild()?.SetParent(successorParent);
                    successor = null;
                    return true;
                }



            }
            else if (hlpNode.GetLeftChild() is not null)
            {

                var successor = hlpNode.GetLeftChild()!;
                while (successor.GetRightChild() is not null)
                {
                    successor = successor.GetRightChild()!;
                }
                DeleteParent = successor.GetParent().Data;
                if (successor.Data.Compare(hlpNode.GetLeftChild()!.Data) == 0)
                {
                    hlpNode.Data = successor.Data;
                    DeleteParent = successor.Data;
                    successor.SetParent(null);
                    hlpNode.SetLeftChild(successor.GetLeftChild());
                    successor.GetLeftChild()?.SetParent(hlpNode);
                    successor.SetLeftChild(null);
                    successor = null;
                    return true;
                }
                else
                {
                    DeleteParent = successor.GetParent().Data;
                    hlpNode.Data = successor.Data;
                    var successorParent = successor.GetParent()!;
                    successorParent.SetRightChild(successor.GetLeftChild());
                    successor.GetLeftChild()?.SetParent(successorParent);
                    successor = null;
                    return true;
                }

            }

            return false;

        }

        public override bool Insert(T data)
        {
            //if there is no root, create it
            if (Root is null)
            {
                Root = new Node<T>(data);
                return true;
            }
            //if (Find(data) is not null) return false;
            var node = Root;
            int cmp;
            while (true)
            {
                cmp = node.Data.Compare(data);
                if (cmp < 0)
                {
                    if (node.GetRightChild() == null)
                    {
                        var temp = new Node<T>(data);
                        node.SetRightChild(temp);
                        temp.SetParent(node);
                        return true;
                    }
                    node = node.GetRightChild()!;
                }
                else if (cmp > 0)
                {
                    if (node.GetLeftChild() == null)
                    {
                        var temp = new Node<T>(data);
                        node.SetLeftChild(temp);
                        temp.SetParent(node);
                        return true;
                    }
                    node = node.GetLeftChild()!;
                }
                else return false; // duplicate
            }

        }
        public override T? Find(T data)
        {
            if (Root is null) throw new InvalidOperationException("Tree is empty");
            else
            {
                var hlpNode = Root;
                while (hlpNode != null)
                {
                    int cmp = hlpNode.Data.Compare(data);
                    if (cmp == 0) return hlpNode.Data;
                    else if (cmp == -1) hlpNode = hlpNode.GetRightChild()!;
                    else if (cmp == 1) hlpNode = hlpNode.GetLeftChild()!;
                }
            }
            throw new InvalidOperationException("Data not found");
        }



        protected override INode<T> GetNodeFromData(T data)
        {
            var hlpNode = Root;
            while (hlpNode != null)
            {
                int cmp = hlpNode.Data.Compare(data);
                if (cmp == 0) return hlpNode;
                else if (cmp == -1) hlpNode = hlpNode.GetRightChild()!;
                else if (cmp == 1) hlpNode = hlpNode.GetLeftChild()!;
            }
            return hlpNode!;
        }

        public override T FindMax(T? node_data)
        {
            if (Root is null) throw new InvalidOperationException("Tree is empty");
            INode<T> node;
            if(node_data is null)
            {
                node = Root;
            } else
            {
                node = GetNodeFromData(node_data);
            }
            while (node.GetRightChild() is not null)
            {
                node = node.GetRightChild()!;
            }
            return node.Data;
        }

        public override T FindMin(T? node_data)
        {
            if (Root is null) throw new InvalidOperationException("Tree is empty");
            INode<T> node;
            if (node_data is null)
            {
                node = Root;
            }
            else
            {
                node = GetNodeFromData(node_data);
            }
            while (node.GetLeftChild() is not null)
            {
                node = node.GetLeftChild()!;
            }
            return node.Data;
        }
       
        private INode<T> GetClosestNodeToMin(T min)
        {
            var node = Root;
            INode<T> closest = null;
            while (node != null)
            {
                int cmp = node.Data.Compare(min);
                if (cmp == 0) return node;
                if (cmp > 0)
                {
                    closest = node;
                    node = node.GetLeftChild();
                }
                else
                {
                    node = node.GetRightChild();
                }
            }
            return closest;
        }

        public override List<T> FindInterval(T min, T max)
        {
            if (Root is null) throw new InvalidOperationException("Tree is empty");
            var result = new List<T>();
            if (min.Compare(max) == 0)
            {
                result.Add(min);
                return result;
            }
            var current = GetClosestNodeToMin(min);
            while (current != null)
            {

                if (current.Data.Compare(max) > 0)
                    break;
                result.Add(current.Data);
                // Move to the next node in in-order traversal
                if (current.GetRightChild() != null)
                {
                    current = current.GetRightChild();
                    while (current.GetLeftChild() != null)
                    {
                        current = current.GetLeftChild();
                    }
                }
                else
                {
                    var parent = current.GetParent();
                    while (parent != null && current == parent.GetRightChild())
                    {
                        current = parent;
                        parent = parent.GetParent();
                    }
                    current = parent;
                }

            }
            return result;
        }
        public void WriteIntoCSV(string filename)
        {
            var csv = new StringBuilder();
            Queue<INode<T>> queue = new Queue<INode<T>>();
            queue.Enqueue(Root);
            while(queue.Count > 0)
            {
                INode<T> temp = queue.Dequeue();
                csv.Append(temp.Data.ToString());
                if(temp.GetLeftChild() is not null)
                {
                    queue.Enqueue(temp.GetLeftChild());
                }
                if(temp.GetRightChild() is not null)
                {
                    queue.Enqueue(temp.GetRightChild());
                }
            }
            File.WriteAllText(filename, csv.ToString());
        }
        public void LoadFromCSV(string filename, T t)
        {
            var content = File.ReadAllText(filename);
            var lines = content.Split('\n');

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                T data = (T)t.toData(line);
                this.Insert(data);
            }

        }
    }
        public class AVL<T> : BST<T> where T : IData
    {
        public AVL() { }

        public override bool Insert(T data)
        {
            if (Root is null)
            {
                Root = new AVLNode<T>(data);
                return true;
            }

            var node = Root;
            AVLNode<T> parent = null;

            
            while (node != null)
            {
                parent = (AVLNode<T>)node;
                int cmp = node.Data.Compare(data);

                if (cmp < 0)
                    node = node.GetRightChild();
                else if (cmp > 0)
                    node = node.GetLeftChild();
                else
                    return false; // duplicate value
            }

            
            var newNode = new AVLNode<T>(data);
            newNode.SetParent(parent);

            int parentCmp = parent.Data.Compare(data);
            if (parentCmp < 0)
                parent.SetRightChild(newNode);
            else
                parent.SetLeftChild(newNode);

            
            var current = parent;

            current.UpdateHeight();
            bool first = false;
            current = (AVLNode<T>)current.GetParent();
            while (current is not null)
            {
                current.UpdateHeight();
                int balance = current.BalanceFactor;
                if((balance == -2 || balance == 2 || balance == 0) && !first)
                {
                    first = true;
                    if (balance != 0)
                    {
                        Rebalance(current);
                    }
                    break;
                }
                current = (AVLNode<T>)current.GetParent();
            }
            
            
            return true;
        }


        public override bool Delete(T data)
        {
            // Perform actual deletion (base handles restructuring)
            bool deleted = base.Delete(data);
            if (!deleted)
                return false;
            var rebalanceStart = (AVLNode<T>)GetNodeFromData(DeleteParent);
            // Rebalance going upward
            var current = rebalanceStart;
            bool foundZero = false;
            while (current is not null)
            {
                current.UpdateHeight();
                int balance = current.BalanceFactor;
                if(balance == -1 || balance == 1)
                {
                    foundZero = true;
                    break;
                }
                if (!foundZero)
                {
                    Rebalance(current);
                }
                current = (AVLNode<T>)current.GetParent();
            }
            return true;
        }
        private AVLNode<T> Rebalance(AVLNode<T> node)
        {
            //node.UpdateHeight();
            int balance = node.BalanceFactor;

            // Right-heavy
            if (balance > 1)
            {
                var right = (AVLNode<T>)node.GetRightChild()!;
                if (right.BalanceFactor < 0)
                {
                    // Right-Left case
                    node.SetRightChild(RotateRight(right));
                }
                node = RotateLeft(node);
            }
            // Left-heavy
            else if (balance < -1)
            {
                var left = (AVLNode<T>)node.GetLeftChild()!;
                if (left.BalanceFactor > 0)
                {
                    // Left-Right case
                    node.SetLeftChild(RotateLeft(left));
                }
                node = RotateRight(node);
            }

            return node;
        }

        protected AVLNode<T> RotateLeft(INode<T> node)
        {
            if (Root is null) return null!;

            var oldTop = (AVLNode<T>)node;
            var newTop = (AVLNode<T>)oldTop.GetRightChild()!;
            var newRight = newTop.GetLeftChild();

            // Perform rotation
            newTop.SetLeftChild(oldTop);
            oldTop.SetRightChild(newRight);

            // Update parents
            newTop.SetParent(oldTop.GetParent());
            oldTop.SetParent(newTop);
            if (newRight != null)
                newRight.SetParent(oldTop);

            // Reconnect newTop with oldTop parent
            if (newTop.GetParent() != null)
            {
                var parent = newTop.GetParent()!;
                if (parent.GetLeftChild() == oldTop)
                    parent.SetLeftChild(newTop);
                else
                    parent.SetRightChild(newTop);
            }
            else
            {
                Root = newTop;
                newTop.SetParent(null);
            }

            // Update heights
            oldTop.UpdateHeight();
            newTop.UpdateHeight();

            return newTop;
        }

        protected AVLNode<T> RotateRight(INode<T> node)
        {
            if (Root is null) return null!;

            var oldTop = (AVLNode<T>)node;
            var newTop = (AVLNode<T>)oldTop.GetLeftChild()!;
            var newLeft = newTop.GetRightChild();

            // Perform rotation
            newTop.SetRightChild(oldTop);
            oldTop.SetLeftChild(newLeft);

            // Update parents
            newTop.SetParent(oldTop.GetParent());
            oldTop.SetParent(newTop);
            if (newLeft != null)
                newLeft.SetParent(oldTop);

            // Reconnect newTop with oldTop's parent
            if (newTop.GetParent() != null)
            {
                var parent = newTop.GetParent()!;
                if (parent.GetRightChild() == oldTop)
                    parent.SetRightChild(newTop);
                else
                    parent.SetLeftChild(newTop);
            }
            else
            {
                Root = newTop;
                newTop.SetParent(null);
            }

            // Update heights
            oldTop.UpdateHeight();
            newTop.UpdateHeight();

            return newTop;
        }

        public bool IsAVL()
        {
            var min = FindMin(default);
            var max = FindMax(default);
            
            var current = (AVLNode<T>)GetNodeFromData(min);
            while (current != null)
            {
                
                if (current.Data.Compare(max) >= 0)
                    break;
                current.UpdateHeight();
                int checkHeight = RecursivelyCheckHeight(current);
                if (checkHeight != current.Height)
                {
                    return false;
                }
                if (current.BalanceFactor > 1 || current.BalanceFactor < -1)
                {
                    return false;
                }
                // Move to the next node in in-order traversal
                if (current.GetRightChild() != null)
                {
                    current = (AVLNode<T>)current.GetRightChild();
                    while (current.GetLeftChild() != null)
                    {
                        current = (AVLNode<T>)current.GetLeftChild();
                    }
                }
                else
                {
                    var parent = (AVLNode<T>)current.GetParent();
                    while (parent != null && current == parent.GetRightChild())
                    {
                        current = parent;
                        parent = (AVLNode<T>)parent.GetParent();
                    }
                    current = parent;
                }
            } 
            return true;
        }
        private int RecursivelyCheckHeight(INode<T> node)
        {
            if (node is null)
                return 0;
            return 1 + Math.Max(RecursivelyCheckHeight(node.GetLeftChild()), RecursivelyCheckHeight(node.GetRightChild()));
        }
    }

}
