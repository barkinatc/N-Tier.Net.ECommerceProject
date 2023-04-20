using Project.ENTITIES.Models;
using Project.MVCUI.OutherRequestModel;
using Project.VM.PureVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVCUI.Models.PageVMs
{
    public class OrderPageVM
    {
        //Refaktor
        public Order  Order { get; set; }

        public List<Order> Orders { get; set; }

        public PaymentRequestModel PaymentRM { get; set; }
    }
}