using System;
using System.Windows.Forms;
using AUS2.WHO;

namespace AUS2GUI
{
    public partial class Form3 : Form
    {
        public WHOcontroller Who { get; set; }

        public PCRTest FoundTest { get; private set; }
        public Person FoundPerson { get; private set; }

        public Form3()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        public Form3(WHOcontroller who) : this()
        {
            Who = who ?? throw new ArgumentNullException(nameof(who));
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            if (Who == null)
            {
                MessageBox.Show("WHO instance is not set. Call this form with new Form3(who).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox1.Text?.Trim(), out int nk))
            {
                MessageBox.Show("Unikatny kod PCR testu must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ucp = textBox2.Text?.Trim();
            if (string.IsNullOrWhiteSpace(ucp))
            {
                MessageBox.Show("Unikatne cislo pacienta is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var (test, person) = Who.VyhladajVysledokTestu(nk, ucp);
                FoundTest = test;
                FoundPerson = person;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (KeyNotFoundException knf)
            {
                MessageBox.Show(knf.Message, "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while searching:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
