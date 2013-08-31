using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mono.Api.AmazonClient.Model
{
    [DataContract]
    public class Item
    {
        [DataMember]
        public string ASIN { get; set; }
        [DataMember]
        public string DetailPageURL { get; set; }
        [DataMember]
        public IEnumerable<ItemLink> ItemLinks { get; set; }
        [DataMember]
        public string SmallImageURL { get; set; }
        [DataMember]
        public string MediumImageURL { get; set; }
        [DataMember]
        public string LargeImageURL { get; set; }
        [DataMember]
        public ItemAttributes Attributes { get; set; }
        [DataMember]
        public Ranking Ranking { get; set; }
        [DataMember]
        public OfferSummary OfferSummary { get; set; }
        [DataMember]
        public string EditorialReview { get; set; }

        public string Title { get { return Attributes.Title; } }
        public string Price
        {
            get
            {
                var price = OfferSummary.LowestNewPrice.FormattedPrice;
                price = price ?? OfferSummary.LowestUsedPrice.FormattedPrice;
                price = price ?? OfferSummary.LowestCollectiblePrice.FormattedPrice;
                price = price ?? CatalogPrice;
                return price;
            }
        }
        public string CatalogPrice { get { return Attributes.FormattedPrice; } }
        public string FormattedPrice
        {
            get
            {
                var p = Price;
                p = p ?? CatalogPrice;
                if (p == null)
                {
                    return "-";
                }
                if (Price == null)
                {
                    return CatalogPrice;
                }
                if (CatalogPrice == null)
                {
                    return Price;
                }
                var ratio = 0f;
                try
                {
                    ratio = ((Attributes.Amount - OfferSummary.LowestNewPrice.Amount) / (float)Attributes.Amount) * 100;
                }
                catch (Exception)
                {
                }
                if (ratio <= 0f)
                {
                    return Price;
                }
                return string.Format("{0} (Off: {1:0.}%)", Price, ratio);
            }
        }
        public string Image { get { return LargeImageURL; } }
        
    }
}
