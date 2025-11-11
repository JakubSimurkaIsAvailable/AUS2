using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2
{
    public interface IData
    {
        int Compare(IData otherData);
        IData toData(string line);
    }
}
