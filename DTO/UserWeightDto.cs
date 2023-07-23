using System;
namespace predictionService.DTO
{
	public class UserWeightDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public int UserId { get; set; }
    }
}

