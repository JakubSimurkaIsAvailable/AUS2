using System;
using System.Collections.Generic;

namespace AUS2
{
    public enum RBColor { Red, Black }

    public class RBNode<T> : INode<T> where T : IData
    {
        public T Data { get; set; }
        public RBNode<T>? Left;
        public RBNode<T>? Right;
        public RBNode<T>? Parent;
        public RBColor Color;

        public RBNode(T value)
        {
            Data = value;
            Color = RBColor.Red;
        }

        public INode<T>? GetLeftChild() => Left;
        public INode<T>? GetRightChild() => Right;
        public INode<T>? GetParent() => Parent;
        public void SetParent(INode<T>? node) => Parent = (RBNode<T>?)node;
        public void SetLeftChild(INode<T>? node) => Left = (RBNode<T>?)node;
        public void SetRightChild(INode<T>? node) => Right = (RBNode<T>?)node;
    }

    public class RBTree<T> : Tree<T> where T : IData
    {
        protected new RBNode<T>? Root;

        protected override INode<T> GetNodeFromData(T data)
        {
            var node = Root;
            while (node != null)
            {
                int cmp = node.Data.Compare(data);
                if (cmp == 0) return node;
                node = cmp < 0 ? node.Right : node.Left;
            }
            // return null-like - caller must handle
            return null!;
        }

        private static bool IsRed(RBNode<T>? n) => n != null && n.Color == RBColor.Red;
        private static bool IsBlack(RBNode<T>? n) => !IsRed(n);

        private void RotateLeft(RBNode<T> x)
        {
            var y = x.Right ?? throw new InvalidOperationException("RotateLeft: right child null");
            x.SetRightChild(y.Left);
            if (y.Left != null) y.Left.SetParent(x);
            y.SetParent(x.Parent);
            if (x.Parent == null) Root = y;
            else if (x == x.Parent!.Left) x.Parent!.SetLeftChild(y);
            else x.Parent!.SetRightChild(y);
            y.SetLeftChild(x);
            x.SetParent(y);
        }

        private void RotateRight(RBNode<T> x)
        {
            var y = x.Left ?? throw new InvalidOperationException("RotateRight: left child null");
            x.SetLeftChild(y.Right);
            if (y.Right != null) y.Right.SetParent(x);
            y.SetParent(x.Parent);
            if (x.Parent == null) Root = y;
            else if (x == x.Parent!.Right) x.Parent!.SetRightChild(y);
            else x.Parent!.SetLeftChild(y);
            y.SetRightChild(x);
            x.SetParent(y);
        }

        public override bool Insert(T data)
        {
            var z = new RBNode<T>(data);
            RBNode<T>? y = null;
            var x = Root;

            while (x != null)
            {
                y = x;
                int cmp = x.Data.Compare(z.Data);
                if (cmp == 0) return false; // duplicate
                x = cmp < 0 ? x.Right : x.Left;
            }

            z.SetParent(y);
            if (y == null) Root = z;
            else if (y.Data.Compare(z.Data) < 0) y.SetRightChild(z);
            else y.SetLeftChild(z);

            // fixup
            InsertFixup(z);
            return true;
        }

        private void InsertFixup(RBNode<T> z)
        {
            while (IsRed((RBNode<T>?)z.GetParent()))
            {
                var p = (RBNode<T>?)z.GetParent();
                var g = (RBNode<T>?)p?.GetParent();
                if (g == null) break;

                if (p == g.Left)
                {
                    var y = (RBNode<T>?)g.Right;
                    if (IsRed(y))
                    {
                        p.Color = RBColor.Black;
                        y!.Color = RBColor.Black;
                        g.Color = RBColor.Red;
                        z = g;
                    }
                    else
                    {
                        if (z == p.Right)
                        {
                            z = p;
                            RotateLeft(z);
                            p = (RBNode<T>?)z.GetParent();
                            g = (RBNode<T>?)p?.GetParent();
                        }
                        p!.Color = RBColor.Black;
                        if (g != null)
                        {
                            g.Color = RBColor.Red;
                            RotateRight(g);
                        }
                    }
                }
                else
                {
                    var y = (RBNode<T>?)g.Left;
                    if (IsRed(y))
                    {
                        p.Color = RBColor.Black;
                        y!.Color = RBColor.Black;
                        g.Color = RBColor.Red;
                        z = g;
                    }
                    else
                    {
                        if (z == p.Left)
                        {
                            z = p;
                            RotateRight(z);
                            p = (RBNode<T>?)z.GetParent();
                            g = (RBNode<T>?)p?.GetParent();
                        }
                        p!.Color = RBColor.Black;
                        if (g != null)
                        {
                            g.Color = RBColor.Red;
                            RotateLeft(g);
                        }
                    }
                }
            }
            if (Root != null) Root.Color = RBColor.Black;
        }

        private RBNode<T>? Minimum(RBNode<T>? node)
        {
            if (node == null) return null;
            while (node.Left != null) node = node.Left;
            return node;
        }

        private RBNode<T>? Transplant(RBNode<T>? u, RBNode<T>? v)
        {
            if (u?.Parent == null)
            {
                Root = v;
            }
            else if (u == u.Parent!.Left)
            {
                u.Parent!.SetLeftChild(v);
            }
            else
            {
                u.Parent!.SetRightChild(v);
            }
            if (v != null) v.SetParent(u?.Parent);
            return v;
        }

