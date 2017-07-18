using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleMvc6App.Models;

namespace SampleMvc6App.Controllers
{
    [Route("api/persons")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = -1)]
    public class PersonsController : Controller
    {
        [HttpGet]
        public IEnumerable<Person> GetPersons()
        {
            return new List<Person>
            {
                new Person{Name = "Max Musterman", City="Naustadt", Dob=new DateTime(1978, 07, 29)},
                new Person{Name = "Maria Musterfrau", City="London", Dob=new DateTime(1979, 08, 30)},
                new Person{Name = "John Doe", City="Los Angeles", Dob=new DateTime(1980, 09, 01)}
            };
        }
    }
}
