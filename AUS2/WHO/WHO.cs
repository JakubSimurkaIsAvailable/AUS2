using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUS2.WHO
{
    public class WHO
    {
        //private Krajina Krajina { get; set; } = new Krajina();
        private AVL<Person> Ludia { get; set; } = new AVL<Person>();
        private AVL<PCRTest> VsetkyTesty { get; set; } = new AVL<PCRTest>();
        private AVL<PCRTestByDate> VsetkyPozitivneTestyByDate { get; set; } = new AVL<PCRTestByDate>();
        private AVL<PCRTestByDate> VsetkyNegativneTestyByDate { get; set; } = new AVL<PCRTestByDate>();

        private AVL<Kraj> VsetkyKraje { get; set; } = new AVL<Kraj>();
        private AVL<Okres> VsetkyOkresy { get; set; } = new AVL<Okres>();
        private AVL<Pracovisko> VsetkyPracoviska { get; set; } = new AVL<Pracovisko>();

        private int pocetLudi = 0;

        public WHO()
        {

        }
        public void VlozKraj(int kodKraju)
        {
            var kraj = new Kraj(kodKraju);
            //Krajina.Kraje.Insert(kraj);
            VsetkyKraje.Insert(kraj);
        }
        public void VlozOkres(int kodOkresu)
        {
            //var kraj = Krajina.Kraje.Find(new Kraj(kodKraju));
            var okres = new Okres(kodOkresu);
            //kraj.Okresy.Insert(okres);
            VsetkyOkresy.Insert(okres);
        }
        public void VlozPracovisko(int kodPracoviska)
        {
            //var kraj = Krajina.Kraje.Find(new Kraj(kodKraju));
            //var okres = kraj.Okresy.Find(new Okres(kodOkresu));
            var pracovisko = new Pracovisko(kodPracoviska);
            //okres.Pracoviska.Insert(pracovisko);
            VsetkyPracoviska.Insert(pracovisko);
        }
        //1 vloz vysledok PCR testu do systemu
        public void VlozVysledokPCRTestu(PCRTest pcrTest)
        {
            Kraj kraj;
            Okres okres;
            Pracovisko pracovisko;
            Person clovek;
            pcrTest.SetUnikatnyNahodnyKodPCRTestu(VsetkyTesty.FindMax(null).UnikatnyNahodnyKodPCRTestu + 1);
            try
            {
                clovek = Ludia.Find(new Person(pcrTest.UnikatneCisloPacienta));
            } catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Pacient nebol najdeny v systeme. Najprv je potrebne pacienta vlozit do systemu.");
            }

            try
            {
                kraj = VsetkyKraje.Find(new Kraj(pcrTest.KodKraja));
            }
            catch (InvalidOperationException)
            {
                VsetkyKraje.Insert(new Kraj(pcrTest.KodKraja));
                kraj = VsetkyKraje.Find(new Kraj(pcrTest.KodKraja));
            }

            try
            {
                okres = VsetkyOkresy.Find(new Okres(pcrTest.KodOkresu));
            }
            catch (InvalidOperationException)
            {
                VsetkyOkresy.Insert(new Okres(pcrTest.KodOkresu));
                okres = VsetkyOkresy.Find(new Okres(pcrTest.KodOkresu));
            }

            try
            {
                pracovisko = VsetkyPracoviska.Find(new Pracovisko(pcrTest.KodPracoviska));
            }
            catch (InvalidOperationException)
            {
                VsetkyPracoviska.Insert(new Pracovisko(pcrTest.KodPracoviska));
                pracovisko = VsetkyPracoviska.Find(new Pracovisko(pcrTest.KodPracoviska));
            }
            if (pcrTest.VysledokTestu)
            {
                pracovisko.TrueTestyDatum.Insert(new PCRTestByDate(pcrTest));
                okres.TrueTestyDatum.Insert(new PCRTestByDate(pcrTest));
                kraj.TrueTestyDatum.Insert(new PCRTestByDate(pcrTest));
                VsetkyPozitivneTestyByDate.Insert(new PCRTestByDate(pcrTest));
            }
            else
            {
                pracovisko.FalseTestyDatum.Insert(new PCRTestByDate(pcrTest));
                okres.FalseTestyDatum.Insert(new PCRTestByDate(pcrTest));
                kraj.FalseTestyDatum.Insert(new PCRTestByDate(pcrTest));
                VsetkyNegativneTestyByDate.Insert(new PCRTestByDate(pcrTest));
            }
            
            VsetkyTesty.Insert(pcrTest);

            //VsetkyTesty.Insert(pcrTest);
            clovek.PCRTests.Insert(pcrTest);
            pcrTest.Pacient = clovek;
            clovek.PCRTestsByDate.Insert(new PCRTestByDate(pcrTest));
        }

        //2 Vyhladanie vysledku testu (definovany kodom PCR testu) pre pacienta (definovany unikatnym cislom pacienta) a zobrazenie vsetkych udajov
        public (PCRTest, Person) VyhladajVysledokTestu(int unikatnyNahodnyKodPCRTestu, string unikatneCisloPacienta)
        {
            Person clovek;
            PCRTest hladanyTest;
            try
            {
                clovek = Ludia.Find(new Person(unikatneCisloPacienta));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Pacient nebol najdeny.");
            }
            try
            {
                hladanyTest = clovek.PCRTests.Find(new PCRTest(unikatnyNahodnyKodPCRTestu));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Test nebol najdeny pre daneho pacienta.");
            }
                return (hladanyTest, clovek);
               
            
        }

        //3 Vypis všetkych uskutočnených testov pre daného pacienta (definovaný unikátnym číslom pacienta) usporiadaných podla dátumu a času ich vykonania.
        public (List<PCRTestByDate>, Person) VypisVsetkychTestovPreDanehoPacienta(string unikatneCisloPacienta)
        {
            try
            {
                Person clovek = Ludia.Find(new Person(unikatneCisloPacienta));
                List<PCRTestByDate> result = clovek.PCRTestsByDate.FindInterval(clovek.PCRTestsByDate.FindMin(null), clovek.PCRTestsByDate.FindMax(null));
                return (result, clovek);
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Pacient nebol najdeny.");
            }
        }

        //4 Vypis vsetkych pozitivnych testov vykonanych za dane casove obdobie pre zadany okres (definovany kodom okresu)
        public List<PCRTestByDate> VypisPozitivnychTestovZaObdobiePreOkres(int kodOkresu, DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            Okres okres;
            try
            {
                okres = VsetkyOkresy.Find(new Okres(kodOkresu));
            } catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Okres nebol najdeny.");
            }
            
            result = okres.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            return result;
        }

        //5. Vypis vsetkych testov uskutocnenych za zadane casove obdobie pre zadany okres (definovany kodom okresu)
        public List<PCRTestByDate> VypisVsetkychTestovZaObdobiePreOkres(int kodOkresu, DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            Okres okres;
            try
            {
                okres = VsetkyOkresy.Find(new Okres(kodOkresu));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Okres nebol najdeny.");
            }
            List<PCRTestByDate> trueTestyVObdobi = okres.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            List<PCRTestByDate> falseTestyVObdobi = okres.FalseTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            result.AddRange(trueTestyVObdobi);
            result.AddRange(falseTestyVObdobi);
            return result;
        }

        //6. Vypis vsetkych pozitivnych testov uskutocnenych za zadane casove obdobie pre zadany kraj (definovany kodom kraja)
        public List<PCRTestByDate> VypisVsetkychPozitivnychTestovPreObdobieKraj(int kodKraja, DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            Kraj kraj;
            try
            {
                kraj = VsetkyKraje.Find(new Kraj(kodKraja));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Kraj nebol najdeny.");
            }
            result = kraj.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            return result;
        }

        //7. Vypis vsetkych testov uskutocnenych za zadane casove obdobie pre zadany kraj (definovany kodom kraja)
        public List<PCRTestByDate> VypisVsetkychTestovPreObdobieKraj(int kodKraja, DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            Kraj kraj;
            try
            {
                kraj = VsetkyKraje.Find(new Kraj(kodKraja));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Kraj nebol najdeny.");
            }
            List<PCRTestByDate> trueTestyVObdobi = kraj.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            return result;
        }

        //8. Vypis vsetkych pozitivnych testov uskutocnenych za zadane casove obdobie
        public List<PCRTestByDate> VypisVsetkychPozitivnychTestovZaObdobie(DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            result = VsetkyPozitivneTestyByDate.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            return result;
        }

        //9. Vypis vsetkych testov uskutocnenych za zadane casove obdobie
        public List<PCRTestByDate> VypisVsetkychTestovZaObdobie(DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            result.AddRange(VsetkyPozitivneTestyByDate.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue))));
            result.AddRange(VsetkyNegativneTestyByDate.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue))));
            return result;
        }

        //10. Vypis chorych osob v okrese (definovanym kodom okresu k zadanemu datumu), pricom osobu povazujeme za choru X dni od pozitivneho testu
        public (List<Person>, List<DateTime>) VypisChorychVOkreseZaObodobieX(int kodOkresu, DateTime datum, int X)
        {
            List<Person> result = [];
            List<DateTime> dates = [];
            Okres okres;
            DateTime zaciatok = datum.AddDays(-X);
            try
            {
                okres = VsetkyOkresy.Find(new Okres(kodOkresu));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Okres nebol najdeny.");
            }
            List<PCRTestByDate> pozitivneTesty = okres.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue)));
            foreach (PCRTestByDate test in pozitivneTesty)
            {
                Person clovek = test.Inner.Pacient;
                if (!result.Contains(clovek))
                {
                    result.Add(clovek);
                    dates.Add(test.Inner.DatumCas);
                }
            }
            return (result, dates);
        }

        //11. Vypis chorych osob v okrese (definovany kodom okresu) k zadanemu datumu, pricom osoby su usporiadane podla hodnoty testu
        public (List<Person>, List<PCRTest>) VypisChorychVOkresePodlaHodnotyTestuZaObdobieX(int kodOkresu, DateTime datum, int X)
        {
            List<Person> result = [];
            List<PCRTest> testy = [];
            List<PCRTest> testyNaVratenie = [];
            Okres okres;
            DateTime zaciatok = datum.AddDays(-X);
            try
            {
                okres = VsetkyOkresy.Find(new Okres(kodOkresu));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Okres nebol najdeny.");
            }
            testy = okres.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue))).Select(t => t.Inner).ToList();
            testy = testy.OrderByDescending(t => t.HodnotaTestu).ToList();
            foreach (var test in testy)
            {

                Person clovek = test.Pacient;
                if (!result.Contains(clovek))
                {
                    result.Add(clovek);
                    testyNaVratenie.Add(test);
                }

            }
            return (result, testyNaVratenie);
        }

        //12. Vypis chorych osob v kraji (definovanym kodom kraja) k zadanemu datumu, pricom osobu povazujeme za choru X dni od pozitivneho testu
        public List<Person> VypisChorychVKrajiZaObodobieX(int kodKraja, DateTime datum, int X)
        {
            List<Person> result = [];
            Kraj kraj;
            try
            {
                kraj = VsetkyKraje.Find(new Kraj(kodKraja));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Kraj nebol najdeny.");
            }
            result = kraj.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(datum.AddDays(-X), int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue)))
                .Select(t => t.Inner.Pacient)
                .ToList();
            foreach (var test in result) {
                if (!result.Contains(test))
                {
                    result.Add(test);
                }
            }
            return result;
        }

        //13. Vypis chorych osob k zadanemu datumu, pricom osobu povazujeme za choru X dni od pozitivneho testu
        public List<Person> VypisChorychZaObodobieX(DateTime datum, int X)
        {
            List<Person> result = [];
            DateTime zaciatok = datum.AddDays(-X);
            result = VsetkyPozitivneTestyByDate.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue)))
                .Select(t => t.Inner.Pacient)
                .ToList();
            foreach (var test in result)
            {
                if (!result.Contains(test))
                {
                    result.Add(test);
                }
            }
                return result;
        }

        //14. Vypis jednej chorej osoby k zadanemu datumu, pricom osobu povazujeme za choru X dni od pozitivneho testu (X zada uzivatel) z kazdeho okresu, ktora ma najvyssiu hodnotu testu
        public List<PCRTest> VypisNajchorsejOsobyOkresu(DateTime datum, int x)
        {
            var result = new List<PCRTest>();
            var okresy = VsetkyOkresy.FindInterval(VsetkyOkresy.FindMin(null), VsetkyOkresy.FindMax(null));
            foreach (var okres in okresy)
            {
                List<PCRTestByDate> testyPodlaDatumu = [];
                try
                {
                    testyPodlaDatumu = okres.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(datum.AddDays(-x), int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue)));
                    result.Add(testyPodlaDatumu.OrderByDescending(t => t.Inner.HodnotaTestu).First().Inner);
                } catch (InvalidOperationException)
                {
                    continue;
                }
                
            }
            return result;
        }

        //15. Vypis okresov usporiadanych podla poctu chorych osob k zadanemu datumu, pricom osobu povazujeme za choru X dni od pozitivneho testu
        public (List<Okres>, List<int>) VypisPodlaPoctuChorychPreDatumX(DateTime datum, int X)
        {
            var result = new List<Okres>();
            var pocetChorychList = new List<int>();
            var okreschory = new List<(Okres okres, int pocet)>();
            var okresy = VsetkyOkresy.FindInterval(VsetkyOkresy.FindMin(null), VsetkyOkresy.FindMax(null));
            foreach (var okres in okresy)
            {
                var pozitivneTesty = okres.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(datum.AddDays(-X), int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue)));
                List<Person> unikatniChori = new();
                foreach (var test in pozitivneTesty)
                {
                    if (!unikatniChori.Any(p => string.Equals(p.UnikatneCisloPacienta, test.Inner.UnikatneCisloPacienta)))
                    {
                        unikatniChori.Add(test.Inner.Pacient);
                    }
                }
                okreschory.Add((okres, unikatniChori.Count));
                
            }
            var zoradeneOkresy = okreschory.OrderByDescending(k => k.pocet).ToList();
            result = zoradeneOkresy.Select(k => k.okres).ToList();
            pocetChorychList = zoradeneOkresy.Select(k => k.pocet).ToList();
            return (result, pocetChorychList);
        }

        //16. Vypis krajov usporiadanych podla poctu chorych osob k zadanemu datumu, pricom osobu povazujeme za choru X dni od pozitivneho testu
        public (List<Kraj>, List<int>) VypisKrajovPodlaPoctuChorychPreDatumX(DateTime datum, int X)
        {
            var result = new List<Kraj>();
            var pocetChorychList = new List<int>();
            var krajchory = new List<(Kraj kraj, int pocet)>();
            var kraje = VsetkyKraje.FindInterval(VsetkyKraje.FindMin(null), VsetkyKraje.FindMax(null));
            foreach (var kraj in kraje)
            {
                var pozitivneTesty = kraj.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(datum.AddDays(-X), int.MinValue)), new PCRTestByDate(new PCRTest(datum, int.MaxValue)));
                List<Person> unikatniChori = new();
                foreach (var test in pozitivneTesty)
                {
                    if (!unikatniChori.Any(p => string.Equals(p.UnikatneCisloPacienta, test.Inner.UnikatneCisloPacienta)))
                    {
                        unikatniChori.Add(test.Inner.Pacient);
                    }
                }
                krajchory.Add((kraj, unikatniChori.Count));
                
            }
            var zoradeneKraje = krajchory.OrderByDescending(k => k.pocet).ToList();
            result = zoradeneKraje.Select(k => k.kraj).ToList();
            pocetChorychList = zoradeneKraje.Select(k => k.pocet).ToList();
            return (result, pocetChorychList);
        }

        //17 Vypis vsetkych testov uskutocnenych za zadane casove obdobie na danom pracovisku (definovany kodom pracoviska)
        public List<PCRTestByDate> VypisVsetkychTestovZaObdobiePrePracovisko(int kodPracoviska, DateTime zaciatok, DateTime koniec)
        {
            List<PCRTestByDate> result = [];
            Pracovisko pracovisko;
            try
            {
                pracovisko = VsetkyPracoviska.Find(new Pracovisko(kodPracoviska));
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException("Pracovisko nebolo najdene.");
            }
            List<PCRTestByDate> trueTestyVObdobi = pracovisko.TrueTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            List<PCRTestByDate> falseTestyVObdobi = pracovisko.FalseTestyDatum.FindInterval(new PCRTestByDate(new PCRTest(zaciatok, int.MinValue)), new PCRTestByDate(new PCRTest(koniec, int.MaxValue)));
            result.AddRange(trueTestyVObdobi);
            result.AddRange(falseTestyVObdobi);
            return result;
        }

        //18 Vyhladanie PCR testu podla jeho kodu
        public PCRTest VyhladajPCRTestPodlaKodu(int unikatnyNahodnyKodPCRTestu)
        {
            PCRTest hladanyTest = VsetkyTesty.Find(new PCRTest(unikatnyNahodnyKodPCRTestu));
            return hladanyTest;
        }

        //19 Vlozenie osoby do systemu
        public void VlozOsobuDoSystemu(Person person)
        {
            Ludia.Insert(person);
            pocetLudi++;
            person.SetUnikatneCisloPacienta(person.DatumNarodenia.ToString("yyMMdd") + "/" + pocetLudi.ToString("D4"));
        }

        //20. Trvale a nenavratne vymazanie vysledku PCR testu (napr pri chybnom vlozeni), test je definovany svojim kodom
        public void VymazaniePCRTestu(int unikatnyNahodnyKodPCRTestu)
        {
            PCRTest testNaVymazanie = VsetkyTesty.Find(new PCRTest(unikatnyNahodnyKodPCRTestu));
            VsetkyTesty.Delete(testNaVymazanie);
            Person clovek = Ludia.Find(new Person(testNaVymazanie.UnikatneCisloPacienta));
            clovek.PCRTests.Delete(testNaVymazanie);
            clovek.PCRTestsByDate.Delete(new PCRTestByDate(testNaVymazanie));
            Kraj kraj = VsetkyKraje.Find(new Kraj(testNaVymazanie.KodKraja));
            Okres okres = VsetkyOkresy.Find(new Okres(testNaVymazanie.KodOkresu));
            Pracovisko pracovisko = VsetkyPracoviska.Find(new Pracovisko(testNaVymazanie.KodPracoviska));
            if (testNaVymazanie.VysledokTestu)
            {
                pracovisko.TrueTestyDatum.Delete(new PCRTestByDate(testNaVymazanie));
                okres.TrueTestyDatum.Delete(new PCRTestByDate(testNaVymazanie));
                kraj.TrueTestyDatum.Delete(new PCRTestByDate(testNaVymazanie));
                VsetkyPozitivneTestyByDate.Delete(new PCRTestByDate(testNaVymazanie));
            }
            else
            {
                pracovisko.FalseTestyDatum.Delete(new PCRTestByDate(testNaVymazanie));
                okres.FalseTestyDatum.Delete(new PCRTestByDate(testNaVymazanie));
                kraj.FalseTestyDatum.Delete(new PCRTestByDate(testNaVymazanie));
                VsetkyNegativneTestyByDate.Delete(new PCRTestByDate(testNaVymazanie));
            }
        }

        //21 Vymazanie osoby zo systemu (definovana unikatnym cislom pacienta) aj s jej vysledkami PCR testov
        public void VymazanieOsobyZoSystemu(string unikatneCisloPacienta)
        {
            Person clovek = Ludia.Find(new Person(unikatneCisloPacienta));
            List<PCRTest> testyNaVymazanie = clovek.PCRTests.FindInterval(clovek.PCRTests.FindMin(null), clovek.PCRTests.FindMax(null));
            foreach (PCRTest test in testyNaVymazanie)
            {
                VymazaniePCRTestu(test.UnikatnyNahodnyKodPCRTestu);
            }
            Ludia.Delete(clovek);
        }

        public void NacitajDoSuboru()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Vyberte priečinok, kam sa má uložiť nový priečinok s CSV súbormi";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    string outputFolder = Path.Combine(selectedPath, dateFolder);
                    

                    Directory.CreateDirectory(outputFolder);

                    string ludiaPath = Path.Combine(outputFolder, "ludia.csv");
                    string testyPath = Path.Combine(outputFolder, "testy.csv");
                    string krajePath = Path.Combine(outputFolder, "kraje.csv");
                    string okresyPath = Path.Combine(outputFolder, "okresy.csv");
                    string pracoviskaPath = Path.Combine(outputFolder, "pracoviska.csv");

                    Ludia.WriteIntoCSV(ludiaPath);
                    VsetkyKraje.WriteIntoCSV(krajePath);
                    VsetkyOkresy.WriteIntoCSV(okresyPath);
                    VsetkyPracoviska.WriteIntoCSV(pracoviskaPath);
                    VsetkyTesty.WriteIntoCSV(testyPath);
                    MessageBox.Show($"Súbory boli uložené do: {outputFolder}", "Úspech",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void NacitajZoSuboru()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Vyberte priečinok, kde sa nachádzajú súbory ludia.csv a testy.csv";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;

                    string ludiaPath = Path.Combine(selectedPath, "ludia.csv");
                    string testyPath = Path.Combine(selectedPath, "testy.csv");
                    string krajePath = Path.Combine(selectedPath, "kraje.csv");
                    string okresyPath = Path.Combine(selectedPath, "okresy.csv");
                    string pracoviskaPath = Path.Combine(selectedPath, "pracoviska.csv");

                    if (File.Exists(ludiaPath) && File.Exists(testyPath))
                    {
                        Ludia.LoadFromCSV(ludiaPath, new Person());
                        VsetkyKraje.LoadFromCSV(krajePath, new Kraj());
                        VsetkyOkresy.LoadFromCSV(okresyPath, new Okres());
                        VsetkyPracoviska.LoadFromCSV(pracoviskaPath, new Pracovisko());
                        VsetkyTesty.LoadFromCSV(testyPath, new PCRTest());
                        BuildHierarchyFromTests();
                    }
                    else
                    {
                        MessageBox.Show("Súbory ludia.csv alebo testy.csv neboli nájdené v zvolenom priečinku.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BuildHierarchyFromTests()
        {
            List<PCRTest> vsetkyTesty = VsetkyTesty.FindInterval(VsetkyTesty.FindMin(null), VsetkyTesty.FindMax(null));
            foreach (PCRTest test in vsetkyTesty)
            {
                VlozVysledokPCRTestu(test);
            }
        }
    }
}
