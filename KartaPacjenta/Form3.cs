using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KartaPacjenta {
    public partial class Form3 : Form {
        Pacjent pacjent;
        ZasobMedyczny zasob;
        public Form3(Pacjent p, ZasobMedyczny zm) {
            InitializeComponent();
            pacjent = p;
            zasob = zm;
            label4.Text = pacjent.imie + " " + pacjent.nazwisko;
            label5.Text = zasob.nazwa;
            label6.Text = zasob.data;
        }

        private void Form3_Load(object sender, EventArgs e) {
            webBrowser1.DocumentText = zasob.tresc;
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
