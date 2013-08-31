using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mono.Api.AmazonClient.Model
{
    [DataContract]
    public class OfferSummary
    {
        [DataMember]
        public Price LowestNewPrice { get; set; }
        [DataMember]
        public Price LowestUsedPrice { get; set; }
        [DataMember]
        public Price LowestCollectiblePrice { get; set; }
        [DataMember]
        public int TotalNew { get; set; }
        [DataMember]
        public int TotalUsed { get; set; }
        [DataMember]
        public int TotalCollectible { get; set; }
        [DataMember]
        public int TotalRefurbished { get; set; }
    }
}
