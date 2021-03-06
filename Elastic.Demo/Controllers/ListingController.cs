using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Elastic.Demo.Models;
using Nest;


namespace Elastic.Demo.Controllers
{
    [Route("api/listing")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public ListingController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Get listing by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = _elasticClient.Get<Listing>(id);
            if (response.Found)
            {
                return Ok(response.Source);

            }
            return NotFound();
        }
        
        /// <summary>
        /// Index a listing
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Listing listing)
        {
            var response = await _elasticClient.IndexDocumentAsync(listing);
            if (!response.IsValid)
            {
                return BadRequest();
            }

            return Ok($"Document was created and its id is: {response.Id}");
        }

        /// <summary>
        /// Update listing by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="listing"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Listing listing)
        {

          await _elasticClient
                .UpdateAsync<Listing>(id, u => 
                    u.Index("listings").Doc(listing));


            return Ok($"Listing with id - {id} was successfully updated");
        }

        /// <summary>
        /// Delete listing by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var response = _elasticClient.Delete<Listing>(id);
            if (response.IsValid)
            {
                return Ok($"Listing with id - {id} was succesfully deleted");

            }
            return NotFound();
        }

        /// <summary>
        /// Get object by globalId
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        [HttpGet("globalId/{globalId}")]
        public async Task<IActionResult> GetByGlobalId(long globalId)
        {
            var response = _elasticClient.Search<Listing>(s => s
                .Query(q => q
                    .Match(mq =>
                        mq.Field(f => f.GlobalId).Query(globalId.ToString()))
                )
            );

            if (response.IsValid)
            {
                return Ok(response.Documents.First());

            }
            return NotFound();
        }
    }
}

