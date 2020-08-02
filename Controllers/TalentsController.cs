using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheLifeTimeTalents.Models;

namespace TheLifeTimeTalents.Controllers
{
    [Route("api/[controller]")]
    public class TalentsController : Controller
    {

        private static readonly TalentRepository repository = new TalentRepository();

        //[EnableCors]
        [HttpGet]
        public IActionResult GetAllTalents()
        {
            return Ok(repository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetTalent(int id)
        {
            Talent talent = repository.Get(id);
            if (talent == null)
            {
                return BadRequest(new { message = "Talent cannot be found!" });
            }
            return Ok(talent);
        }

        [HttpPost]
        public IActionResult AddTalent(Talent talent)
        {
            if (talent == null)
            {
                return BadRequest(new { message = "Talent cannot be empty!" });
            }
            repository.Add(talent);
            return Ok(talent);
        }

        [HttpPut("{id}")]
        public IActionResult EditTalent(int id, Talent talent)
        {
            if (talent == null)
            {
                return BadRequest(new { message = "Talent cannot be empty!" });
            }
            talent.Id = id;
            repository.Update(talent);
            return Ok(talent);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTalent(int id)
        {
            repository.Remove(id);
            return Ok(new { message = "Deleted " + id });
        }
    }
}

