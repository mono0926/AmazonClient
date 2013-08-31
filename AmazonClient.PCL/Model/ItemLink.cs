using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mono.Api.AmazonClient.Model
{
    [DataContract]
    public class ItemLink
    {
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string URL { get; set; }
    }
}
