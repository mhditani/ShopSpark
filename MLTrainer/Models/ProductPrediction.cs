using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTrainer.Models
{
    public class ProductPrediction
    {
        [ColumnName("Score")]
        public float PredictedRating { get; set; }
    }
}
