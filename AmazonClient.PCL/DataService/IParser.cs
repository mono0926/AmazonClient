using Mono.Api.AmazonClient.Model;

namespace Mono.Api.AmazonClient.DataService
{
   public interface IParser
    {
        Item[] Parse(string response);
    }
}