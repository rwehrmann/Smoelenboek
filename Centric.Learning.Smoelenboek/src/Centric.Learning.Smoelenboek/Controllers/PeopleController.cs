using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Centric.Learning.Smoelenboek.Models;
using Centric.Learning.Smoelenboek.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Centric.Learning.Smoelenboek.Business;
using Centric.Learning.Smoelenboek.Business.Storage;
using Microsoft.AspNetCore.Authorization;

namespace Centric.Learning.Smoelenboek.Controllers
{
    [Authorize]
    public class PeopleController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhotoRepository _photoRepository;
        private readonly IPeopleRepository _peopleRepository;

        public PeopleController(ILogger<HomeController> logger, IPeopleRepository peopleRepository, IPhotoRepository photoRepository)
        {
            _logger = logger;
            _photoRepository = photoRepository;
            _peopleRepository = peopleRepository;
        }

        // GET: People
        public async Task<ActionResult> Index()
        {
            var peopleList = await _peopleRepository.GetPeopleAsync();
            return View(peopleList);
        }

        // GET: People/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Person person = await _peopleRepository.GetPersonAsync(id.Value);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Person person, IFormFile imageContent)
        {
            if (ModelState.IsValid)
            {
                //Save image to disk
                person.Image = await SaveImage(person, imageContent);

                await _peopleRepository.AddPersonAsync(person);
                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: People/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Person person = await _peopleRepository.GetPersonAsync(id.Value);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Person person, IFormFile imageContent)
        {
            if (ModelState.IsValid)
            {
                //Save image to disk
                if (imageContent != null)
                {
                    person.Image = await SaveImage(person, imageContent);
                }
                await _peopleRepository.EditPersonAsync(person);
                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: People/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Person person = await _peopleRepository.GetPersonAsync(id.Value);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Person person = await _peopleRepository.GetPersonAsync(id);
            await _peopleRepository.DeletePersonAsync(person);
            await _photoRepository.DeletePhotoAsync(person.Image);

            return RedirectToAction("Index");
        }

        private async Task<string> SaveImage(Person person, IFormFile imageContent)
        {
            var imageName = person.Name.ToLower() + Path.GetExtension(imageContent.FileName);

            Stream readStream = imageContent.OpenReadStream();
            await _photoRepository.UploadPhotoAsync(imageName, readStream, imageContent.ContentType);

            return imageName;
        }


    }
}