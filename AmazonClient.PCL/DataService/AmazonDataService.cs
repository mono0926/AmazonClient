using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Api.AmazonClient.AmazonProxyService;
using Mono.Api.AmazonClient.Model;
using Mono.Framework.Common.Extensios;
using Mono.Framework.Common.IO;

namespace Mono.Api.AmazonClient.DataService
{
    // TODO: test
    public class AmazonDataService : IAmazonDataService
    {
        private readonly IAmazonProxyService _service;
        private readonly IParser _parser;
        private readonly ISettingStore _settingStore;
        private readonly ISerializer _serializer;

        

        public AmazonDataService(CountryType countryType, IAmazonProxyService service, IParser parser, ISettingStore settingStore, ISerializer serializer)
        {
            _service = service;
            CountryType = countryType;
            _parser = parser;
            _settingStore = settingStore;
            _serializer = serializer;
        }

        public AmazonDataService(CountryType countryType, ISettingStore settingStore, ISerializer serializer)
            : this(countryType, new AmazonProxyService(new AmazonServiceClient()), new Parser(), settingStore, serializer)
        {
        }

        public async Task<IEnumerable<SearchIndexType>> AvailableTypesAsync()
        {
            return await _service.AvailableTypesAsync(CountryType);
        }
        public async Task<IEnumerable<CountryType>> AvailableCountriesAsync()
        {
            return await _service.AvailableCountriesAsync();
        }

        public async Task<Ranking> GetRanking(SearchIndexType indexType)
        {
            bool cached = false;
            string response = null;
            var key = string.Format("{0}:{1}", CountryType, indexType);
            var filename = string.Format("{0}.xml", indexType);
            bool cache;
            var pre = _settingStore.Load<DateTime>(key, out cache);
            if (cache)
            {
                var diff = DateTime.Now - pre;
                if (diff.CompareTo(TimeSpan.FromMinutes(30)) < 0)
                {
                    response = await _serializer.ReadFile(CountryType.ToString(), filename);
                    cached = response != null;
                }
                else
                {
                    Enumerable.Range(2, 9).ForEach(async i =>
                        {
                            var deleteName = string.Format("{0}_{1}.xml", indexType, i);
                            await _serializer.DeleteFile(CountryType.ToString(), deleteName);
                        });
                }
            }

            if (response == null)
            {
                response = await _service.ItemSearchAsync(CountryType, indexType, 1);
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }
                _serializer.WriteFile(CountryType.ToString(), filename, response);
                _settingStore.Save(key, DateTime.Now.ToString());
            }

            var items = _parser.Parse(response).Where(x => !x.Attributes.IsAdultProduct);
            return new Ranking(this) { IndexType = indexType, Items = items, Cached = cached };
        }

        public async Task<ItemsResult> GetMoreItems(SearchIndexType indexType, int itemPage)
        {
            var filename = string.Format("{0}_{1}.xml", indexType, itemPage);
            var response = await _serializer.ReadFile(CountryType.ToString(), filename);
            var result = LoadedResultType.Cached;
            if (response == null)
            {
                result = LoadedResultType.Downloaded;
                response = await _service.ItemSearchAsync(CountryType, indexType, itemPage);
                if (string.IsNullOrEmpty(response))
                {
                    result = LoadedResultType.Failed;
                    return new ItemsResult { Result = result };
                }
                _serializer.WriteFile(CountryType.ToString(), filename, response);
            }
            return new ItemsResult
            {
                Items = _parser.Parse(response).Where(x => !x.Attributes.IsAdultProduct), 
                Result = result,
            };
        }


        public CountryType CountryType { get; set; }
    }

}
