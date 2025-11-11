using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class Okres : IData
    {
        public AVL<PCRTestByDate> TrueTestyDatum { get; private set; } = new AVL<PCRTestByDate>();
        public AVL<PCRTestByDate> FalseTestyDatum { get; private set; } = new AVL<PCRTestByDate>();
        public int KodOkresu { get; private set; }
        public Okres(int kodOkresu)
        {
            KodOkresu = kodOkresu;
        }
        public Okres()
        {

        }
        public int Compare(IData otherData)
        {
            return KodOkresu.CompareTo(((Okres)otherData).KodOkresu);
        }

        public IData toData(string line)
        {
            var split = line.Split(';');
            int kodOkresu = int.Parse(split[0]);
            return new Okres(kodOkresu);
        }
        public override string ToString()
        {
            return $"{KodOkresu};\n";
        }
    }
}
