using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono.Api.AmazonClient.Model
{
    public class ItemsResult
    {
        public IEnumerable<Item> Items { get; set; }
        public LoadedResultType Result { get; set; }
    }

    public enum LoadedResultType
    {
        Cached,
        Downloaded,
        Failed,
    }
}
