using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Api.AmazonClient.AmazonProxyService;
using Mono.Api.AmazonClient.Model;

namespace Mono.Api.AmazonClient.DataService
{
    public interface IAmazonDataService
    {
        Task<IEnumerable<SearchIndexType>> AvailableTypesAsync();
        Task<IEnumerable<CountryType>> AvailableCountriesAsync();
        Task<Ranking> GetRanking(SearchIndexType indexType);
        Task<ItemsResult> GetMoreItems(SearchIndexType indexType, int itemPage);
        CountryType CountryType { get; set; }
    }
}