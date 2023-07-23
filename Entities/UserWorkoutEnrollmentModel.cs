using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace predictionService.Entities
{
	public class UserWorkoutEnrollmentModel
	{
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        //public UserModel User { get; set; }

        //[ForeignKey("UserModel")]
        //public int UserId { get; set; }

        //[JsonIgnore]
        //public WorkoutPlanModel WorkoutPlan { get; set; }

        [ForeignKey("WorkoutPlanModel")]
        public int WorkoutPlanId { get; set; }
        public int Days { get; set; }
        public int CompletedDays { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
    }
}

