using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class Pracovisko : IData
    {
        public int KodPracoviska { get; private set; }
        public AVL<PCRTestByDate> TrueTestyDatum { get; private set; } = new AVL<PCRTestByDate>();
        public AVL<PCRTestByDate> FalseTestyDatum { get; private set; } = new AVL<PCRTestByDate>();

        public Pracovisko(int kodPracoviska)
        {
            KodPracoviska = kodPracoviska;
        }
        public Pracovisko()
        {

        }
        public int Compare(IData otherData)
        {
            return KodPracoviska.CompareTo(((Pracovisko)otherData).KodPracoviska);
        }
        public IData toData(string line)
        {
            var split = line.Split(';');
            int kodPracoviska = int.Parse(split[0]);
            return new Pracovisko(kodPracoviska);
        }
        public override string ToString()
        {
            return $"{KodPracoviska};\n";
        }
    }
}
