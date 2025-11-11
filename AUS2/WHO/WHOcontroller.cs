using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class WHOcontroller
    {
        private readonly WHO _who;
        private readonly Generator generator;

        public WHOcontroller()
        {
            _who = new WHO();
            generator = new Generator(_who);
        }

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

        private string FormatTestDetailedStr(PCRTest test)
        {
            if (test == null) return "<null test>\r\n";
            var sb = new StringBuilder();
            sb.AppendLine("=== PCR Test ===");
            sb.AppendLine($"DatumCas: {test.DatumCas:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"UnikatneCisloPacienta: {test.UnikatneCisloPacienta}");
            sb.AppendLine($"UnikatnyNahodnyKodPCRTestu: {test.UnikatnyNahodnyKodPCRTestu}");
            sb.AppendLine($"KodPracoviska: {test.KodPracoviska}");
            sb.AppendLine($"KodOkresu: {test.KodOkresu}");
            sb.AppendLine($"KodKraja: {test.KodKraja}");
            sb.AppendLine($"VysledokTestu: {test.VysledokTestu}");
            sb.AppendLine($"HodnotaTestu: {test.HodnotaTestu}");
            sb.AppendLine($"Poznamka: {test.Poznamka}");
            sb.AppendLine();
            return sb.ToString();
        }

        private string FormatPersonDetailedStr(Person person)
        {
            if (person == null) return "<null person>\r\n";
            var sb = new StringBuilder();
            sb.AppendLine("=== Patient ===");
            sb.AppendLine($"Meno: {person.Meno}");
            sb.AppendLine($"Priezvisko: {person.Priezvisko}");
            sb.AppendLine($"DatumNarodenia: {person.DatumNarodenia:yyyy-MM-dd}");
            sb.AppendLine($"UnikatneCisloPacienta: {person.UnikatneCisloPacienta}");
            sb.AppendLine();
            return sb.ToString();
        }

        private string FormatTestListStr(List<PCRTestByDate> list, string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine(title);
            sb.AppendLine(new string('=', Math.Min(80, Math.Max(10, title?.Length ?? 10))));
            sb.AppendLine();

            if (list == null || list.Count == 0)
            {
                sb.AppendLine("<no tests>");
            }
            else
            {
                foreach (var item in list)
                {
                    sb.Append(FormatTestDetailedStr(item.Inner));
                }
            }

            return sb.ToString();
        }

        // Insert PCR test and return a human-readable result string (no UI logic).
        public string InsertPcrResultFormatted(PCRTest pcrTest)
        {
            if (pcrTest == null) return "No PCR test provided.";
            try
            {
                VlozVysledokPCRTestu(pcrTest);
                return "PCR test inserted successfully.";
            }
            catch (Exception ex)
            {
                return $"Failed to insert PCR test:\r\n{ex.Message}";
            }
        }

        // Format a single test + patient details
        public string FormatTestAndPerson(PCRTest test, Person person)
        {
            var sb = new StringBuilder();
            sb.Append(FormatTestDetailedStr(test));
            sb.Append(FormatPersonDetailedStr(person));
            return sb.ToString();
        }

        // 3 -> formatted
        public string GetAllTestsForPatientFormatted(string unikatneCisloPacienta)
        {
            try
            {
                var (tests, person) = VypisVsetkychTestovPreDanehoPacienta(unikatneCisloPacienta);
                var sb = new StringBuilder();
                sb.AppendLine($"Vsetky testy pre pacienta {person.UnikatneCisloPacienta}, pocet testov: {tests.Count}");
                sb.Append(FormatPersonDetailedStr(person));
                if (tests == null || tests.Count == 0)
                {
                    sb.AppendLine("<no tests>");
                }
                else
                {
                    foreach (var wrapper in tests)
                    {
                        sb.Append(FormatTestDetailedStr(wrapper.Inner));
                    }
                }
                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving tests:\r\n{ex.Message}";
            }
        }

        // 4
        public string GetPositiveTestsInDistrictFormatted(int kodOkresu, DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisPozitivnychTestovZaObdobiePreOkres(kodOkresu, zac, kon);
                return FormatTestListStr(list, $"Pozitivne testy v okrese {kodOkresu} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving positive tests:\r\n{ex.Message}";
            }
        }

        // 5
        public string GetAllTestsInDistrictFormatted(int kodOkresu, DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisVsetkychTestovZaObdobiePreOkres(kodOkresu, zac, kon);
                return FormatTestListStr(list, $"Vsetky testy v okrese {kodOkresu} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving tests:\r\n{ex.Message}";
            }
        }

        // 6
        public string GetPositiveTestsInRegionFormatted(int kodKraja, DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisVsetkychPozitivnychTestovPreObdobieKraj(kodKraja, zac, kon);
                return FormatTestListStr(list, $"Pozitivne testy v kraji {kodKraja} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving positive tests:\r\n{ex.Message}";
            }
        }

        // 7
        public string GetAllTestsInRegionFormatted(int kodKraja, DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisVsetkychTestovPreObdobieKraj(kodKraja, zac, kon);
                return FormatTestListStr(list, $"Vsetky testy v kraji {kodKraja} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving tests:\r\n{ex.Message}";
            }
        }

        // 8
        public string GetAllPositiveTestsFormatted(DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisVsetkychPozitivnychTestovZaObdobie(zac, kon);
                return FormatTestListStr(list, $"Pozitivne testy ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (Exception ex)
            {
                return $"Error while retrieving positive tests:\r\n{ex.Message}";
            }
        }

        // 9
        public string GetAllTestsFormatted(DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisVsetkychTestovZaObdobie(zac, kon);
                return FormatTestListStr(list, $"Vsetky testy ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (Exception ex)
            {
                return $"Error while retrieving tests:\r\n{ex.Message}";
            }
        }

        // 10
        public string ListSickInDistrictFromDateRangeFormatted(int kodOkresu, DateTime datum, int X)
        {
            try
            {
                var (persons, dates) = VypisChorychVOkreseZaObodobieX(kodOkresu, datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Chori v okrese {kodOkresu} k {datum:yyyy-MM-dd} (X = {X} dni) \nPocet chorych: {persons.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (persons == null || persons.Count == 0)
                {
                    sb.AppendLine("<no persons>");
                }
                else
                {
                    for (int i = 0; i < persons.Count; i++)
                    {
                        var person = persons[i];
                        sb.Append(FormatPersonDetailedStr(person));

                        if (dates != null && i < dates.Count)
                        {
                            sb.AppendLine($"RelevantDate: {dates[i]:yyyy-MM-dd}");
                        }

                        sb.AppendLine(new string('-', 60));
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving sick persons:\r\n{ex.Message}";
            }
        }

        // 11
        public string ListSickInDistrictFromDateRangeByResultFormatted(int kodOkresu, DateTime datum, int X)
        {
            try
            {
                var persons = VypisChorychVOkresePodlaHodnotyTestuZaObdobieX(kodOkresu, datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Chori v okrese {kodOkresu} k {datum:yyyy-MM-dd} (X = {X} dni) - zoradeni podla hodnoty testu \nPocet chorych: {persons.Item1.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (persons.Item1 == null || persons.Item1.Count == 0)
                {
                    sb.AppendLine("<no persons>");
                }
                else
                {
                    for (int i = 0; i < persons.Item1.Count; i++)
                    {
                        var person = persons.Item1[i];
                        var test = persons.Item2[i];
                        sb.Append(FormatPersonDetailedStr(person));
                        sb.Append(FormatTestDetailedStr(test));
                        sb.AppendLine(new string('-', 60));
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving sick persons:\r\n{ex.Message}";
            }
        }

        // 12
        public string ListSickInRegionFromDateRangeFormatted(int kodKraja, DateTime datum, int X)
        {
            try
            {
                var persons = VypisChorychVKrajiZaObodobieX(kodKraja, datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Chori v kraji {kodKraja} k {datum:yyyy-MM-dd} (X = {X} dni) \nPocet chorych: {persons.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (persons == null || persons.Count == 0)
                {
                    sb.AppendLine("<no persons>");
                }
                else
                {
                    foreach (var person in persons)
                    {
                        sb.Append(FormatPersonDetailedStr(person));
                        sb.AppendLine(new string('-', 60));
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving sick persons:\r\n{ex.Message}";
            }
        }

        // 13
        public string ListSickFromDateRangeFormatted(DateTime datum, int X)
        {
            try
            {
                var persons = VypisChorychZaObodobieX(datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Chori k {datum:yyyy-MM-dd} (X = {X} dni) \nPocet chorych {persons.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (persons == null || persons.Count == 0)
                {
                    sb.AppendLine("<no persons>");
                }
                else
                {
                    foreach (var person in persons)
                    {
                        sb.Append(FormatPersonDetailedStr(person));
                        sb.AppendLine(new string('-', 60));
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving sick persons:\r\n{ex.Message}";
            }
        }

        // 14
        public string SingleSickWithHighestResultInDistrictFormatted(DateTime datum, int X)
        {
            try
            {
                var tests = VypisNajchorsejOsobyOkresu(datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Najhorsie (najvyssia hodnota) osoby z kazdého okresu k {datum:yyyy-MM-dd} (X = {X} dni) \nPocet hodnot: {tests.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (tests == null || tests.Count == 0)
                {
                    sb.AppendLine("<no results>");
                }
                else
                {
                    foreach (var test in tests)
                    {
                        sb.Append(FormatTestDetailedStr(test));

                        if (test.Pacient != null)
                        {
                            sb.Append(FormatPersonDetailedStr(test.Pacient));
                        }

                        sb.AppendLine(new string('-', 60));
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving results:\r\n{ex.Message}";
            }
        }

        // 15
        public string DistrictsOrderedByPositiveCountFormatted(DateTime datum, int X)
        {
            try
            {
                var (okresy, counts) = VypisPodlaPoctuChorychPreDatumX(datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Okresy usporiadane podla poctu chorych k {datum:yyyy-MM-dd} (X = {X} dni) \nPocet okresov: {okresy.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (okresy == null || okresy.Count == 0)
                {
                    sb.AppendLine("<no districts>");
                }
                else
                {
                    sb.AppendLine("Rank\tKodOkresu\tCount");
                    for (int i = 0; i < okresy.Count; i++)
                    {
                        var kod = okresy[i]?.KodOkresu ?? 0;
                        var count = (counts != null && i < counts.Count) ? counts[i] : 0;
                        sb.AppendLine($"{i + 1}\t{kod}\t{count}");
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving districts ordered by positive count:\r\n{ex.Message}";
            }
        }

        // 16
        public string RegionsOrderedByPositiveCountFormatted(DateTime datum, int X)
        {
            try
            {
                var (kraje, counts) = VypisKrajovPodlaPoctuChorychPreDatumX(datum, X);

                var sb = new StringBuilder();
                sb.AppendLine($"Kraje usporiadane podľa poctu chorych k {datum:yyyy-MM-dd} (X = {X} dni) \nPocet krajov: {kraje.Count}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();

                if (kraje == null || kraje.Count == 0)
                {
                    sb.AppendLine("<no regions>");
                }
                else
                {
                    sb.AppendLine("Rank\tKodKraja\tCount");
                    for (int i = 0; i < kraje.Count; i++)
                    {
                        var kod = kraje[i]?.KodKraja ?? 0;
                        var count = (counts != null && i < counts.Count) ? counts[i] : 0;
                        sb.AppendLine($"{i + 1}\t{kod}\t{count}");
                    }
                }

                return sb.ToString();
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving regions ordered by positive count:\r\n{ex.Message}";
            }
        }

        // 17
        public string TestsOnWorkplaceInPeriodFormatted(int kodPracoviska, DateTime zac, DateTime kon)
        {
            try
            {
                var list = VypisVsetkychTestovZaObdobiePrePracovisko(kodPracoviska, zac, kon);
                return FormatTestListStr(list, $"Testy na pracovisku {kodPracoviska} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov na pracovisku: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                return $"Not found: {knf.Message}";
            }
            catch (Exception ex)
            {
                return $"Error while retrieving tests for workplace:\r\n{ex.Message}";
            }
        }

        // 18
        public string FindPcrByCodeFormatted(int code)
        {
            try
            {
                var test = VyhladajPCRTestPodlaKodu(code);
                if (test == null) return $"No PCR test found with code {code}.";
                var sb = new StringBuilder();
                sb.Append(FormatTestDetailedStr(test));
                sb.Append(FormatPersonDetailedStr(test.Pacient));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error while searching PCR test:\r\n{ex.Message}";
            }
        }

        // 19
        public string InsertPersonFormatted(Person person)
        {
            if (person == null) return "No person provided.";
            try
            {
                VlozOsobuDoSystemu(person);
                return "Osoba bola vlozena do systemu.";
            }
            catch (Exception ex)
            {
                return $"Vkladanie osoby zlyhalo:\r\n{ex.Message}";
            }
        }

        // 20
        public string DeletePcrTestFormatted(int code)
        {
            try
            {
                VymazaniePCRTestu(code);
                return $"PCR test {code} deleted.";
            }
            catch (KeyNotFoundException knf)
            {
                return knf.Message;
            }
            catch (Exception ex)
            {
                return $"Failed to delete PCR test:\r\n{ex.Message}";
            }
        }

        // 21
        public string DeletePersonFormatted(string ucp)
        {
            try
            {
                VymazanieOsobyZoSystemu(ucp);
                return $"Person '{ucp}' deleted.";
            }
            catch (KeyNotFoundException knf)
            {
                return knf.Message;
            }
            catch (Exception ex)
            {
                return $"Failed to delete person:\r\n{ex.Message}";
            }
        }

        // import/export/generate wrappers that return status string
        public string ImportFromFileFormatted()
        {
            try
            {
                NacitajZoSuboru();
                return "Import completed.";
            }
            catch (Exception ex)
            {
                return $"Import failed:\r\n{ex.Message}";
            }
        }

        public string ExportToFileFormatted()
        {
            try
            {
                NacitajDoSuboru();
                return "Export completed.";
            }
            catch (Exception ex)
            {
                return $"Export failed:\r\n{ex.Message}";
            }
        }

        public string GenerateFormatted()
        {
            try
            {
                Generuj();
                return "Generation completed.";
            }
            catch (Exception ex)
            {
                return $"Generation failed:\r\n{ex.Message}";
            }
        }
    }
}
