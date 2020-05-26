using Centric.Learning.Smoelenboek.Business;
using System.Collections.Generic;

namespace Centric.Learning.Smoelenboek.Models
{
    public class SearchViewModel
    {
        public string SearchText { get; set; }
        public List<Person> People { get; set; }
    }
}