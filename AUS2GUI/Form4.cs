using System;
using System.Windows.Forms;

namespace AUS2GUI
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        public string UnikatneCisloPacienta => textBox1.Text?.Trim() ?? string.Empty;

        private void Button1_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Unikatné číslo pacienta je povinné.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
