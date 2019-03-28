using Microsoft.AspNetCore.Mvc;
using SICompulsory2.DB;
using SICompulsory2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SICompulsory2.Controllers
{
    [Route("")]
    public class AcronymController : Controller
    {
        private readonly TermContext _db;
        public AcronymController(TermContext db)
        {
            _db = db;
        }
        [HttpGet()]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost()]
        public IActionResult CreateAcronym(string acronym, char specialCharacter)
        {
            try
            {
                _db.SpecialCharacters.Add(new SpecialCharacters { Acronym = acronym, SpecialCharacter = (int)specialCharacter });
                _db.SaveChanges();
                return View("index");
            }
            catch (Exception ex) { 
                return Json(new { success = false,exception = ex });
            }
        }
    }
}
