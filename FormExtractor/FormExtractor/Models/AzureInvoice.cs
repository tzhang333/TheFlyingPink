using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormExtractor.Models
{
    public class AzureInvoice
    {
        public string Id { get; set; }

        public double Total { get; set; }

        public bool Success { get; set; }

        public string JSON { get; set; }
    }
}