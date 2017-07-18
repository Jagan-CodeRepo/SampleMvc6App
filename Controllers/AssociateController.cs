using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleMvc6App.Models;

namespace SampleMvc6App.Controllers
{
    [Route("api/[controller]")]
    public class AssociateController : Controller
    {
        [HttpGet]
        public IEnumerable<Associate> Get()
        {
            var employees = new List<Associate>
            {
            new Associate 
            {
                ID = 1,
                Name = "Jagan",
                Designation = "A"
            },
            new Associate 
            {
                ID = 2,
                Name = "Madhava",
                Designation = "PA"
            },
            new Associate 
            {
                ID = 3,
                Name = "Varrun",
                Designation = "A"
            },
            new Associate 
            {
                ID = 4,
                Name = "Nivetha",
                Designation = "PA"
            }
            };
        return employees;
        }
    }
}
