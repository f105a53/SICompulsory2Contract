using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICompulsory2.DB;

namespace SICompulsory2.Controllers
{
    [Route("search")]
    public class SearchController : Controller
    {
        private readonly TermContext _db;
        public SearchController(TermContext db) {
            _db = db;
        }
        [HttpGet("acronyms")]
        public async Task<JsonResult> GetAcronyms() 
            => Json(
            new { results =
                _db.SpecialCharacters.OrderBy(x => x.Acronym)
                    .Select(x => new {
                        acronym = x.Acronym,
                        specialCharacter = char.ConvertFromUtf32(x.SpecialCharacter)
                    })
                }
            );
        // GET api/value
        [HttpGet("")]
        public async Task<JsonResult> GetAsync(string q)
        {
            if (q == null) // if no parameter return all
                return Json(new { results= _db.Terms.Select(x => x.Text) } );

            var term = q;
            IList<string> acronymsChecked = new List<string>();
            IList<string> acronyms = new List<string>();
            await _db.SpecialCharacters.Where(x => 
                term.Contains(char.ConvertFromUtf32(x.SpecialCharacter)) //find all specialcharacters in the term
            ).ForEachAsync(x =>{
                //foreach special character in the word add a word with the special character replaced with the acronym
                var alreadyFoundAcronyms = acronyms.ToArray();
                    foreach (string s in alreadyFoundAcronyms)
                     GenerateAllVariationsWithReplacement(s, char.ConvertFromUtf32(x.SpecialCharacter), x.Acronym,ref acronyms);

                GenerateAllVariationsWithReplacement(term, char.ConvertFromUtf32(x.SpecialCharacter), x.Acronym, ref acronyms);

            });
            await _db.SpecialCharacters.Where(x => 
                term.Contains(x.Acronym) // if term contains an acronym from the db
            ).ForEachAsync(x => 
             {
                 var alreadyFoundAcronyms = acronyms.ToArray();
                 foreach (string s in alreadyFoundAcronyms)
                    GenerateAllVariationsWithReplacement(s, x.Acronym, char.ConvertFromUtf32(x.SpecialCharacter), ref acronyms);

                 GenerateAllVariationsWithReplacement(term, x.Acronym,char.ConvertFromUtf32(x.SpecialCharacter),ref acronyms);

             });
            string sql = "SELECT * FROM Terms where text like " + "'%" + term + "%'";
            foreach (string word in acronyms)
                sql += ( " OR text like " + "'%" + word + "%'");

            return Json( new {
                results = _db.Terms.FromSql(sql).Select(x => x.Text),
                searchTerms = acronyms
            });
        }
        private IQueryable<int> GetSpecilCharactersFor(string s ) => _db.SpecialCharacters.Where(x => x.Acronym == s).Select(x => x.SpecialCharacter);
        private string GetAcronymFor(int i) => _db.SpecialCharacters.FirstOrDefault(x => x.SpecialCharacter == i).Acronym;

        private IList<string> GenerateAllVariationsWithReplacement(string term, string oldVal,string newVal, ref IList<string> results)
        {
            for (int i = 1; i <= term.Length; i++)
            {
                if (i < term.Length - 1)
                {
                    var s0 = term.Substring(0, i);
                    var s1 = term.Substring(i, term.Length-i);
                    var newWord = s0.Replace(oldVal, newVal) + s1;
                    if (newWord != term && !results.Contains(newWord))
                        results.Add(newWord);

                    var s2 = term.Substring(term.Length - i);
                    var newWord2 = term.Substring(0, term.Length-i) + s2.Replace(oldVal, newVal);
                    if (newWord2 != term && !results.Contains(newWord2))
                        results.Add(newWord2);

                }
                else if (i == term.Length - 1)
                {

                    var s0 = term.Substring(0, i);
                    var s1 = term.Substring(i);
                    var newWord = s0.Replace(oldVal, newVal) +s1;
                    if (newWord != term && !results.Contains(newWord))
                        results.Add(newWord);
                }
                else if(i == term.Length)
                {
                    var newWord = term.Replace(oldVal, newVal);
                    if (newWord != term && !results.Contains(newWord))
                        results.Add(newWord);
                }
            }
            return results;
        }
        private int[] Replace(int[] arr,int newVal, int index)
        {
            int[] newArr = arr.Select(x => x).ToArray();
            newArr[index] = newVal;
            return newArr;
        }
        private string MakeString(int[] arr)
        {
            string s = "";
            foreach (int i in arr) {
                s += char.ConvertFromUtf32(i);
            }
            return s;
        }
    }
}
