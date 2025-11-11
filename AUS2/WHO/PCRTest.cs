using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class PCRTest : IData
    {
        public DateTime DatumCas { get; private set; }
        public string UnikatneCisloPacienta { get; private set; }
        public int UnikatnyNahodnyKodPCRTestu { get; private set; }
        public int KodPracoviska { get; private set; }
        public int KodOkresu { get; private set; }
        public int KodKraja { get; private set; }
        public bool VysledokTestu { get; private set; }
        public double HodnotaTestu { get; private set; }
        public string Poznamka { get; private set; }
        public Person Pacient { get; set; }

        public PCRTest()
        {

        }
        public PCRTest(DateTime datumCas, string unikatneCisloPacienta, int kodPracoviska, int kodOkresu, int kodKraja, bool vysledokTestu, double hodnotaTestu, string poznamka) 
        { 
            DatumCas = datumCas;
            UnikatneCisloPacienta = unikatneCisloPacienta;
            
            KodPracoviska = kodPracoviska;
            KodOkresu = kodOkresu;
            KodKraja = kodKraja;
            VysledokTestu = vysledokTestu;
            HodnotaTestu = hodnotaTestu;
            Poznamka = poznamka;
            UnikatnyNahodnyKodPCRTestu = 0;
        }
        public PCRTest(DateTime datumCas, string unikatneCisloPacienta, int unikatnyNahodnyKodPCRTestu,  int kodPracoviska, int kodOkresu, int kodKraja, bool vysledokTestu, double hodnotaTestu, string poznamka)
        {
            DatumCas = datumCas;
            UnikatneCisloPacienta = unikatneCisloPacienta;
            UnikatnyNahodnyKodPCRTestu = unikatnyNahodnyKodPCRTestu;
            KodPracoviska = kodPracoviska;
            KodOkresu = kodOkresu;
            KodKraja = kodKraja;
            VysledokTestu = vysledokTestu;
            HodnotaTestu = hodnotaTestu;
            Poznamka = poznamka;
        }
        public PCRTest(int unikatnyNahodnyKodPCRTestu)
        {
            UnikatnyNahodnyKodPCRTestu = unikatnyNahodnyKodPCRTestu;
        }

        public PCRTest(PCRTest other)
        {
            DatumCas = other.DatumCas;
            UnikatneCisloPacienta = other.UnikatneCisloPacienta;
            UnikatnyNahodnyKodPCRTestu = other.UnikatnyNahodnyKodPCRTestu;
            KodPracoviska = other.KodPracoviska;
            KodOkresu = other.KodOkresu;
            KodKraja = other.KodKraja;
            VysledokTestu = other.VysledokTestu;
            HodnotaTestu = other.HodnotaTestu;
            Poznamka = other.Poznamka;
            Pacient = other.Pacient is null ? null! : new Person(other.Pacient);
        }

        public PCRTest(DateTime datumCas, int unikatnyNahodnyKodPCRTestu)
        {
            DatumCas = datumCas;
            UnikatnyNahodnyKodPCRTestu = unikatnyNahodnyKodPCRTestu;
        }

        public void SetUnikatnyNahodnyKodPCRTestu(int kod)
        {
            if (UnikatnyNahodnyKodPCRTestu == 0)
            {
                UnikatnyNahodnyKodPCRTestu = kod;
            }
        }

        public  int Compare(IData otherData)
        {
            return UnikatnyNahodnyKodPCRTestu.CompareTo(((PCRTest)otherData).UnikatnyNahodnyKodPCRTestu);
        }

        public IData toData(string line)
        {
            string[] parts = line.Split(';');
            DateTime datumCas = DateTime.Parse(parts[0]);
            string unikatneCisloPacienta = parts[1];
            int unikatnyNahodnyKodPCRTestu = int.Parse(parts[2]);
            int kodPracoviska = int.Parse(parts[3]);
            int kodOkresu = int.Parse(parts[4]);
            int kodKraja = int.Parse(parts[5]);
            bool vysledokTestu = bool.Parse(parts[6]);
            double hodnotaTestu = double.Parse(parts[7]);
            string poznamka = parts[8];
            return new PCRTest(datumCas, unikatneCisloPacienta, unikatnyNahodnyKodPCRTestu, kodPracoviska, kodOkresu, kodKraja, vysledokTestu, hodnotaTestu, poznamka);
        }
        public override string ToString()
        {
            return $"{DatumCas};{UnikatneCisloPacienta};{UnikatnyNahodnyKodPCRTestu};{KodPracoviska};{KodOkresu};{KodKraja};{VysledokTestu};{HodnotaTestu};{Poznamka};\n";
        }
    }

    public class PCRTestByDate : IData
    {
        public PCRTest Inner { get; private set; }
        public PCRTestByDate(PCRTest inner)
        {
            Inner = inner;
        }
        public int Compare(IData otherData)
        {
            int result = Inner.DatumCas.CompareTo(((PCRTestByDate)otherData).Inner.DatumCas);
            if (result == 0)
            {
                result = Inner.UnikatnyNahodnyKodPCRTestu.CompareTo(((PCRTestByDate)otherData).Inner.UnikatnyNahodnyKodPCRTestu);
            }
            return result;
        }

        public IData toData(string line)
        {
            return Inner.toData(line);
        }

        public override string ToString()
        {
            return Inner.ToString();
        }
    }
    /*
    public class PCRTestByNK : IData
    {
        public PCRTest Inner { get; private set; }
        public PCRTestByNK(PCRTest inner)
        {
            Inner = inner;
        }
        public int Compare(IData otherData)
        {
            return Inner.UnikatnyNahodnyKodPCRTestu.CompareTo(((PCRTestByNK)otherData).Inner.UnikatnyNahodnyKodPCRTestu);
        }
        public IData toData(string line)
        {
            return Inner.toData(line);
        }
        public override string ToString()
        {
            return Inner.ToString();
        }
    }
    */

    public class PCRTestByVT : IData
    {
        public PCRTest Inner { get; private set; }
        public PCRTestByVT(PCRTest inner)
        {
            Inner = inner;
        }
        public int Compare(IData otherData)
        {
            int result = Inner.VysledokTestu.CompareTo(((PCRTestByVT)otherData).Inner.VysledokTestu);
            if (result == 0)
            {
                result = Inner.UnikatnyNahodnyKodPCRTestu.CompareTo(((PCRTestByVT)otherData).Inner.UnikatnyNahodnyKodPCRTestu);
            }
            return result;
        }
        public IData toData(string line)
        {
            return Inner.toData(line);
        }
        public override string ToString()
        {
            return Inner.ToString();
        }
    }





}
