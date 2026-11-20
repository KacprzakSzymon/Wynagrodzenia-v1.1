using System;
using System.Collections.Generic;
using System.Text;

namespace wpf
{
    public class WynagrodzenieModel
    {
        public DateTime Data { get; set; }
        public decimal KwotaBrutto { get; set; }
        public string TypUmowy { get; set; }
        public decimal Netto { get; set; }
        public string Ulga { get; set; }
    }
}
