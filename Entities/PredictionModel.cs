using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace predictionService.Entities
{
	public class PredictionModel
	{
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        //public User User { get; set; }

        //[ForeignKey("User")]
        public int UserId { get; set; }
    }
}

