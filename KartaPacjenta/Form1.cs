using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KartaPacjenta {
    public partial class Form1 : Form {
        private List<Pacjent> listaPacjentow;
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            
        }

        private void getPatientsFHIR(string uri) {
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
            IList<SearchResult> searchResults = new List<SearchResult>();
            foreach (JToken result in results) {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                SearchResult searchResult = result["resource"].ToObject<SearchResult>();
                searchResults.Add(searchResult);
            }

            listaPacjentow = new List<Pacjent>();
            Pacjent p;
            string tmpstr;

            foreach(SearchResult sr in searchResults) {
                Console.WriteLine("Parsed patient: {0}", sr.id);
                p = new Pacjent();
                p.id = sr.id;
                p.dataUrodzenia = sr.birthDate;
                p.dataAktualizacji = sr.meta.lastUpdated;
                p.adres = "";

                if (sr.address != null && sr.address.Length > 0 && sr.address[0].line != null && sr.address[0].line.Length > 0)
                    p.adres += "   " + sr.address[0].line[0] + Environment.NewLine;

                if (sr.address != null && sr.address.Length > 0 && sr.address[0].city != null && sr.address[0].city.Length > 0)
                    p.adres += "   " + sr.address[0].city + Environment.NewLine;

                if (sr.address != null && sr.address.Length > 0 && sr.address[0].country != null && sr.address[0].country.Length > 0)
                    p.adres += "   " + sr.address[0].country + Environment.NewLine;

                if (sr.name != null && sr.name.Length > 0 && sr.name[0].given !=null && sr.name[0].given.Length > 0)
                    tmpstr = sr.name[0].given[0];
                else
                    tmpstr = "unknown";
                p.imie = tmpstr.First().ToString().ToUpper() + tmpstr.Substring(1).ToLower();
                if (sr.name != null)
                    tmpstr = sr.name[0].family;
                else
                    tmpstr = "unknown";
                p.nazwisko = tmpstr.First().ToString().ToUpper() + tmpstr.Substring(1).ToLower();

                listaPacjentow.Add(p);
            }

            listaPacjentow.Sort((a, b) => a.dataAktualizacji.CompareTo(b.dataAktualizacji));

            foreach (Pacjent pac in listaPacjentow) {
                listBox1.Items.Add(pac.id + " || " + pac.dataUrodzenia + " || " + pac.imie + " " + pac.nazwisko);
            }
            
            listBox1.SelectedIndex = 0;

        }

        private void button1_Click(object sender, EventArgs e) {
            string uri = "http://hapi.fhir.org/baseDstu3/Patient/_search?_count=100&birthdate=gt1925-01-01";

            string imie = Regex.Replace(textBox1.Text, @"\s+", "");
            string nazwisko = Regex.Replace(textBox2.Text, @"\s+", "");
            if(imie.Length > 0) {
                uri += "&given=" + imie;
            }
            if (nazwisko.Length > 0) {
                uri += "&family=" + nazwisko;
            }

            listBox1.Items.Clear();

            getPatientsFHIR(uri);
        }

        private void button3_Click(object sender, EventArgs e) {
            Pacjent wybranyPacjent = listaPacjentow[listBox1.SelectedIndex];
            Form2 form2 = new Form2(wybranyPacjent);
            form2.Show();
        }
    }
}
