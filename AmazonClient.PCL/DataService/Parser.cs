using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mono.Api.AmazonClient.Model;

namespace Mono.Api.AmazonClient.DataService
{
    class Parser : IParser
    {
        public static readonly XNamespace Namespace;
        static Parser()
        {
            Parser.Namespace = "http://webservices.amazon.com/AWSECommerceService/2011-08-01";
        }

        public Item[] Parse(string response)
        {
            var doc = XDocument.Parse(response);
            var items = doc.Descendants(Namespace + "Item")
                .Select(x => new Item
                {
                    ASIN = x.GetValue("ASIN"),
                    DetailPageURL = x.GetValue("DetailPageURL"),
                    ItemLinks = x.Descendants(Namespace + "ItemLink")
                                    .Select(y => new ItemLink { URL = y.GetValue("URL"), Description = y.GetValue("Description") }),
                    SmallImageURL = x.GetAttributesValue("SmallImage", "URL"),
                    MediumImageURL = x.GetAttributesValue("MediumImage", "URL"),
                    LargeImageURL = x.GetAttributesValue("LargeImage", "URL"),
                    Attributes = new ItemAttributes
                    {
                        Author = x.GetAttributesValue("ItemAttributes", "Author"),
                        Format = x.GetAttributesValue("ItemAttributes", "Format"),
                        Binding = x.GetAttributesValue("ItemAttributes", "Binding"),
                        IsAdultProduct = x.GetAttributesValue<bool>("ItemAttributes", "IsAdultProduct"),
                        ISBN = x.GetAttributesValue("ItemAttributes", "ISBN"),
                        Label = x.GetAttributesValue("ItemAttributes", "Label"),
                        Amount = x.GetListPrice<int>("Amount"),
                        FormattedPrice = x.GetListPrice<string>("FormattedPrice"),
                        Manufacturer = x.GetAttributesValue("ItemAttributes", "Manufacturer"),
                        NumberOfPages = x.GetAttributesValue<int>("ItemAttributes", "NumberOfPages"),
                        PublicationDate = x.GetAttributesValue<DateTime>("ItemAttributes", "PublicationDate"),
                        ReleaseDate = x.GetAttributesValue<DateTime>("ItemAttributes", "ReleaseDate"),
                        Publisher = x.GetAttributesValue("ItemAttributes", "Publisher"),
                        Title = x.GetAttributesValue("ItemAttributes", "Title"),
                    },
                    OfferSummary = new OfferSummary
                    {
                        TotalNew = x.GetAttributesValue<int>("OfferSummary", "TotalNew"),
                        TotalUsed = x.GetAttributesValue<int>("OfferSummary", "TotalUsed"),
                        TotalCollectible = x.GetAttributesValue<int>("OfferSummary", "TotalCollectible"),
                        TotalRefurbished = x.GetAttributesValue<int>("OfferSummary", "TotalRefurbished"),
                        LowestNewPrice = new Price
                        {
                            Amount = x.GetPrice<int>("LowestNewPrice", "Amount"),
                            CurrencyCode = x.GetPrice<string>("LowestNewPrice", "CurrencyCode"),
                            FormattedPrice = x.GetPrice<string>("LowestNewPrice", "FormattedPrice"),
                        },
                        LowestUsedPrice = new Price
                        {
                            Amount = x.GetPrice<int>("LowestUsedPrice", "Amount"),
                            CurrencyCode = x.GetPrice<string>("LowestUsedPrice", "CurrencyCode"),
                            FormattedPrice = x.GetPrice<string>("LowestUsedPrice", "FormattedPrice"),
                        },
                        LowestCollectiblePrice = new Price
                        {
                            Amount = x.GetPrice<int>("LowestCollectiblePrice", "Amount"),
                            CurrencyCode = x.GetPrice<string>("LowestCollectiblePrice", "CurrencyCode"),
                            FormattedPrice = x.GetPrice<string>("LowestCollectiblePrice", "FormattedPrice"),
                        },
                    },
                    EditorialReview = x.GetEditorialReview(),
                }).ToArray();
            return items;
        }
    }
    static class Extensions
    {
        public static string GetEditorialReview(this XElement element)
        {
            var e = element.Descendants(Parser.Namespace + "EditorialReview")
                .Select(x => x.Element(Parser.Namespace + "Content").GetValue());
            return string.Join(Environment.NewLine, e);
        }

        public static T GetPrice<T>(this XElement element, string kind, string v)
        {
            var p = element.Element(Parser.Namespace + "OfferSummary");
            if (p == null)
            {
                return default(T);
            }
            return p.GetAttributesValue<T>(kind, v);
        }

        public static T GetAttributesValue<T>(this XElement element, string parentElem, string name)
        {
            try
            {

                var p = element.Element(Parser.Namespace + parentElem);
                if (p == null)
                {
                    return default(T);
                }
                var r = GetValue<T>(p.Element(Parser.Namespace + name));
                return r;
            }
            catch (Exception)
            {
            }
            return default(T);
        }
        public static T GetListPrice<T>(this XElement element, string name)
        {
            var attr = element.Element(Parser.Namespace + "ItemAttributes").Element(Parser.Namespace + "ListPrice");
            if (attr == null)
            {
                return default(T);
            }
            var r = GetValue<T>(attr.Element(Parser.Namespace + name));
            return r;
        }
        public static string GetAttributesValue(this XElement element, string parentElem, string name)
        {
            return element.GetAttributesValue<string>(parentElem, name);
        }

        public static T GetValue<T>(this XElement element)
        {
            if (typeof(T) == typeof(bool) && element != null && (element.Value == "0" || element.Value == "1"))
            {
                element.Value = element.Value == "0" ? "false" : "true";
            }
            return element == null ? default(T) : (T)Convert.ChangeType(element.Value, typeof(T));
        }
        public static string GetValue(this XElement element, string elementName)
        {
            return element.GetValue<string>(elementName);
        }

        public static T GetValue<T>(this XElement element, string elementName)
        {
            if (element == null)
            {
                return default(T);
            }
            var e = element.Element(Parser.Namespace + elementName);
            return e == null ? default(T) : (T)Convert.ChangeType(e.Value, typeof(T));
        }

        private static string GetValue(this XElement element)
        {
            return GetValue<string>(element);
        }
    }
}
