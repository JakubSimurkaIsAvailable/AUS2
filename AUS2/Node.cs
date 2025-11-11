using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AUS2
{
    public interface INode<T> where T : IData
    {
        INode<T>? GetLeftChild();
        INode<T>? GetRightChild();
        INode<T>? GetParent();
        void SetParent(INode<T>? node);
        void SetLeftChild(INode<T>? node);
        void SetRightChild(INode<T>? node);
        T Data { get; set; }
    }
    public class Node<T> : INode<T> where T : IData
    {
        protected Node<T>? LeftChild;
        protected Node<T>? RightChild;
        protected Node<T>? Parent;
        public T Data { get; set; }

        public Node(T value)
        {
            this.Data = value;
        }

        public INode<T>? GetLeftChild() => LeftChild;
        public INode<T>? GetRightChild() => RightChild;
        public INode<T>? GetParent() => Parent;

        public void SetParent(INode<T>? node)
        {
            Parent = node as Node<T>;
        }

        public void SetLeftChild(INode<T>? node)
        {
            LeftChild = node as Node<T>;
        }

        public void SetRightChild(INode<T>? node)
        {
            RightChild = node as Node<T>;
        }
    }


    public class AVLNode<T> : INode<T> where T : IData
    {
        public int BalanceFactor => GetBalanceFactor();
        public int Height { get; private set; } = 1;
        protected AVLNode<T>? LeftChild;
        protected AVLNode<T>? RightChild;
        protected AVLNode<T>? Parent;
        
        public T Data { get; set; }
        public AVLNode(T value)
        {
            
            this.Data = value;
        }

        public INode<T>? GetLeftChild() => LeftChild;

        public INode<T>? GetParent() => Parent;

        public INode<T>? GetRightChild() => RightChild;

        public void SetLeftChild(INode<T>? node)
        {
            LeftChild = node as AVLNode<T>;
        }

        public void SetParent(INode<T>? node)
        {
            Parent = node as AVLNode<T>;
        }

        public void SetRightChild(INode<T>? node)
        {
            RightChild = node as AVLNode<T>;
        }

        private int GetBalanceFactor()
        {
            int leftHeight = LeftChild?.Height ?? 0;
            int rightHeight = RightChild?.Height ?? 0;
            return rightHeight - leftHeight;
        }

        private int ComputeHeight()
        {
            return 1 + Math.Max(LeftChild?.Height ?? 0, RightChild?.Height ?? 0);
        }

        public void UpdateHeight()
        {
            Height = ComputeHeight();
        }
    }
}
