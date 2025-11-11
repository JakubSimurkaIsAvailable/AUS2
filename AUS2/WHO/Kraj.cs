using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class Kraj : IData
    {
        public AVL<PCRTestByDate> TrueTestyDatum { get; private set; } = new AVL<PCRTestByDate>();
        public AVL<PCRTestByDate> FalseTestyDatum { get; private set; } = new AVL<PCRTestByDate>();

        public int KodKraja { get; private set; }
        public Kraj(int kodKraja)
        {
            KodKraja = kodKraja;
        }
        public Kraj()
        {

        }

        public int Compare(IData otherData)
        {
            return KodKraja.CompareTo(((Kraj)otherData).KodKraja);
        }
        public IData toData(string line)
        {
            var split = line.Split(';');
            int kodKraja = int.Parse(split[0]);
            return new Kraj(kodKraja);
        }
        public override string ToString()
        {
            return $"{KodKraja};\n";
        }
    }
}
