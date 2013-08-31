using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Api.AmazonClient.AmazonProxyService;
using Mono.Api.AmazonClient.DataService;
using Mono.Api.AmazonClient.Model;
using Mono.Framework.Common.IO;
using Moq;

namespace AmazonClient.Test.DataService
{
    [TestClass]
    public class AmazonDataServiceTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public async Task GetAvailableTypes()
        {

            var mockProxyService = new Mock<IAmazonProxyService>();
            IEnumerable<SearchIndexType> fakeTypes = new SearchIndexType[]
                {
                    SearchIndexType.Apparel, SearchIndexType.Appliances, SearchIndexType.ArtsAndCrafts,
                };
            mockProxyService.Setup(x => x.AvailableTypesAsync(CountryType.Japan))
                            .Returns(Task.Factory.StartNew(() => fakeTypes));

            var service = new AmazonDataService(CountryType.Japan,
                                                mockProxyService.Object, null, null, null);

            var types = await service.AvailableTypesAsync();
            Assert.IsNotNull(types);
            Assert.AreEqual(3, types.Count());
        }

        // 初回起動時、ネットワークでランキング取得
        [TestMethod]
        public async Task GetRanking()
        {
            var mockProsyService = new Mock<IAmazonProxyService>();
            mockProsyService.Setup(x => x.ItemSearchAsync(CountryType.Japan, SearchIndexType.Apparel, 1))
                            .Returns(Task.Factory.StartNew(() => "success_response"));
            var mockSerializer = new Mock<ISerializer>();
            //mockSerializer.Setup(x => x.SaveXml("", It.IsAny<XElement>(), false));
            //mockSerializer.Setup(x => x.LoadXml("", false));

            var mockSettingStore = new Mock<ISettingStore>();
            bool cached = false;
            mockSettingStore.Setup(x => x.Load<DateTime>("Japan:Apparel", out cached, false))
                            .Returns(new DateTime());

            var mockParser = new Mock<IParser>();
            mockParser.Setup(x => x.Parse("success_response"))
                      .Returns(new Item[]
                          {
                              new Item()
                                  {
                                      Attributes = new ItemAttributes() {IsAdultProduct = false}
                                  },
                              new Item()
                                  {
                                      Attributes = new ItemAttributes() {IsAdultProduct = false}
                                  },
                              new Item()
                                  {
                                      Attributes = new ItemAttributes() {IsAdultProduct = true}
                                  },
                          });


            var service = new AmazonDataService(CountryType.Japan,
                                                mockProsyService.Object, mockParser.Object, mockSettingStore.Object,
                                                mockSerializer.Object);
            var ranking = await service.GetRanking(SearchIndexType.Apparel);


            Assert.IsNotNull(ranking);
            Assert.IsNotNull(ranking.Items);
            Assert.AreEqual(2, ranking.Items.Count());
            mockSerializer.VerifyAll();
            mockSettingStore.VerifyAll();
        }

    }
}
