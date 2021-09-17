using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Elastic.Demo.Models;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Elastic.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public ListingsController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Get listing by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Listing> GetById(string id)
        {
            var response = _elasticClient.Get<Listing>(id);
            return response.Source;
        }

        /// <summary>
        /// Index a listing
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        [HttpPost]
        public IndexResponse Post([FromBody] Listing listing)
        {
            var response = _elasticClient.IndexDocumentAsync(listing);
            return response.Result;
        }

        /// <summary>
        /// Update listing by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="listing"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IInlineGet<Listing>> Put(int id, [FromBody] Listing listing)
        {
            var response = await _elasticClient
                .UpdateAsync<Listing>(id, u => u.Index("listings").Doc(listing));

            return response.Get;
        }

        /// <summary>
        /// Delete listing by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public Result Delete(int id)
        {
            var response = _elasticClient.Delete<Listing>(id);
            return response.Result;
        }

        /// <summary>
        /// Get object by globalId
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        [HttpGet("{globalId}")]
        public async Task<IReadOnlyCollection<Listing>> GetByGlobalId(string globalId)
        {
            var response = _elasticClient.Search<Listing>(s => s
                .Query(q => q
                    .Match(mq =>
                        mq.Field(f => f.GlobalId).Query(globalId))
                )
            );
            return response.Documents;
        }

    }
}

