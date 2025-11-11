using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2.WHO
{
    public class Generator
    {
        private WHO WHO { get; set; }
        public Generator(WHO who) 
        { 
            WHO = who;
        }

        public void Generuj(int pocetKrajov, int okresovNaKraj, int pracoviskNaOkres, int pocetLudi, int testovNaOsobu)
        {
            var rnd = new Random(1);
            var mena = new[] { "Ján", "Peter", "Marek", "Tomáš", "Lukáš", "Martin", "Filip", "Michal", "Andrej", "Roman", "Mária", "Anna", "Eva", "Petra", "Lucia" };
            var priezviska = new[] { "Novák", "Kováč", "Horváth", "Varga", "Tóth", "Baláž", "Kučera", "Molnár", "Bartoš", "Švec", "Svoboda", "Mička", "Urban" };

            var kraje = Enumerable.Range(1, pocetKrajov).ToList();
            var okresToKraj = new Dictionary<int, int>();
            var pracToOkres = new Dictionary<int, int>();
            var pracoviskaList = new List<int>();

            foreach (var kodKraja in kraje)
            {
                WHO.VlozKraj(kodKraja);
                for(int i = 1; i <= okresovNaKraj;  i++)
                {
                    int kodOkresu = kodKraja * 1000 + i;
                    okresToKraj[kodOkresu] = kodKraja;
                    WHO.VlozOkres(kodOkresu);
                    for(int j = 1; j <= pracoviskNaOkres; j++)
                    {
                        int kodPracoviska = kodOkresu * 1000 + j;
                        pracToOkres[kodPracoviska] = kodOkresu;
                        pracoviskaList.Add(kodPracoviska);
                        WHO.VlozPracovisko(kodPracoviska);
                    }
                }
            }
            int idtestu = 0;
            for (int i = 1; i <= pocetLudi; i++)
            {
                var meno = mena[rnd.Next(mena.Length)];
                var priezvisko = priezviska[rnd.Next(priezviska.Length)];
                var rok= rnd.Next(1940, 2006);
                var mesiac = rnd.Next(1, 13);
                var den = rnd.Next(1, DateTime.DaysInMonth(rok, mesiac) + 1);
                var datumNarodenia = new DateTime(rok, mesiac, den);
                var person = new Person(meno, priezvisko, datumNarodenia);
                WHO.VlozOsobuDoSystemu(person);
                string ucp = person.UnikatneCisloPacienta;
                for (int j = 0; j < testovNaOsobu; j++)
                {
                    int kodPracoviska = pracoviskaList[rnd.Next(pracoviskaList.Count)];
                    int kodOkresu = pracToOkres[kodPracoviska];
                    int kodKraja = okresToKraj[kodOkresu];

                    var dniDozadu = rnd.Next(0, 365 * 2);
                    var minutyDozazu = rnd.Next(0, 24 * 60);
                    var datumCas = DateTime.Now.AddDays(-dniDozadu).AddMinutes(-minutyDozazu);

                    bool vysledok = rnd.NextDouble() < 0.15;
                    double hodnota = vysledok ? Math.Round(rnd.NextDouble() * 40.0 + 1.0, 3) : Math.Round(rnd.NextDouble() * 0.99 + 0.01, 3);
                    string poznamka = string.Empty;
                    idtestu++;
                    var test = new PCRTest(datumCas, ucp, idtestu, kodPracoviska, kodOkresu, kodKraja, vysledok, hodnota, poznamka);
                    WHO.VlozVysledokPCRTestu(test);
                }
                Console.WriteLine(person.Meno + " " + person.Priezvisko);
            }
        }
    }
}
