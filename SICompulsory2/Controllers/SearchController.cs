using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
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
                return Json(new { results = _db.Terms.Select(x => x.Text) });

            var term = q;
            IList<string> acronyms = new List<string>();
            await _db.SpecialCharacters.Where(x =>
                term.Contains(char.ConvertFromUtf32(x.SpecialCharacter)) //find all specialcharacters in the term
            ).ForEachAsync(x => {
                //foreach special character in the word add a word with the special character replaced with the acronym
                //string[] acronymsFound = acronyms.AsEnumerable().ToArray();
                //foreach (string s in acronymsFound)
                //    GenerateAllVariationsWithReplacement(s, char.ConvertFromUtf32(x.SpecialCharacter),x.Acronym , ref acronyms);

                GenerateAllVariationsWithReplacement(term, char.ConvertFromUtf32(x.SpecialCharacter), x.Acronym, ref acronyms);
            });
            await _db.SpecialCharacters.Where(x =>
                term.Contains(x.Acronym) // if term contains an acronym from the db
            ).ForEachAsync(x =>
             {
                 string[] acronymsFound = acronyms.AsEnumerable().ToArray();
                 foreach (string s in acronymsFound)
                     if (s.Contains(x.Acronym))
                         GenerateAllVariationsWithReplacement(s, x.Acronym, char.ConvertFromUtf32(x.SpecialCharacter), ref acronyms);

                 GenerateAllVariationsWithReplacement(term, x.Acronym, char.ConvertFromUtf32(x.SpecialCharacter), ref acronyms);

             });
            string sql = "SELECT * FROM Terms where text like " + "'%" + term + "%'";
            foreach (string word in acronyms)
                sql += (" OR text like " + "'%" + word + "%'");

            var results = _db.Terms.FromSql(sql);

            return Json(new {
                results = results.OrderBy(x => x.Text),
                searchTerms = acronyms
            });
        }

        private IQueryable<int> GetSpecilCharactersFor(string s ) => _db.SpecialCharacters.Where(x => x.Acronym == s).Select(x => x.SpecialCharacter);
        private string GetAcronymFor(int i) => _db.SpecialCharacters.FirstOrDefault(x => x.SpecialCharacter == i).Acronym;

        private IList<string> GenerateAllVariationsWithReplacement(string term, string oldVal,string newVal, ref IList<string> results)
        {
            IList<string> tmpResults = new List<string>();
            for (int i = 0; i <= term.Length-1; i++)
                for(int j = 1; j<=term.Length -i;j++)
                {
                    var s = term.Substring(i, j);
                        if (s.Contains(oldVal)) {
                            string preS = term.Substring(0, i);
                            string postS = term.Substring(i + j);
                            var newTerm = preS + s.Replace(oldVal, newVal) + postS;
                            if (!tmpResults.Contains(newTerm))
                                tmpResults.Add( newTerm);
                        }
                }
            foreach (String s in tmpResults)
                results.Add(s);
            return results;
        }
    }
}
