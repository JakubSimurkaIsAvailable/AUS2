using System;
using System.Windows.Forms;
using AUS2.WHO;

namespace AUS2GUI
{
    public partial class Form2 : Form
    {
        public PCRTest CreatedTest { get; private set; }

        public Form2()
        {
            InitializeComponent();

            button1.Click += Button1_Click;
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            try
            {
                DateTime datum = dateTimePicker1.Value;
                string unikatneCisloPacienta = textBox1.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(unikatneCisloPacienta))
                {
                    MessageBox.Show("Unikatne cislo pacienta is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBox2.Text?.Trim(), out int kodPracoviska))
                {
                    MessageBox.Show("Kod Pracoviska must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBox3.Text?.Trim(), out int kodOkresu))
                {
                    MessageBox.Show("Kod Okresu must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBox4.Text?.Trim(), out int kodKraja))
                {
                    MessageBox.Show("Kod Kraja must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                
                if (!double.TryParse(textBox6.Text?.Trim(), out double hodnotaTestu))
                {
                    var res = MessageBox.Show("Hodnota testu is not a valid number. Use 0.0?", "Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.No) return;
                    hodnotaTestu = 0.0;
                }

                string poznamka = textBox7.Text ?? string.Empty;
                bool vysledok = checkBox1.Checked;

                

                CreatedTest = new PCRTest(datum, unikatneCisloPacienta, kodPracoviska, kodOkresu, kodKraja, vysledok, hodnotaTestu, poznamka);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse input: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
