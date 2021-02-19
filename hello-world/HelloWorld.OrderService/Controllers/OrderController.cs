using HelloWorld.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Dapr.Client;

namespace HelloWorld.Controllers
{
    [Route("[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// dapr sidecar http 地址
        /// </summary>
        const string DAPR_SIDECAR_HTTP = "http://localhost:3500";

        /// <summary>
        /// statestore name
        /// </summary>
        const string STATE_STORE_NAME = "statestore";

        /// <summary>
        /// dapr state 存储键值
        /// </summary>
        const string STATE_KEY = "order";

        readonly ILogger<OrderController> _logger;
        readonly IHttpClientFactory _httpClientFactory;

        public OrderController(ILogger<OrderController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        /// <summary>
        /// 新建订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Create([FromBody] NewOrderInput input)
        {
            using var client = new DaprClientBuilder()
                .UseHttpEndpoint(DAPR_SIDECAR_HTTP)
                .Build();

            try
            {
                await client.SaveStateAsync(STATE_STORE_NAME, STATE_KEY, input.Data);

                return "保存订单成功";
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "保存订单出错!");
                return ex.Message;
            }
        }


        /// <summary>
        /// 获取订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get()
        {
            using var client = new DaprClientBuilder()
                .UseHttpEndpoint(DAPR_SIDECAR_HTTP)
                .Build();

            try
            {
                var res = await client.GetStateAsync<OrderDto>(STATE_STORE_NAME, STATE_KEY);
                if (res == null)
                {
                    return "获取订单失败!";
                }

                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "获取订单出现错误!");
                return ex.Message;
            }
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<string> Delete()
        {
            using var client = new DaprClientBuilder()
                .UseHttpEndpoint(DAPR_SIDECAR_HTTP)
                .Build();

            try
            {
                await client.DeleteStateAsync(STATE_STORE_NAME, STATE_KEY);
                return "删除订单成功!";
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "删除订单出现错误!");
                return ex.Message;
            }
        }
    }
}
