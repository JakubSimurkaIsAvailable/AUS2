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

        // helper: Form1 only collects input and writes out the string returned by WHOcontroller
        private void DisplayOutput(string text)
        {
            PanelItems.Controls.Clear();

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.White,
                Text = text ?? string.Empty
            };

            PanelItems.Controls.Add(rtb);
        }

        // ----- Action handlers -----

        //1 Vloz vysledok PCR testu
        private void InsertPcrResult()
        {
            using var f = new Form2();
            var dr = f.ShowDialog(this);
            if (dr == DialogResult.OK && f.CreatedTest != null)
            {
                var output = who.InsertPcrResultFormatted(f.CreatedTest);
                DisplayOutput(output);
            }
        }

        //2 Vyhladaj test pre pacienta
        private void FindTestForPatient()
        {
            using var f = new Form3(who);
            var dr = f.ShowDialog(this);
            if (dr == DialogResult.OK && f.FoundTest != null && f.FoundPerson != null)
            {
                var output = who.FormatTestAndPerson(f.FoundTest, f.FoundPerson);
                DisplayOutput(output);
            }
        }

        //3 Vypis vsetky testy pre pacienta
        private void ListAllTestsForPatient()
        {
            using var f = new Form4();
            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            var ucp = f.UnikatneCisloPacienta;
            var output = who.GetAllTestsForPatientFormatted(ucp);
            DisplayOutput(output);
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

            var output = who.GetPositiveTestsInDistrictFormatted(kodOkresu, zac, kon);
            DisplayOutput(output);
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

            var output = who.GetAllTestsInDistrictFormatted(kodOkresu, zac, kon);
            DisplayOutput(output);
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

            var output = who.GetPositiveTestsInRegionFormatted(kodKraja, zac, kon);
            DisplayOutput(output);
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

            var output = who.GetAllTestsInRegionFormatted(kodKraja, zac, kon);
            DisplayOutput(output);
        }
        //8 Vsetky pozitivne testy
        private void AllPositiveTests()
        {
            using var f = new Form5();
            f.PrimaryLabel = "Celkovo";
            f.RequireCode = false;

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            var output = who.GetAllPositiveTestsFormatted(zac, kon);
            DisplayOutput(output);
        }
        //9 Vsetky testy
        private void AllTests()
        {
            using var f = new Form5();
            f.PrimaryLabel = "Celkovo";
            f.RequireCode = false;

            var dr = f.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            DateTime zac = f.Zaciatok;
            DateTime kon = f.Koniec;

            var output = who.GetAllTestsFormatted(zac, kon);
            DisplayOutput(output);
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

            var output = who.ListSickInDistrictFromDateRangeFormatted(kodOkresu, datum, X);
            DisplayOutput(output);
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

            var output = who.ListSickInDistrictFromDateRangeByResultFormatted(kodOkresu, datum, X);
            DisplayOutput(output);
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

            var output = who.ListSickInRegionFromDateRangeFormatted(kodKraja, datum, X);
            DisplayOutput(output);
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

            var output = who.ListSickFromDateRangeFormatted(datum, X);
            DisplayOutput(output);
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

            var output = who.SingleSickWithHighestResultInDistrictFormatted(datum, X);
            DisplayOutput(output);
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

            var output = who.DistrictsOrderedByPositiveCountFormatted(datum, X);
            DisplayOutput(output);
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

            var output = who.RegionsOrderedByPositiveCountFormatted(datum, X);
            DisplayOutput(output);
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

            var output = who.TestsOnWorkplaceInPeriodFormatted(kodPracoviska, zac, kon);
            DisplayOutput(output);
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

            var output = who.FindPcrByCodeFormatted(code);
            DisplayOutput(output);
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

                var person = new Person(meno, priezvisko, datum);
                var output = who.InsertPersonFormatted(person);
                DisplayOutput(output);
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

            var output = who.DeletePcrTestFormatted(code);
            DisplayOutput(output);
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

            var output = who.DeletePersonFormatted(ucp);
            DisplayOutput(output);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var output = who.ImportFromFileFormatted();
            DisplayOutput(output);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var output = who.ExportToFileFormatted();
            DisplayOutput(output);
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var output = who.GenerateFormatted();
            DisplayOutput(output);
        }
    }
}
