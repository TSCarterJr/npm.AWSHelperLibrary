using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Orion.Common.Helpers.AWS
{
    internal static class AWSEC2Helper
    {
        private static IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());


        /// <summary>
        /// Get's the instance ID of the EC2 server the code is running on.
        /// </summary>
        /// <returns></returns>
        public static string GetEc2InstanceId()
        {
            var isInCache = MemoryCache.TryGetValue("ServerInstanceId", out string result);
            if (!isInCache)
            {
                var response = CacheOptions().Result;
                return response;
            }

            if (result.Contains("UNKNOWN"))
                return CacheOptions().Result; //Should handle instances where the AWS API returned back an unknown.
            return result;
        }

        /// <summary>
        /// Get's the instance region of the EC2 server the code is running on.
        /// </summary>
        /// <returns></returns>
        public static Amazon.RegionEndpoint GetEc2InstanceRegion()
        {
            try
            {
                var isInCache = MemoryCache.TryGetValue("ServerInstanceRegion", out string result);
                if (!isInCache)
                {
                    var response = CacheRegionOptions().Result;
                    return Amazon.RegionEndpoint.GetBySystemName(response);
                }
                else
                {
                    if (result.Contains("UNKNOWN"))
                        return Amazon.RegionEndpoint.GetBySystemName(CacheRegionOptions().Result);
                    return Amazon.RegionEndpoint.GetBySystemName(result);
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static async Task<string> CacheOptions()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //Windows
                MemoryCache.Set("ServerInstanceId", "");
                return ("");
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                //Linux
                var serverHostName = "";
                HttpClient client = new HttpClient();

                var tokenMessage = new HttpRequestMessage(HttpMethod.Put, "http://169.254.169.254/latest/api/token");
                tokenMessage.Headers.Add("X-aws-ec2-metadata-token-ttl-seconds", "21600");
                var tokenRes = await client.SendAsync(tokenMessage);
                string token = await tokenRes.Content.ReadAsStringAsync();

                var message = new HttpRequestMessage(HttpMethod.Get, "http://169.254.169.254/latest/meta-data/instance-id");
                message.Headers.Add("X-aws-ec2-metadata-token", token);
                var instanceNameRes = await client.SendAsync(message);
                serverHostName = await instanceNameRes.Content.ReadAsStringAsync();

                if (serverHostName.ToLower().Contains("html") || string.IsNullOrWhiteSpace(serverHostName)) serverHostName = "(UNKNOWN) ";

                MemoryCache.Set("ServerInstanceId", $"{serverHostName} ");
                return $"{serverHostName} ";
            }
        }
        private static async Task<string> CacheRegionOptions()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //Windows
                MemoryCache.Set("ServerInstanceRegion", "");
                return ("");
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                //Linux
                HttpClient client = new HttpClient();

                var tokenMessage = new HttpRequestMessage(HttpMethod.Put, "http://169.254.169.254/latest/api/token");
                tokenMessage.Headers.Add("X-aws-ec2-metadata-token-ttl-seconds", "21600");
                var tokenRes = await client.SendAsync(tokenMessage);
                string token = await tokenRes.Content.ReadAsStringAsync();

                var message = new HttpRequestMessage(HttpMethod.Get, "http://169.254.169.254/latest/meta-data/placement/region");
                message.Headers.Add("X-aws-ec2-metadata-token", token);
                var instanceNameRes = await client.SendAsync(message);
                var serverHostRegion = await instanceNameRes.Content.ReadAsStringAsync();

                if (serverHostRegion.ToLower().Contains("html") || string.IsNullOrWhiteSpace(serverHostRegion)) serverHostRegion = "(UNKNOWN) ";

                MemoryCache.Set("ServerInstanceRegion", serverHostRegion);
                return serverHostRegion;
            }
        }
    }
}
