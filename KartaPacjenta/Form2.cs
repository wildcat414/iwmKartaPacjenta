using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KartaPacjenta {
    public partial class Form2 : Form {
        private Pacjent pacjent;
        private List<ZasobMedyczny> listaZasobow;
        public Form2(Pacjent p) {
            InitializeComponent();
            pacjent = p;
            this.Text = "Wybrany pacjent: " + pacjent.imie + " " + pacjent.nazwisko;
            string plec;
            if(pacjent.plec == "male") {
                plec = "mężczyzna";
            } else if(pacjent.plec == "female") {
                plec = "kobieta";
            } else {
                plec = "brak danych";
            }
            textBox3.Text = "ID: " + pacjent.id + Environment.NewLine +
                "Imię: " + pacjent.imie + Environment.NewLine + 
                "Nazwisko: " + pacjent.nazwisko + Environment.NewLine +
                "Płeć: " + plec;
            textBox4.Text = "Adres:" + Environment.NewLine + pacjent.adres;
        }

        private void Form2_Load(object sender, EventArgs e) {
            string uri = "http://hapi.fhir.org/baseDstu3/Patient/"+ pacjent.id +"/$everything";
            getPatientDataFHIR(uri);
        }

        private void getPatientDataFHIR(string uri) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Console.WriteLine("Content length is {0}", response.ContentLength);
            Console.WriteLine("Content type is {0}", response.ContentType);

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            Console.WriteLine("Response stream received.");
            // Console.WriteLine(readStream.ReadToEnd());

            string res = readStream.ReadToEnd();
            Console.WriteLine("Request result length: {0}", res.Length);

            response.Close();
            readStream.Close();


            JObject googleSearch = JObject.Parse(res);

            // get JSON result objects into a list
            IList<JToken> results = googleSearch["entry"].Children().ToList();

            // serialize JSON results into .NET objects
            IList<ResourceEntry> resourceEntries = new List<ResourceEntry>();
            foreach (JToken result in results) {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                ResourceEntry resourceEntry = result["resource"].ToObject<ResourceEntry>();
                resourceEntries.Add(resourceEntry);
            }

            listaZasobow = new List<ZasobMedyczny>();
            ZasobMedyczny z;
            string tmpstr;

            foreach (ResourceEntry re in resourceEntries) {
                Console.WriteLine("Parsed resource: {0}", re.id);
                z = new ZasobMedyczny();
                z.id = re.id;
                z.nazwa = re.resourceType;
                z.data = re.meta.lastUpdated;
                z.tresc = re.text.div;
                if(true || z.nazwa == "Patient" || z.nazwa == "Observation" || z.nazwa == "Medication" || z.nazwa == "MedicationStatement") {
                    listaZasobow.Add(z);
                }
            }
            
            listaZasobow.Sort((a, b) => a.data.CompareTo(b.data));

            foreach (ZasobMedyczny zas in listaZasobow) {
                listBox1.Items.Add(zas.id + " || " + zas.nazwa + " || " + zas.data);
            }

            listBox1.SelectedIndex = 0;

        }

        private void button3_Click(object sender, EventArgs e) {
            ZasobMedyczny wybranyZasob = listaZasobow[listBox1.SelectedIndex];
            Form3 form3 = new Form3(pacjent, wybranyZasob);
            form3.Show();
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
