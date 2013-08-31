using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mono.Api.AmazonClient.Model
{
    [DataContract]
    public class Price
    {
        [DataMember]
        public int Amount { get; set; }
        [DataMember]
        public string CurrencyCode { get; set; }
        [DataMember]
        public string FormattedPrice { get; set; }
    }
}
