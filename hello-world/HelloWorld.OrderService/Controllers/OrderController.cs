using HelloWorld.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Dapr.Actors;
using Dapr.Extensions;
using Dapr.AppCallback;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HelloWorld.Controllers
{
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        const string StateStoreName = "order";

        readonly ILogger<OrderController> _logger;
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _httpClientFactory;

        public OrderController(ILogger<OrderController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 新建订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> NewOrder([FromBody]NewOrderInput input)
        {
            try
            {
                // 读取配置获取 dapr runtime 的 http 路径
                var daprHttpService = _configuration["Dapr:RuntimeHttpService"];
                // 拼接 dapr state url
                var stateUrl = $"{daprHttpService}/state/statestore";


                // 创建http client
                var httpClient = _httpClientFactory.CreateClient("dapr_state");

                // 创建请求参数
                var states = new List<StateDto<NewOrderInput>>()
                {
                    new StateDto<NewOrderInput>(StateStoreName,input)
                };
                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };

                var requestDataString = JsonConvert.SerializeObject(states, setting);
                var stringContent = new StringContent(requestDataString, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(stateUrl, stringContent);
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    return "保存订单失败!";
                }


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
        public async Task<string> Query()
        {
            try
            {
                // 读取配置获取 dapr runtime 的 http 路径
                var daprHttpService = _configuration["Dapr:RuntimeHttpService"];
                // 拼接 dapr state url
                var stateUrl = $"{daprHttpService}/state/statestore/{StateStoreName}";


                // 创建http client
                var httpClient = _httpClientFactory.CreateClient("dapr_state");


                var response = await httpClient.GetAsync(stateUrl);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return "获取订单失败!";
                }
                return await response.Content.ReadAsStringAsync();
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
        public async Task<string> Remove()
        {
            try
            {
                // 读取配置获取 dapr runtime 的 http 路径
                var daprHttpService = _configuration["Dapr:RuntimeHttpService"];
                // 拼接 dapr state url
                var stateUrl = $"{daprHttpService}/state/statestore/{StateStoreName}";


                // 创建http client
                var httpClient = _httpClientFactory.CreateClient("dapr_state");


                var response = await httpClient.DeleteAsync(stateUrl);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return "删除订单失败!";
                }
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
