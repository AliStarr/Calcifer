using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calcifer.Utility
{
    // Quick model for JSON API consumption.
    internal class APOTDHelper
    {
        public string Copyright { get; set; }
        public DateTime Date { get; set; }
        public string Explanation { get; set; }
        public string HDUrl { get; set; }
        public string MediaType { get; set; }
        public string ServiceVersion { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
    }
}
