using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartaPacjenta {
    public class ResourceEntry {
        public string resourceType { get; set; }
        public string id { get; set; }
        public metaStruct meta { get; set; }
        public textStruct text { get; set; }
    }
    public struct metaStruct {
        public string versionId { get; set; }
        public string lastUpdated { get; set; }
    }
    public struct textStruct {
        public string status { get; set; }
        public string div { get; set; }
    }
}
