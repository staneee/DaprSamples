using Dapr.Client;

using HelloWorld.Dtos;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;

namespace HelloWorld
{
    class Program
    {
        /// <summary>
        /// dapr sidecar http 地址
        /// </summary>
        const string DAPR_SIDECAR_HTTP = "http://localhost:3500";

        const string APP_ID = "orderservice";

        const string METHOD_NAME = "/order/create";

        static void Main(string[] args)
        {

            var client = DaprClient.CreateInvokeHttpClient(
                appId: APP_ID,
                daprEndpoint: DAPR_SIDECAR_HTTP
                );

            var count = 1;
            while (true)
            {
                System.Console.WriteLine("第 {0} 次创建订单", count);
                try
                {
                    var input = new NewOrderInput
                    {
                        Data = new OrderDto
                        {
                            OrderId = count++.ToString()
                        }
                    };
                    var res = client.PostAsJsonAsync(METHOD_NAME, input)
                        .GetAwaiter().GetResult();

                    Console.WriteLine(res.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(3000);
            }
        }
    }
}
