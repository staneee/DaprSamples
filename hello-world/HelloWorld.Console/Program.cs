using HelloWorld.Dtos;

using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Threading;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderServiceUrl = "http://localhost:3500/v1.0/invoke/orderservice";
            var createApi = $"{orderServiceUrl}/method/order/create";

            var httpClient = new HttpClient();

            var count = 1;
            while (true)
            {
                System.Console.WriteLine("第 {0} 次创建订单", count);
                try
                {
                    var jsonStr = JsonConvert.SerializeObject(new NewOrderInput
                    {
                        Data = new OrderDto
                        {
                            OrderId = count++.ToString()
                        }
                    });
                    var stringContent = new StringContent(jsonStr, System.Text.Encoding.UTF8, "application/json");

                    var response = httpClient.PostAsync(createApi,stringContent).GetAwaiter().GetResult();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        System.Console.WriteLine(
                            "StatusCode: {0} \r\nMessage: {1}",
                            response.StatusCode,
                            response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                            );
                    }
                    else
                    {
                        System.Console.WriteLine(
                            response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                            );
                    }
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
