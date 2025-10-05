using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTrainer.Models
{
    public class ProductInteraction
    {
        [LoadColumn(0)]
        public string UserId { get; set; } = string.Empty;

        [LoadColumn(1)]
        public float ProductId { get; set; }

        [LoadColumn(2)]
        public float Rating { get; set; }
    }
}
