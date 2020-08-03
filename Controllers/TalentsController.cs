using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheLifeTimeTalents.Models;
using TheLifeTimeTalents.Data;
using System.Collections.Immutable;
using TheLifeTimeTalents.Helpers;

namespace TheLifeTimeTalents.Controllers
{
    [Route("api/[controller]")]
    public class TalentsController : Controller
    {

        private ApplicationDbContext Database { get; }
        //Create a Constructor, so that the .NET engine can pass in the 
        //ApplicationDbContext object which represents the database session.
        public TalentsController(ApplicationDbContext databaseFromServicesContainer)

        {
            //Reference the database object which is residing inside the Web App's
            //service container.
            //       _appSettings = appSettings.Value;
            Database = databaseFromServicesContainer;

        }
        private static readonly TalentRepository repository = new TalentRepository();

        //[EnableCors]
        [HttpGet]
        public IActionResult GetAllTalents()
        {
            List<Talent> TalentQueryResults = new List<Talent>();
            List<object> talentList = new List<object>();
            TalentQueryResults = Database.Talent.ToList();
            try
            {
                foreach (var oneTalent in TalentQueryResults)
                {
                    //  oneEvent.EventName.Sort();
                    talentList.Add(new
                    {
                        id = oneTalent.Id,
                        name = oneTalent.Name,
                        shortName = oneTalent.ShortName,
                        reknown = oneTalent.Reknown,
                        bio = oneTalent.Bio,
                        image = oneTalent.ImageURL,
                        createdBy = oneTalent.CreatedBy
                        

                    });
                }
            }

            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(talentList);
           

     
        }

        [HttpGet("{id}")]
        public IActionResult GetTalent(int id)
        {
          
            Talent onetalent = new Talent();
            if (onetalent == null)
            {
                return BadRequest(new { message = "Talent cannot be found!" });
            }
            try
            {

                var foundOneTalent = Database.Talent
                    .Where(eachTalent => eachTalent.Id == id).Single();
                onetalent.Id = foundOneTalent.Id;
                onetalent.Name = foundOneTalent.Name;
                onetalent.ShortName = foundOneTalent.ShortName;
                onetalent.Reknown = foundOneTalent.Reknown;
                onetalent.Bio = foundOneTalent.Bio;
                onetalent.CreatedBy = foundOneTalent.CreatedBy;
                onetalent.ImageURL = foundOneTalent.ImageURL;
         

            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(onetalent);
        }
           
          // return Ok(talent);
    
        

        [HttpPost]
        public IActionResult AddTalent(Talent talent)
        {
            string customMessage = "";
            Talent newTalent = new Talent();
            if (talent == null)
            {
                return BadRequest(new { message = "Talent cannot be empty!" });
            }
         //   repository.Add(talent);
            //Database.Talent.Add(talent);
            //Database.SaveChanges();//Telling the database model to save the changes
            return Ok(talent);
        }


        [HttpPost("Add")]
        public IActionResult Add([FromForm] IFormCollection webFormdata)
        {
            string customMessage = "";
            Talent newTalent = new Talent();
            try
            {
                //Copy out all the venue data into the new Venue instance,
                //new.
                //  int userId = int.Parse(User.FindFirst("userid").Value);
           //    newTalent.Id = int.Parse(data["id"]);
                newTalent.Name = webFormdata["name"]; 
                newTalent.ShortName = webFormdata["shortName"];
                newTalent.Reknown = webFormdata["reknown"];
                newTalent.Bio = webFormdata["bio"];
                newTalent.ImageURL = webFormdata["imageURL"];
                newTalent.CreatedBy = 1;

                //person incharge
                Database.Talent.Add(newTalent);
                Database.SaveChanges();//Telling the database model to save the changes
            }
            catch (Exception exceptionObject)
            {
                if (exceptionObject.InnerException.Message
                          .Contains("Talent_Name_UniqueConstraint") == true)
                {
                    customMessage = "Unable to save Talent record due " +
                                  "to another record having the same TalentName: " +
                                  webFormdata["name"];
                    //Create an anonymous type object that has one property, message.
                    //This anonymous object's message property contains a simple string message
                    object httpFailRequestResultMessage = new { message = customMessage };
                    //Return a bad http request message to the client
                    return BadRequest(httpFailRequestResultMessage);
                }
            }//End of Try..Catch block

            //If there is no runtime error in the try catch block, the code execution
            //should reach here. Sending success message back to the client.

            //******************************************************
            //Construct a custom message for the client
            //Create a success message anonymous type object which has a 
            //message member variable (property)
            var successRequestResultMessage = new
            {
                message = "Saved Talent record"

            };
            //Create a OkObjectResult class instance, httpOkResult.
            //When creating the object, provide the previous message object into it.
            OkObjectResult httpOkResult =
                 new OkObjectResult(successRequestResultMessage);
            //Send the OkObjectResult class object back to the client.
            return httpOkResult;
         
            }

        [HttpPut("{id}")]
        public IActionResult EditTalent(int id, Talent talent, [FromForm] IFormCollection webFormdata)
        {
            if (talent == null)
            {
                return BadRequest(new { message = "Talent cannot be empty!" });
            }
            var oneTalent = Database.Talent
              .Where(talentEntity => talentEntity.Id == id).Single();
            oneTalent.Name = webFormdata["name"];
            oneTalent.ShortName = webFormdata["shortName"];
            oneTalent.Reknown = webFormdata["reknown"];
            oneTalent.Bio = webFormdata["bio"];
            oneTalent.ImageURL = webFormdata["imageURL"];
            Database.Talent.Update(oneTalent);
            Database.SaveChanges();
            talent.Id = id;
          

            return Ok(new { message = "Edited talent record of " + oneTalent.Name });


        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTalent(int id)
        {
            var foundOneTalent = Database.Talent
                         .Single(eachTalent => eachTalent.Id == id);
            Database.Talent.Remove(foundOneTalent);
            Database.SaveChanges();
       
            return Ok(new { message = "Deleted talent record of id " + id  });
        }
    }
}

