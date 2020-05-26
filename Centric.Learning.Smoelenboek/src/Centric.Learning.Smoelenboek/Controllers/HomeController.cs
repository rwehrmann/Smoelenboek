using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Centric.Learning.Smoelenboek.Models;
using Centric.Learning.Smoelenboek.Storage;
using Microsoft.AspNetCore.Hosting;
using Centric.Learning.Smoelenboek.Business.Storage;
using Centric.Learning.Smoelenboek.Business;
using Microsoft.AspNetCore.Http;

namespace Centric.Learning.Smoelenboek.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhotoRepository _photoRepository;
        private readonly IPeopleRepository _peopleRepository;

        public HomeController(ILogger<HomeController> logger, IPeopleRepository peopleRepository, IPhotoRepository photoRepository)
        {
            _logger = logger;
            _photoRepository = photoRepository;
            _peopleRepository = peopleRepository;
        }

        public async Task<IActionResult> Index()
        {
            // initialize session search count
            if (!HttpContext.Session.Keys.Contains("SearchCount"))
            {
                HttpContext.Session.SetInt32("SearchCount", 0);
            }

            var searchModel = new SearchViewModel
            {
                SearchText = "",
                People = await GetPeople()
            };

            ViewBag.SearchCount = HttpContext.Session.GetInt32("SearchCount").Value;
            return View(searchModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(SearchViewModel model)
        {

            var searchModel = new SearchViewModel
            {
                SearchText = model.SearchText,
                People = await GetPeople(model.SearchText)
            };

            // Increase session search count
            HttpContext.Session.SetInt32("SearchCount", HttpContext.Session.GetInt32("SearchCount").HasValue ? HttpContext.Session.GetInt32("SearchCount").Value + 1: 1);

            ViewBag.SearchCount = HttpContext.Session.GetInt32("SearchCount").Value;
            return View("Index", searchModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<Person>> GetPeople(string search = null)
        {
            List<Person> people = await _peopleRepository.GetPeopleAsync(search);
            var peopleList = people.OrderBy(p => p.Name).ToList();

            return peopleList;
        }

        public async Task<FileResult> Photos(int id)
        {
            Person person = await _peopleRepository.GetPersonAsync(id);
            var content = await _photoRepository.GetPhotoAsync(person.Image);
            return new FileContentResult(content, "image/jpg");
        }

        public async Task<IActionResult> Initialize()
        {
            try
            {
                await Initializer.Initialise(_peopleRepository, _photoRepository);
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

    }
}
