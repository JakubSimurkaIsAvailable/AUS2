using System;
using System.Windows.Forms;

namespace AUS2GUI
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        
        public string PrimaryLabel
        {
            get => label1.Text;
            set => label1.Text = value ?? string.Empty;
        }

        public int KodOkresu { get; private set; }
        public DateTime Zaciatok => dateTimePicker1.Value.Date;
        public DateTime Koniec => dateTimePicker2.Value.Date;

        private void Button1_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text?.Trim(), out int kod))
            {
                MessageBox.Show("Kod musí byť celé číslo.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
            {
                var res = MessageBox.Show("Dátum od je neskorší ako dátum do. Chcete ich zameniť?", "Dátumy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    var tmp = dateTimePicker1.Value;
                    dateTimePicker1.Value = dateTimePicker2.Value;
                    dateTimePicker2.Value = tmp;
                }
                else
                {
                    return;
                }
            }

            KodOkresu = kod;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
