using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SICompulsory2.Models
{
    public class Term
    {
        public string Text { get; set; }

        public int DistanceFrom(string term) {
            // term = 'app' LOW
            // text = 'application' pretty high
            // return high distance
            string s = Text.Substring(Text.IndexOf(term));
            return s.ToCharArray().Select(x => (int)x) .Sum() - term.ToCharArray().Select(x => (int)x).Sum();
        }
    }
}