        public override bool Delete(T data)
        {
            // find node
            var z = Root;
            while (z != null)
            {
                int cmp = z.Data.Compare(data);
                if (cmp == 0) break;
                z = cmp < 0 ? z.Right : z.Left;
            }

            if (z == null) return false;

            var y = z;
            var yOriginalColor = y.Color;
            RBNode<T>? x;

            if (z.Left == null)
            {
                x = z.Right;
                Transplant(z, z.Right);
            }
            else if (z.Right == null)
            {
                x = z.Left;
                Transplant(z, z.Left);
            }
            else
            {
                y = Minimum(z.Right);
                if (y == null) return false; // should not happen
                yOriginalColor = y.Color;
                x = y.Right;
                if (y.Parent == z)
                {
                    if (x != null) x.SetParent(y);
                }
                else
                {
                    Transplant(y, y.Right);
                    y.SetRightChild(z.Right);
                    y.Right!.SetParent(y);
                }
                Transplant(z, y);
                y.SetLeftChild(z.Left);
                y.Left!.SetParent(y);
                y.Color = z.Color;
            }

            if (yOriginalColor == RBColor.Black)
            {
                DeleteFixup(x, z.Parent);
            }
            return true;
        }

        // pass parentHint to support case when x == null (we still need parent to continue fixup)
        private void DeleteFixup(RBNode<T>? x, INode<T>? parentHint)
        {
            RBNode<T>? parent = (RBNode<T>?)parentHint;
            while (x != Root && IsBlack(x))
            {
                if (parent == null)
                {
                    // try to set parent from x if available
                    parent = x?.Parent;
                    if (parent == null) break;
                }

                if (x == parent.Left)
                {
                    var w = (RBNode<T>?)parent.Right;
                    if (IsRed(w))
                    {
                        w!.Color = RBColor.Black;
                        parent.Color = RBColor.Red;
                        RotateLeft(parent);
                        parent = (RBNode<T>?)x?.GetParent();
                        w = (RBNode<T>?)parent?.Right;
                    }
                    if (IsBlack(w?.Left as RBNode<T>) && IsBlack(w?.Right as RBNode<T>))
                    {
                        if (w != null) w.Color = RBColor.Red;
                        x = parent;
                        parent = (RBNode<T>?)x?.GetParent();
                    }
                    else
                    {
                        if (IsBlack(w?.Right as RBNode<T>))
                        {
                            if (w?.Left != null) w.Left.Color = RBColor.Black;
                            if (w != null) w.Color = RBColor.Red;
                            RotateRight(w!);
                            w = (RBNode<T>?)parent?.Right;
                        }
                        if (w != null)
                        {
                            w.Color = parent!.Color;
                            parent.Color = RBColor.Black;
                            if (w.Right != null) w.Right.Color = RBColor.Black;
                        }
                        RotateLeft(parent!);
                        x = Root;
                        break;
                    }
                }
                else
                {
                    var w = (RBNode<T>?)parent.Left;
                    if (IsRed(w))
                    {
                        w!.Color = RBColor.Black;
                        parent.Color = RBColor.Red;
                        RotateRight(parent);
                        parent = (RBNode<T>?)x?.GetParent();
                        w = (RBNode<T>?)parent?.Left;
                    }
                    if (IsBlack(w?.Right as RBNode<T>) && IsBlack(w?.Left as RBNode<T>))
                    {
                        if (w != null) w.Color = RBColor.Red;
                        x = parent;
                        parent = (RBNode<T>?)x?.GetParent();
                    }
                    else
                    {
                        if (IsBlack(w?.Left as RBNode<T>))
                        {
                            if (w?.Right != null) w.Right.Color = RBColor.Black;
                            if (w != null) w.Color = RBColor.Red;
                            RotateLeft(w!);
                            w = (RBNode<T>?)parent?.Left;
                        }
                        if (w != null)
                        {
                            w.Color = parent!.Color;
                            parent.Color = RBColor.Black;
                            if (w.Left != null) w.Left.Color = RBColor.Black;
                        }
                        RotateRight(parent!);
                        x = Root;
                        break;
                    }
                }
            }
            if (x != null) x.Color = RBColor.Black;
        }

        public override T? Find(T data)
        {
            var node = Root;
            while (node != null)
            {
                int cmp = node.Data.Compare(data);
                if (cmp == 0) return data;
                node = cmp < 0 ? node.Right : node.Left;
            }
            throw new InvalidOperationException("Data not found in tree");
        }

        public override T FindMax(T? node_data)
        {
            if (Root == null) throw new InvalidOperationException("Tree is empty");
            RBNode<T>? node;
            if (node_data == null) node = Root;
            else
            {
                var n = (RBNode<T>)GetNodeFromData(node_data);
                node = n ?? Root;
            }
            while (node!.Right != null) node = node.Right;
            return node.Data;
        }

        public override T FindMin(T? node_data)
        {
            if (Root == null) throw new InvalidOperationException("Tree is empty");
            RBNode<T>? node;
            if (node_data == null) node = Root;
            else
            {
                var n = (RBNode<T>)GetNodeFromData(node_data);
                node = n ?? Root;
            }
            while (node!.Left != null) node = node.Left;
            return node.Data;
        }

        public override List<T> FindInterval(T min, T max)
        {
            var result = new List<T>();
            if (Root == null) throw new InvalidOperationException("Tree is empty");
            if (min.Compare(max) > 0) return result;

            var stack = new Stack<RBNode<T>?>();
            var node = Root;
            while (stack.Count > 0 || node != null)
            {
                while (node != null)
                {
                    int cmpMin = node.Data.Compare(min);
                    if (cmpMin >= 0)
                    {
                        stack.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        node = node.Right;
                    }
                }

                if (stack.Count == 0) break;
                node = stack.Pop();
                int cmpLow = node.Data.Compare(min);
                int cmpHigh = node.Data.Compare(max);
                if (cmpLow >= 0 && cmpHigh <= 0) result.Add(node.Data);
                if (cmpHigh > 0) break;
                node = node.Right;
            }

            return result;
        }
    }
}
