using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartaPacjenta {
    public class SearchResult {
        public string id { get; set; }
        public string birthDate { get; set; }
        public metaStruct meta { get; set; }
        public nameStruct[] name { get; set; }
        public addressStruct[] address { get; set; }
    }

    public struct nameStruct {
        public string family { get; set; }
        public string[] given { get; set; }
    }

    public struct addressStruct {
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string[] line { get; set; }
    }
}
