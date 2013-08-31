using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mono.Api.AmazonClient.Model
{
    [DataContract]
    public class ItemAttributes
    {

        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Format { get; set; }
        [DataMember]
        public string Binding { get; set; }
        [DataMember]
        public bool IsAdultProduct { get; set; }
        [DataMember]
        public string ISBN { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public int Amount { get; set; }
        [DataMember]
        public string FormattedPrice { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public int NumberOfPages { get; set; }
        [DataMember]
        public DateTime PublicationDate { get; set; }
        [DataMember]
        public DateTime ReleaseDate { get; set; }
        [DataMember]
        public string Publisher { get; set; }
        [DataMember]
        public string Title { get; set; }
    }
}
