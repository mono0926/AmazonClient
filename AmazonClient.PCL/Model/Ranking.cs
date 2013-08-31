using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Mono.Api.AmazonClient.AmazonProxyService;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Mono.Api.AmazonClient.DataService;
using Mono.Framework.Common.Extensios;

namespace Mono.Api.AmazonClient.Model
{
    [DataContract]
    public class Ranking
    {
        readonly AmazonDataService _dataService;
        bool _loadMore;
        public Ranking(AmazonDataService dataService)
        {
            _dataService = dataService;
            AllItems = new ObservableCollection<Item>();
            AllItemsWithoutFirst = new ObservableCollection<Item>();
        }
        public int Page { get; private set; }

        private IEnumerable<Item> _items;

        public IEnumerable<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                AllItems.Clear();
                AllItemsWithoutFirst.Clear();
                Page = 1;
                var itemArray = _items.ToArray();
                itemArray.ForEach(x => AllItems.Add(x));
                itemArray.Skip(1).ForEach(x => AllItemsWithoutFirst.Add(x));
            }
        }

        private async Task<bool> LoadMoreItems()
        {
            Page += 1;
            var result = await _dataService.GetMoreItems(IndexType, Page);
            if (result.Result == LoadedResultType.Failed)
            {
                return false;
            }
            var items = result.Items;
            items.ForEach(x => 
                {
                    x.Ranking = this;
                    AllItems.Add(x);
                });
            items.ForEach(x =>
                {
                    x.Ranking = this;
                    AllItemsWithoutFirst.Add(x);
                });
            if (Page == 10)
            {
                return false;
            }
            return true;
        }

        object loadLock = new object();
        public async Task LoadAllItems()
        {
            lock (loadLock)
            {
                if (_loadMore)
                {
                    return;
                }
                _loadMore = true;
            }
            if (Page == 10)
            {
                return;
            }
            while (true)
            {
                var r = await LoadMoreItems();
                if (!r)
                {
                    _loadMore = false;
                    return;
                }
            }
        }

        public ObservableCollection<Item> AllItems { get; set; }
        public ObservableCollection<Item> AllItemsWithoutFirst { get; set; }


        [DataMember]
        public SearchIndexType IndexType { get; set; }

        public string Title { get { return IndexType.ToString(); } }

        public bool Cached { get; set; }

        public string Image { get { return Items == null || Items.Any() ? Items.First().Image : null; } }
        public string MediumImageURL { get { return Items == null || Items.Any() ? Items.First().MediumImageURL : null; } }
    }
}
