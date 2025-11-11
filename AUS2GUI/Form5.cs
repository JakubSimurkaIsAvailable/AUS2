using System;
using System.Windows.Forms;

namespace AUS2GUI
{
    public partial class Form5 : Form
    {
        private bool _requireCode = true;

        public Form5()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
            AcceptButton = button1;
            UpdateRequireCodeVisibility();
        }

        public string PrimaryLabel
        {
            get => label1.Text;
            set => label1.Text = value ?? string.Empty;
        }

        public bool RequireCode
        {
            get => _requireCode;
            set
            {
                _requireCode = value;
                UpdateRequireCodeVisibility();
            }
        }

        public int KodOkresu { get; private set; }
        public DateTime Zaciatok => dateTimePicker1.Value.Date;
        public DateTime Koniec => dateTimePicker2.Value.Date;

        private void UpdateRequireCodeVisibility()
        {
            textBox1.Visible = _requireCode;
            // If code is not required, clear previous value to avoid confusion.
            if (!_requireCode)
            {
                textBox1.Text = string.Empty;
                KodOkresu = 0;
            }
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            int kod = 0;
            if (_requireCode)
            {
                if (!int.TryParse(textBox1.Text?.Trim(), out kod))
                {
                    MessageBox.Show("Kod musí byť celé číslo.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
