using Microsoft.AspNetCore.Mvc;
using SICompulsory2.DB;
using SICompulsory2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SICompulsory2.Controllers
{
    [Route("term")]
    public class TermController : Controller
    {
        private readonly TermContext _db;
        public TermController(TermContext db) {
            _db = db;
        }
        [HttpPost("addTerm")]
        public async Task<IActionResult> AddTerm(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                try
                {
                    _db.Terms.Add(new Term { Text = term });
                    _db.SaveChanges();
                    return Json(new { success = true, term = new { Text = term } });
                } catch (Exception) { return Json(new { succeess = false }); }
            }
            else return Json(new { success = false });
        }
    }
}
