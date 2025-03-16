using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Business;
using Project.commands;
using Project.Infrastructure;
using Project.Interface;
using Project.Models;
using Shared.commands;
using System.Text.Json;
using Shared.Interface;


namespace Project.Controllers
{
    [Route("[controller]")]

    public class PhysicalPersonController : Controller
    {
        private readonly IPhysicalPersonService _service;
    
        public PhysicalPersonController(IPhysicalPersonService service)
        {
            _service = service;
         

        }


        [HttpPost]  
        public async Task<IActionResult> CreatePhysicalPerson(CreatePersonCommandDto physicalPerson)
        {
            if (physicalPerson == null)
            {
                return BadRequest("Invalid person data.");
            }

            var personId = await _service.CreatePersonAsync(physicalPerson);
            return Ok(personId);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreatePersonCommandDto person)
        {
            if (person == null)
            {
                return BadRequest("Invalid person data.");
            }

            var personId = await _service.CreatePersonAsync(person);
            return RedirectToAction("Details", new { id = personId });
        }


        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }





        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var person = await _service.GetPhysicalPersonAsync(id);
            if (person == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            return View(person);
        }
        [HttpGet("Details")]
        public async Task<IActionResult> AllDetails()
        {
            var persons = await _service.AllGetPhysicalPersonAsync();

            // მაშინ, თუ პირი არ არსებობს, მაინც დაგიბრუნდება გვერდი, მაგრამ ცარიელი მონაცემები
            return View(persons ?? new List<PhysicalPerson>());
        }



        [HttpPost]
        [Route("AddRelation")]
        public async Task<IActionResult> AddRelation(int fromId, int toId, RelationType relationType)
        {
            try
            {
                await _service.AddRelationAsync(fromId, toId, relationType);

               
                return RedirectToAction("Details", "PhysicalPerson", new { id = fromId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("AddRelation");
            }
        }


        [HttpGet("AddRelation")]
        public IActionResult AddRelation()
        {
            return View();
        }




        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePhysicalPerson(int id)
        //{
        //    try
        //    {
        //        await _service.DeletePersonAsync(id);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle error appropriately
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpDelete("Delete/{personId}")]
        public async Task<IActionResult> DeletePhysicalPerson(int personId)
        {
            try
            {
                await _service.DeletePersonAsync(personId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Delete/View")]
        public IActionResult Delete()
        {
            return View();
        }




        [HttpDelete("{id}/Relation/{toId}")]
        public async Task<IActionResult> DeleteRelation(int id, int toId)
        {
            await _service.DeleteRelationAsync(id, toId);
            return NoContent();
        }

  

        // PUT: PhysicalPerson/{id} - API-სთვის (Swagger)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhysicalPerson(int id, [FromBody] UpdatePersonCommandDto updatedPerson)
        {
            if (updatedPerson == null)
            {
                return BadRequest("Invalid person data.");
            }

            var result = await _service.UpdatePersonAsync(id, updatedPerson);

            if (!result)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            return Ok($"Person with ID {id} updated successfully.");
        }


        // GET method
        [HttpGet("Edit/{id}")]
       
        public async Task<IActionResult> Edit(int id)
        {
            var person = await _service.GetPhysicalPersonAsync(id);
            if (person == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            // Get the available RelationTypes
            var relationTypes = Enum.GetValues(typeof(RelationType))
                                     .Cast<RelationType>()
                                     .Select(rt => new { Value = rt.ToString(), Text = rt.ToString() })
                                     .ToList();

            // Map PhysicalPerson to UpdatePersonCommandDto
            var updatePersonDto = new UpdatePersonCommandDto
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                BirthDate = person.BirthDate,
                Gender = person.Gender,
                PersonalId = person.PersonalId,
                City = new UpdateCityDbo { Name = person.City?.Name },
                PhoneNumbers = person.PhoneNumbers.Select(p => new UpdatePhoneNumberDto
                {
                    Number = p.Number,
                    Type = p.Type
                }).ToList(),
                ImagePath = person.ImagePath,
                RelatedPersons = person.RelatedTo.Select(r => new RelatedPersonDto
                {
                    FirstName = r.RelatedTo.FirstName,
                    LastName = r.RelatedTo.LastName,
                    Relationship = r.RelationType.ToString()
                }).ToList()
            };

            // Add the relation types to the model
            ViewBag.RelationTypes = relationTypes;

            return View(updatePersonDto);
        }

        
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, UpdatePersonCommandDto updatedPerson, IFormFile? photo)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedPerson);
            }

            var personExists = await _service.GetPhysicalPersonAsync(id);
            if (personExists == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            // თუ ფოტო აიტვირთა, შევინახოთ
            if (photo != null && photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // თუ დირექტორია არ არსებობს, შევქმნათ
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, photo.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                updatedPerson.ImagePath = "/uploads/" + photo.FileName;
            }
            else
            {
                // თუ ფოტო არ აიტვირთა, ძველი ImagePath დავტოვოთ
                updatedPerson.ImagePath = personExists.ImagePath;
            }

            var result = await _service.UpdatePersonAsync(id, updatedPerson);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to update person.");
                return View(updatedPerson);
            }

            return RedirectToAction("Details", new { id = id });
        }


        [HttpPost]
        [Route("ProfilePicture")]
        public async Task<IActionResult> AddProfilePicture(int personId, IFormFile image, [FromServices] IFileService fileService)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            var createdImage = await fileService.SaveFileAsync(image);
            await _service.SetProfilePictureAsync(personId, createdImage);

            // გადამისამართება PhysicalPerson/Details-ზე შესაბამისი ID-თი
            return RedirectToAction("Details", "PhysicalPerson", new { id = personId });
        }


        [HttpGet]
        [Route("{personId}/ProfilePicture")]
        public async Task<IActionResult> GetProfilePicture(int personId, [FromServices] IFileService fileService)
        {
            var imagePath = await _service.GetProfilePictureAsync(personId);
            var image = await fileService.GetFileAsync(imagePath);
            return File(image, "image/jpeg");
        }


        [HttpGet]
        public IActionResult AddImg()
        {
            return View();
        }



        [HttpDelete]
        [Route("{personId}/ProfilePicture")]
        public async Task<IActionResult> DeleteProfilePicture(int personId, [FromServices] IFileService fileService)
        {
            await _service.DeleteProfilePictureAsync(personId);

            return Ok();
        }

   


        [HttpGet("Quicksearch")]
        public async Task<IActionResult> SearchPhysicalPersons([FromQuery] string searchTerm)
        {
            var persons = await _service.QuickSearchPhysicalPersonsAsync(searchTerm);

            if (persons == null || !persons.Any())
            {
                return NotFound();
            }

            return Ok(persons);
        }

        [HttpGet("PhysicalPerson/Filter")]
        public IActionResult Filter()
        {
            return View();
        }



        [HttpGet("Search")]
        public async Task<IActionResult> SearchPhysicalPersonsAsync([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (persons, totalCount) = await _service.SearchPhysicalPersonsAsync(searchTerm, pageNumber, pageSize);

            if (!persons.Any())
            {
                return NotFound("No matching persons found.");
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                WriteIndented = true
            };

            return Ok(new { persons, totalCount });
        }




    }
}

