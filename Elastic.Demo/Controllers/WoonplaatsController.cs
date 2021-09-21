using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Demo.Models;
using Nest;


namespace Elastic.Demo.Controllers
{
    [Route("api/woonplaats")]
    [ApiController]
    public class WoonplaatsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public WoonplaatsController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Get the count of listings per woonplaats
        /// </summary>
        /// <returns></returns>
        [HttpGet("top-woonplaats")]
        public async Task<IActionResult> GetCountOfListingsPerWoonplats()
        {
            var response = _elasticClient.Search<Listing>(s => s
                .Size(0)
                .Aggregations(a => a
                    .Terms("listings_per_woonplaats", c => c
                        .Field(f => f.Woonplaats.Suffix("keyword"))
                    )
                )
            );

            if (response.IsValid)
            {
                var buckets = response.Aggregations.Terms("listings_per_woonplaats").Buckets.ToList();
                var relevantInfoBukcets = new List<KeyValuePair<string, long?>>(){};

                foreach (var bucket in buckets)
                {
                    relevantInfoBukcets.Add(new KeyValuePair<string, long?>(bucket.Key, bucket.DocCount));
                }

                return Ok(relevantInfoBukcets);
            }

            return BadRequest();
        }
    }
}

