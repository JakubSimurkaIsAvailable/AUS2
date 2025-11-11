using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySearchTreeLib
{
    public interface IBSTNode<T> where T : IBSTData
    {
        public T Data { get; }
        protected IBSTNode<T>? LeftSon { get; set; }
        protected IBSTNode<T>? RightSon { get; set; }
        protected IBSTNode<T>? Parent { get; set; }
        
    }
}
