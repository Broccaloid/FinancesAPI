using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FinancialOperationsController : ControllerBase
    {
        public IUnitOfWork UnitOfWork { get; }

        public FinancialOperationsController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        // GET: api/<FinancialOperationsController>
        [HttpGet]
        public IEnumerable<FinancialOperation> Get()
        {
            //return new List<FinancialOperation> { new FinancialOperation { BalanceChange = -1000, Date = new DateTime(), Id = 0, Type = "LOL"} };
            return UnitOfWork.FinancialOperations.GetAll();
        }

        // GET api/<FinancialOperationsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FinancialOperationsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FinancialOperationsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FinancialOperationsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
