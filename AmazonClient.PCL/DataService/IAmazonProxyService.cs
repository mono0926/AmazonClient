using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Api.AmazonClient.AmazonProxyService;

namespace Mono.Api.AmazonClient.DataService
{
    public interface IAmazonProxyService
    {
        Task<string> ItemSearchAsync(CountryType countryType, SearchIndexType indexType, int itemPage);
        Task<IEnumerable<SearchIndexType>> AvailableTypesAsync(CountryType countryType);
        Task<IEnumerable<CountryType>> AvailableCountriesAsync();
    }
}
