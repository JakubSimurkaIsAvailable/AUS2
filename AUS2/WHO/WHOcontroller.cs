using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class WHOcontroller
    {
        private static WHO _who = new WHO();
        private Generator generator = new Generator(_who);
        // Assumption: PCRTest and Person have copy constructors:
        //   public PCRTest(PCRTest other) { ... }
        //   public Person(Person other) { ... }
        // Also PCRTestByDate/PCRTestByVT/... can be recreated from a PCRTest (as used in WHO.cs).

        private static PCRTest Clone(PCRTest src) => src is null ? null! : new PCRTest(src);
        private static Person Clone(Person src) => src is null ? null! : new Person(src);

        private static PCRTestByDate Clone(PCRTestByDate src) =>
            src is null ? null! : new PCRTestByDate(Clone(src.Inner));
        private static List<PCRTestByDate> CloneListPCRTestByDate(List<PCRTestByDate> src) =>
            src is null ? null! : src.Select(Clone).ToList();

        private static List<PCRTest> CloneListPCRTest(List<PCRTest> src) =>
            src is null ? null! : src.Select(Clone).ToList();

        private static List<Person> CloneListPerson(List<Person> src) =>
            src is null ? null! : src.Select(Clone).ToList();

        // 1
        public void VlozVysledokPCRTestu(PCRTest pcrTest) => _who.VlozVysledokPCRTestu(pcrTest);

        // 2
        public (PCRTest, Person) VyhladajVysledokTestu(int unikatnyNahodnyKodPCRTestu, string unikatneCisloPacienta)
        {
            var (t, p) = _who.VyhladajVysledokTestu(unikatnyNahodnyKodPCRTestu, unikatneCisloPacienta);
            return (Clone(t), Clone(p));
        }

        // 3
        public (List<PCRTestByDate>, Person) VypisVsetkychTestovPreDanehoPacienta(string unikatneCisloPacienta)
        {
            var (list, person) = _who.VypisVsetkychTestovPreDanehoPacienta(unikatneCisloPacienta);
            return (CloneListPCRTestByDate(list), Clone(person));
        }

        // 4
        public List<PCRTestByDate> VypisPozitivnychTestovZaObdobiePreOkres(int kodOkresu, DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisPozitivnychTestovZaObdobiePreOkres(kodOkresu, zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 5
        public List<PCRTestByDate> VypisVsetkychTestovZaObdobiePreOkres(int kodOkresu, DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisVsetkychTestovZaObdobiePreOkres(kodOkresu, zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 6
        public List<PCRTestByDate> VypisVsetkychPozitivnychTestovPreObdobieKraj(int kodKraja, DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisVsetkychPozitivnychTestovPreObdobieKraj(kodKraja, zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 7
        public List<PCRTestByDate> VypisVsetkychTestovPreObdobieKraj(int kodKraja, DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisVsetkychTestovPreObdobieKraj(kodKraja, zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 8
        public List<PCRTestByDate> VypisVsetkychPozitivnychTestovZaObdobie(DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisVsetkychPozitivnychTestovZaObdobie(zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 9
        public List<PCRTestByDate> VypisVsetkychTestovZaObdobie(DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisVsetkychTestovZaObdobie(zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 10
        public (List<Person>, List<DateTime>) VypisChorychVOkreseZaObodobieX(int kodOkresu, DateTime datum, int X)
        {
            var (people, dates) = _who.VypisChorychVOkreseZaObodobieX(kodOkresu, datum, X);
            return (CloneListPerson(people), dates is null ? null! : new List<DateTime>(dates));
        }

        // 11
        public (List<Person>, List<PCRTest>) VypisChorychVOkresePodlaHodnotyTestuZaObdobieX(int kodOkresu, DateTime datum, int X)
        {
            var res = _who.VypisChorychVOkresePodlaHodnotyTestuZaObdobieX(kodOkresu, datum, X);
            var clonedPersons = CloneListPerson(res.Item1);
            var clonedTests = CloneListPCRTest(res.Item2);
            return (clonedPersons, clonedTests);
        }

        // 12
        public List<Person> VypisChorychVKrajiZaObodobieX(int kodKraja, DateTime datum, int X)
        {
            var res = _who.VypisChorychVKrajiZaObodobieX(kodKraja, datum, X);
            return CloneListPerson(res);
        }

        // 13
        public List<Person> VypisChorychZaObodobieX(DateTime datum, int X)
        {
            var res = _who.VypisChorychZaObodobieX(datum, X);
            return CloneListPerson(res);
        }

        // 14
        public List<PCRTest> VypisNajchorsejOsobyOkresu(DateTime datum, int x)
        {
            var res = _who.VypisNajchorsejOsobyOkresu(datum, x);
            return CloneListPCRTest(res);
        }

        // 15
        public (List<Okres>, List<int>) VypisPodlaPoctuChorychPreDatumX(DateTime datum, int X)
        {
            var (okresy, counts) = _who.VypisPodlaPoctuChorychPreDatumX(datum, X);
            // shallow-copy lists of Okres/Kraj (we only have PCRTest/Person copy ctors guaranteed)
            return (okresy is null ? null! : new List<Okres>(okresy), counts is null ? null! : new List<int>(counts));
        }

        // 16
        public (List<Kraj>, List<int>) VypisKrajovPodlaPoctuChorychPreDatumX(DateTime datum, int X)
        {
            var (kraje, counts) = _who.VypisKrajovPodlaPoctuChorychPreDatumX(datum, X);
            return (kraje is null ? null! : new List<Kraj>(kraje), counts is null ? null! : new List<int>(counts));
        }

        // 17
        public List<PCRTestByDate> VypisVsetkychTestovZaObdobiePrePracovisko(int kodPracoviska, DateTime zaciatok, DateTime koniec)
        {
            var res = _who.VypisVsetkychTestovZaObdobiePrePracovisko(kodPracoviska, zaciatok, koniec);
            return CloneListPCRTestByDate(res);
        }

        // 18
        public PCRTest VyhladajPCRTestPodlaKodu(int unikatnyNahodnyKodPCRTestu)
        {
            var res = _who.VyhladajPCRTestPodlaKodu(unikatnyNahodnyKodPCRTestu);
            return Clone(res);
        }

        // 19
        public void VlozOsobuDoSystemu(Person person) => _who.VlozOsobuDoSystemu(person);

        // 20
        public void VymazaniePCRTestu(int unikatnyNahodnyKodPCRTestu) => _who.VymazaniePCRTestu(unikatnyNahodnyKodPCRTestu);

        // 21
        public void VymazanieOsobyZoSystemu(string unikatneCisloPacienta) => _who.VymazanieOsobyZoSystemu(unikatneCisloPacienta);

        public void NacitajDoSuboru() => _who.NacitajDoSuboru();
        public void NacitajZoSuboru() => _who.NacitajZoSuboru();

        public void Generuj() => generator.Generuj(10, 10, 100, 5000, 10);
    }
}
