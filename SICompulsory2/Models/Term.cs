using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SICompulsory2.Models
{
    public class Term
    {
        public string Text { get; set; }

        public int DistanceFrom(string term,IEnumerable<string> acronyms) {
            // term = 'app' LOW
            // text = 'application' pretty high
            // return high distance
            var termCharArray = term.ToCharArray();
            var textCharArray = Text.ToCharArray();
            var smallest = textCharArray.Length < termCharArray.Length ? textCharArray : termCharArray;
            var notSmallest = textCharArray.Length < termCharArray.Length ? termCharArray : textCharArray;
            int difference = 0;
            for (int i = 0; i < smallest.Length; i++) {
                bool contained = notSmallest.Contains(smallest[i]);
                bool inAcronyms = false;
                foreach (string s in acronyms) //check if acronyms contains letter
                    if (s.Contains(smallest[i])) {
                        inAcronyms = true;
                        break;
                    }
                if (!contained && !inAcronyms) // if term and acronyms doesn't contain letter add 2 difference
                    difference += 2;
                else if (!contained && inAcronyms)  // if term is only in acronyms add 1 difference
                    difference++;

            }

            difference += notSmallest.Length - smallest.Length;
            if (Text.Contains(term) && !string.IsNullOrEmpty(term) )
                difference += Text.IndexOf(term.Substring(0,1) ); //create a bigger difference if term is contained further into the word
            return difference;
            
        }
    }
}

