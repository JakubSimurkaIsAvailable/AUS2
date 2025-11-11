using AUS2.WHO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AUS2GUI
{
    public partial class Form1 : Form
    {
        public WHOcontroller who = new WHOcontroller();
        public Form1()
        {
            InitializeComponent();
            string[] labelNames =
                new string[]
                {
                    "1 Vloz vysledok PCR testu",
                    "2 Vyhladaj test pre pacienta",
                    "3 Vypis vsetky testy pre pacienta",
                    "4 Pozitivne testy v okrese",
                    "5 Vsetky testy v okrese",
                    "6 Pozitivne testy v kraji",
                    "7 Vsetky testy v kraji",
                    "8 Vsetky pozitivne testy",
                    "9 Vsetky testy",
                    "10 Vypis chorych v okrese od datumu a doby X",
                    "11 Vypis chorych v okrese od datumu a doby X podla vysledku",
                    "12 Vypis chorych v kraju od datumu a doby X",
                    "13 Vypis chorych od datumu a doby X",
                    "14 Vypis jednej chorej osoby okresu s najvyssim vysledkom",
                    "15 Vypis okresov usporiadanych podla poctu pozitivnych testov",
                    "16 Vypis krajov usporiadanych podla poctu pozitivnych testov",
                    "17 Vypis testov uskutocnenych za casove obdobie na pracovisku",
                    "18 Vyhladanie PCR testu podla kodu",
                    "19 Vlozenie osoby do systemu",
                    "20 Vymazanie PCR testy",
                    "21 Vymazanie osoby"
                };


            Action[] handlers = new Action[]
            {
                InsertPcrResult,                    // 1
                FindTestForPatient,                 // 2
                ListAllTestsForPatient,             // 3
                PositiveTestsInDistrict,            // 4
                AllTestsInDistrict,                 // 5
                PositiveTestsInRegion,              // 6
                AllTestsInRegion,                   // 7
                AllPositiveTests,                   // 8
                AllTests,                           // 9
                ListSickInDistrictFromDateRange,    // 10
                ListSickInDistrictFromDateRangeByResult, // 11
                ListSickInRegionFromDateRange,      // 12
                ListSickFromDateRange,              // 13
                SingleSickWithHighestResultInDistrict, // 14
                DistrictsOrderedByPositiveCount,    // 15
                RegionsOrderedByPositiveCount,      // 16
                TestsOnWorkplaceInPeriod,           // 17
                FindPcrByCode,                      // 18
                InsertPerson,                       // 19
                DeletePcrTest,                      // 20
                DeletePerson                        // 21
            };


            int yOffset = 10;
            int sideMargin = 10;
            int buttonHeight = 32;

            panelContainer.Resize += (s, e) => ArrangeControls();

            void ArrangeControls()
            {
                panelContainer.SuspendLayout();
                panelContainer.Controls.Clear();
                yOffset = 10;

                int availableWidth = panelContainer.ClientSize.Width - (sideMargin * 2);
                int buttonWidth = (int)(availableWidth * 0.8);

                for (int i = 0; i < labelNames.Length; i++)
                {
                    int idx = i;

                    // Label
                    Label lbl = new Label
                    {
                        Text = labelNames[i],
                        Left = sideMargin,
                        Top = yOffset,
                        Width = availableWidth,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        AutoSize = false,
                        MaximumSize = new Size(availableWidth, 0)
                    };

                    Size textSize = TextRenderer.MeasureText(lbl.Text, lbl.Font, lbl.MaximumSize, TextFormatFlags.WordBreak);
                    lbl.Height = textSize.Height;

                    Button btn = new Button
                    {
                        Text = $"Run {i + 1}",
                        Width = buttonWidth,
                        Height = buttonHeight,
                        Left = sideMargin + (availableWidth - buttonWidth) / 2,
                        Top = lbl.Top + lbl.Height + 5,
                        Name = $"btn{i + 1}"
                    };

                    btn.Click += (sender, e) =>
                    {
                        if (idx >= 0 && idx < handlers.Length && handlers[idx] != null)
                            handlers[idx].Invoke();
                        else
                            MessageBox.Show($"No handler assigned for {btn.Text} (\"{lbl.Text}\")", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };

                    panelContainer.Controls.Add(lbl);
                    panelContainer.Controls.Add(btn);

                    yOffset = btn.Top + buttonHeight + 15;
                }

                panelContainer.ResumeLayout();
            }
            ArrangeControls();
        }

        private void PanelButtons_Paint(object sender, PaintEventArgs e)
        {
        }

        // ----- Action handlers -----

        //1 Vloz vysledok PCR testu
        private void InsertPcrResult()
        {
            using var f = new Form2();
            var dr = f.ShowDialog(this);
            if (dr == DialogResult.OK && f.CreatedTest != null)
            {
                try
                {
                    who.VlozVysledokPCRTestu(f.CreatedTest);
                    MessageBox.Show("PCR test inserted successfully.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to insert PCR test:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //2 Vyhladaj test pre pacienta
        private void FindTestForPatient()
        {
            using var f = new Form3(who);
            var dr = f.ShowDialog(this);
            if (dr == DialogResult.OK && f.FoundTest != null && f.FoundPerson != null)
            {
                ShowTestAndPersonDetails(f.FoundTest, f.FoundPerson);
            }
        }

        private void ShowTestAndPersonDetails(PCRTest test, Person person)
        {
            PanelItems.Controls.Clear();

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.White
            };

            var sb = new StringBuilder();
            sb.Append(FormatTestDetailed(test));
            sb.Append(FormatPersonDetailed(person));

            rtb.Text = sb.ToString();
            PanelItems.Controls.Add(rtb);
        }

        //3 Vypis vsetky testy pre pacienta
        private void ListAllTestsForPatient()
        {
            using var f = new Form4();
            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            var ucp = f.UnikatneCisloPacienta;
            try
            {
                var (tests, person) = who.VypisVsetkychTestovPreDanehoPacienta(ucp);

                ShowTestsForPatient(tests, person);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTestsForPatient(System.Collections.Generic.List<PCRTestByDate> tests, Person person)
        {
            PanelItems.Controls.Clear();

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.White
            };

            var sb = new StringBuilder();
            sb.AppendLine($"Vsetky testy pre pacienta {person.UnikatneCisloPacienta}, pocet testov: {tests.Count}");
            sb.Append(FormatPersonDetailed(person));

            if (tests == null || tests.Count == 0)
            {
                sb.AppendLine("<no tests>");
            }
            else
            {
                foreach (var wrapper in tests)
                {
                    sb.Append(FormatTestDetailed(wrapper.Inner));
                }
            }

            rtb.Text = sb.ToString();
            PanelItems.Controls.Add(rtb);
        }

        //4 Pozitivne testy v okrese
        private void PositiveTestsInDistrict()
        {
            using var f = new Form5();

            f.PrimaryLabel = "Kod Okresu";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodOkresu = f.KodOkresu;
            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisPozitivnychTestovZaObdobiePreOkres(kodOkresu, zac, kon);
                ShowTestList(list, $"Pozitivne testy v okrese {kodOkresu} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving positive tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowTestList(List<PCRTestByDate> list, string title)
        {
            PanelItems.Controls.Clear();

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.White
            };

            var sb = new StringBuilder();
            sb.AppendLine(title);
            sb.AppendLine(new string('=', Math.Min(80, Math.Max(10, title.Length))));
            sb.AppendLine();

            if (list == null || list.Count == 0)
            {
                sb.AppendLine("<no tests>");
            }
            else
            {
                foreach (var item in list)
                {
                    sb.Append(FormatTestDetailed(item.Inner));
                }
            }

            rtb.Text = sb.ToString();
            PanelItems.Controls.Add(rtb);
        }

        private string FormatTestDetailed(PCRTest test)
        {
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

        private string FormatPersonDetailed(Person person)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Patient ===");
            sb.AppendLine($"Meno: {person.Meno}");
            sb.AppendLine($"Priezvisko: {person.Priezvisko}");
            sb.AppendLine($"DatumNarodenia: {person.DatumNarodenia:yyyy-MM-dd}");
            sb.AppendLine($"UnikatneCisloPacienta: {person.UnikatneCisloPacienta}");
            sb.AppendLine();
            return sb.ToString();
        }

        //5 Vsetky testy v okrese
        private void AllTestsInDistrict()
        {
            using var f = new Form5();

            f.PrimaryLabel = "Kod Okresu";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodOkresu = f.KodOkresu;
            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisVsetkychTestovZaObdobiePreOkres(kodOkresu, zac, kon);
                ShowTestList(list, $"Vsetky testy v okrese {kodOkresu} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //6 Pozitivne testy v kraji
        private void PositiveTestsInRegion()
        {
            using var f = new Form5();

            f.PrimaryLabel = "Kod Kraju";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodKraja = f.KodOkresu;
            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisVsetkychPozitivnychTestovPreObdobieKraj(kodKraja, zac, kon);
                ShowTestList(list, $"Pozitivne testy v kraji {kodKraja} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving positive tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //7 Vsetky testy v kraji
        private void AllTestsInRegion()
        {
            using var f = new Form5();

            f.PrimaryLabel = "Kod Kraju";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodKraja = f.KodOkresu;
            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisVsetkychTestovPreObdobieKraj(kodKraja, zac, kon);
                ShowTestList(list, $"Vsetky testy v kraji {kodKraja} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //8 Vsetky pozitivne testy
        private void AllPositiveTests()
        {
            using var f = new Form5();
            f.PrimaryLabel = "Celkovo";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisVsetkychPozitivnychTestovZaObdobie(zac, kon);
                ShowTestList(list, $"Pozitivne testy ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving positive tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //9 Vsetky testy
        private void AllTests()
        {
            // Reuse Form5 to ask for date range; set label to indicate context
            using var f = new Form5();
            f.PrimaryLabel = "Celkovo";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisVsetkychTestovZaObdobie(zac, kon);
                ShowTestList(list, $"Vsetky testy ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving tests:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //10 Vypis chorych v okrese od datumu a doby X
        private void ListSickInDistrictFromDateRange()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Kod Okresu";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodOkresu = f.KodOkresu;
            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var (persons, dates) = who.VypisChorychVOkreseZaObodobieX(kodOkresu, datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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
                        sb.Append(FormatPersonDetailed(person));

                        if (dates != null && i < dates.Count)
                        {
                            sb.AppendLine($"RelevantDate: {dates[i]:yyyy-MM-dd}");
                        }

                        sb.AppendLine(new string('-', 60));
                    }
                }

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving sick persons:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //11 Vypis chorych v okrese od datumu a doby X podla vysledku
        private void ListSickInDistrictFromDateRangeByResult()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Kod Okresu";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodOkresu = f.KodOkresu;
            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var persons = who.VypisChorychVOkresePodlaHodnotyTestuZaObdobieX(kodOkresu, datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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
                    for(int i = 0; i < persons.Item1.Count; i++)
                    {
                        var person = persons.Item1[i];
                        var test = persons.Item2[i];
                        sb.Append(FormatPersonDetailed(person));
                        sb.Append(FormatTestDetailed(test));
                        sb.AppendLine(new string('-', 60));
                    }
                }

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving sick persons:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //12 Vypis chorych v kraju od datumu a doby X
        private void ListSickInRegionFromDateRange()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Kod Kraju";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodKraja = f.KodOkresu;
            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var persons = who.VypisChorychVKrajiZaObodobieX(kodKraja, datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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
                        sb.Append(FormatPersonDetailed(person));
                        sb.AppendLine(new string('-', 60));
                    }
                }

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving sick persons:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //13 Vypis chorych od datumu a doby X
        private void ListSickFromDateRange()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Obdobie";
            f.RequireCode = false;

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var persons = who.VypisChorychZaObodobieX(datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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
                        sb.Append(FormatPersonDetailed(person));
                        sb.AppendLine(new string('-', 60));
                    }
                }

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving sick persons:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //14 Vypis jednej chorej osoby okresu s najvyssim vysledkom
        private void SingleSickWithHighestResultInDistrict()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Obdobie";
            f.RequireCode = false;

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var tests = who.VypisNajchorsejOsobyOkresu(datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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
                        sb.Append(FormatTestDetailed(test));

                        if (test.Pacient != null)
                        {
                            sb.Append(FormatPersonDetailed(test.Pacient));
                        }

                        sb.AppendLine(new string('-', 60));
                    }
                }

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving results:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //15 Vypis okresov usporiadanych podla poctu pozitivnych testov
        private void DistrictsOrderedByPositiveCount()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Obdobie";
            f.RequireCode = false;

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var (okresy, counts) = who.VypisPodlaPoctuChorychPreDatumX(datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving districts ordered by positive count:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //16 Vypis krajov usporiadanych podla poctu pozitivnych testov
        private void RegionsOrderedByPositiveCount()
        {
            using var f = new Form6();

            f.PrimaryLabel = "Obdobie";
            f.RequireCode = false;

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime datum = f.Datum;
            int X = f.PocetDni;

            try
            {
                var (kraje, counts) = who.VypisKrajovPodlaPoctuChorychPreDatumX(datum, X);

                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

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

                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving regions ordered by positive count:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //17 Testy na pracovisku v obdobi
        private void TestsOnWorkplaceInPeriod()
        {
            using var f = new Form5();

            f.PrimaryLabel = "Kod Pracoviska";

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            int kodPracoviska = f.KodOkresu;
            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            try
            {
                var list = who.VypisVsetkychTestovZaObdobiePrePracovisko(kodPracoviska, zac, kon);
                ShowTestList(list, $"Testy na pracovisku {kodPracoviska} ({zac:yyyy-MM-dd} — {kon:yyyy-MM-dd}) \nPocet testov na pracovisku: {list.Count}");
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while retrieving tests for workplace:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //18 Najdi PCR podla kodu
        private void FindPcrByCode()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter PCR test code (numeric):", "Find PCR by code", "");
            if (string.IsNullOrWhiteSpace(input)) return;

            if (!int.TryParse(input.Trim(), out int code))
            {
                MessageBox.Show("Invalid code. Please enter a whole number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var test = who.VyhladajPCRTestPodlaKodu(code);
                if (test == null)
                {
                    MessageBox.Show($"No PCR test found with code {code}.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Render result into PanelItems
                PanelItems.Controls.Clear();
                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font("Consolas", 10),
                    BackColor = Color.White
                };

                var sb = new StringBuilder();
                sb.Append(FormatTestDetailed(test));
                sb.Append(FormatPersonDetailed(test.Pacient));
                rtb.Text = sb.ToString();
                PanelItems.Controls.Add(rtb);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while searching PCR test:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //19 Vloz osobu
        private void InsertPerson()
        {
            using var dlg = new Form
            {
                Text = "Vlozenie osoby",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                Size = new Size(420, 260)
            };

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(8),
                AutoSize = true
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Controls
            var tbMeno = new TextBox { Dock = DockStyle.Fill };
            var tbPriezvisko = new TextBox { Dock = DockStyle.Fill };
            var dtDatum = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, MaxDate = DateTime.Today, Dock = DockStyle.Fill };

            // Helper to add rows
            void AddRow(string labelText, Control ctrl)
            {
                var lbl = new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoSize = true };
                tbl.Controls.Add(lbl);
                tbl.Controls.Add(ctrl);
            }

            AddRow("Meno:", tbMeno);
            AddRow("Priezvisko:", tbPriezvisko);
            AddRow("Datum narodenia:", dtDatum);

            // Buttons
            var btnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, AutoSize = true };
            var btnOk = new Button { Text = "Vlozit", DialogResult = DialogResult.OK, Width = 90 };
            var btnCancel = new Button { Text = "Zrusit", DialogResult = DialogResult.Cancel, Width = 90 };
            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);

            // Spacer + buttons row
            tbl.Controls.Add(new Label());
            tbl.Controls.Add(btnPanel);

            dlg.Controls.Add(tbl);
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                // Validation
                var meno = tbMeno.Text?.Trim() ?? string.Empty;
                var priezvisko = tbPriezvisko.Text?.Trim() ?? string.Empty;
                var datum = dtDatum.Value.Date;

                if (string.IsNullOrWhiteSpace(meno))
                {
                    MessageBox.Show("Meno je povinne.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(priezvisko))
                {
                    MessageBox.Show("Priezvisko je povinne.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var person = new Person(meno, priezvisko, datum);
                    who.VlozOsobuDoSystemu(person);
                    MessageBox.Show("Osoba bola vlozena do systemu.", "Hotovo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Vkladanie osoby zlyhalo:\r\n{ex.Message}", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //20 Vymaz PCR test
        private void DeletePcrTest()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter PCR test code to delete (numeric):", "Delete PCR test", "");
            if (string.IsNullOrWhiteSpace(input)) return;

            if (!int.TryParse(input.Trim(), out int code))
            {
                MessageBox.Show("Invalid code. Please enter a whole number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete PCR test with code {code}? This operation cannot be undone.",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                who.VymazaniePCRTestu(code);
                MessageBox.Show($"PCR test {code} deleted.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PanelItems.Controls.Clear();
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete PCR test:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //21 Vymaz osobu
        private void DeletePerson()
        {
            string ucp = Microsoft.VisualBasic.Interaction.InputBox("Enter unique patient number (Unikatne cislo pacienta) to delete:", "Delete person", "");
            if (string.IsNullOrWhiteSpace(ucp)) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete person '{ucp}' and all their PCR tests? This operation cannot be undone.",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                who.VymazanieOsobyZoSystemu(ucp);
                MessageBox.Show($"Person '{ucp}' deleted.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PanelItems.Controls.Clear();
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete person:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            who.NacitajZoSuboru();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            who.NacitajDoSuboru();
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            who.Generuj();
        }
    }
}
