using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld.Dtos
{
    public class NewOrderInput
    {
        public OrderDto Data { get; set; }
    }

    public class OrderDto
    {
        public string OrderId { get; set; }
    }
}
