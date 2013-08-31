using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Api.AmazonClient.AmazonProxyService;

namespace Mono.Api.AmazonClient.DataService
{
    class AmazonProxyService : IAmazonProxyService
    {
        private readonly IAmazonService _client;

        public AmazonProxyService(IAmazonService client)
        {
            _client = client;
        }

        public async Task<string> ItemSearchAsync(CountryType countryType, SearchIndexType indexType, int itemPage)
        {
            return await Task<string>.Factory.FromAsync(_client.BeginItemSearch,
                                                        _client.EndItemSearch,
                                                        countryType, indexType, itemPage, null);
        }

        public async Task<IEnumerable<SearchIndexType>> AvailableTypesAsync(CountryType countryType)
        {
            return await Task<IEnumerable<SearchIndexType>>.Factory.FromAsync(_client.BeginAvailableTypes,
                                                           _client.EndAvailableTypes,
                                                           countryType, null);
        }

        public async Task<IEnumerable<CountryType>> AvailableCountriesAsync()
        {
            return await Task<IEnumerable<CountryType>>.Factory.FromAsync(_client.BeginAvailableCountries,
                                                           _client.EndAvailableCountries, null);
        }
    }
}
