using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class Person : IData
    {
        public string Meno { get; private set; }
        public string Priezvisko { get; private set; }
        public DateTime DatumNarodenia { get; private set; }
        public string UnikatneCisloPacienta { get; private set; }

        public AVL<PCRTest> PCRTests { get; private set; } = new AVL<PCRTest>();
        public AVL<PCRTestByDate> PCRTestsByDate { get; private set; } = new AVL<PCRTestByDate>();

        public Person()
        {

        }
        public Person(string meno, string priezvisko, DateTime datumNarodenia) 
        {
            Meno = meno;
            Priezvisko = priezvisko;
            DatumNarodenia = datumNarodenia;
            
        }
        private Person(string meno, string priezvisko, DateTime datumNarodenia, string unikatneCisloPacienta)
        {
            Meno = meno;
            Priezvisko = priezvisko;
            DatumNarodenia = datumNarodenia;
            UnikatneCisloPacienta = unikatneCisloPacienta;
        }

        public Person(string unikatneCisloPacienta)
        {
            UnikatneCisloPacienta = unikatneCisloPacienta;
        }

        public Person(Person other)
        {
            Meno = other.Meno;
            Priezvisko = other.Priezvisko;
            DatumNarodenia = other.DatumNarodenia;
            UnikatneCisloPacienta = other.UnikatneCisloPacienta;
            PCRTests = other.PCRTests;
            PCRTestsByDate = other.PCRTestsByDate;
        }

        public void SetUnikatneCisloPacienta(string unikatneCisloPacienta)
        {
            UnikatneCisloPacienta = unikatneCisloPacienta;
        }
        public int Compare(IData otherData)
        {
            int value = string.Compare(UnikatneCisloPacienta, ((Person)otherData).UnikatneCisloPacienta, StringComparison.Ordinal);
            if (value == 0)
            {
                return 0;
            }
            else if (value < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public IData toData(string line)
        {
            string[] parts = line.Split(';');
            string meno = parts[0];
            string priezvisko = parts[1];
            DateTime datumNarodenia = DateTime.Parse(parts[2]);
            string unikatneCisloPacienta = parts[3];
            return new Person(meno, priezvisko, datumNarodenia, unikatneCisloPacienta);
        }
        public override string ToString()
        {
            return $"{Meno};{Priezvisko};{DatumNarodenia.ToShortDateString()};{UnikatneCisloPacienta}\n";
        }
    }
}
