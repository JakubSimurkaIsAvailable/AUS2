using System;
using System.Windows.Forms;

namespace AUS2GUI
{
    public partial class Form6 : Form
    {
        public int KodOkresu { get; private set; }
        public DateTime Datum => dateTimePicker1.Value.Date;
        public int PocetDni { get; private set; }

        public Form6()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
            AcceptButton = button1;
        }

        public string PrimaryLabel
        {
            get => label1.Text;
            set => label1.Text = value ?? string.Empty;
        }

        public bool RequireCode { get; set; } = true;

        private void Button1_Click(object? sender, EventArgs e)
        {
            if (RequireCode)
            {
                if (!int.TryParse(textBox2.Text?.Trim(), out int kod))
                {
                    MessageBox.Show("Kod musí byť celé číslo.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                KodOkresu = kod;
            }
            else
            {
                KodOkresu = 0;
            }

            if (!int.TryParse(textBox1.Text?.Trim(), out int pocet))
            {
                MessageBox.Show("Počet dní musí byť celé číslo.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pocet < 0)
            {
                MessageBox.Show("Počet dní musí byť nezáporné číslo.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PocetDni = pocet;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
