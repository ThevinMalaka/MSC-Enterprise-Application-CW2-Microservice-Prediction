using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using predictionService.DTO;
using predictionService.Entities;
using System.Text.Json;
using System.Net.Http.Headers;

namespace predictionService.Services
{
	public class PredictionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _userServiceEndpoint; // Getting var from appsetting.json file
        private readonly string _workoutServiceEndpoint; // Getting var from appsetting.json file
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PredictionService(ApplicationDbContext context, IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _clientFactory = clientFactory;
            _userServiceEndpoint = configuration["UserServiceEndpoint"]; // Getting var from appsetting.json file
            _workoutServiceEndpoint = configuration["WorkoutServiceEndpoint"]; // Getting var from appsetting.json file
            _httpContextAccessor = httpContextAccessor;
        }

        //create a new prediction
        public async Task<PredictionModel> CreatePredictionAsync(int userId)
        {

            //get current workout plan
            UserWorkoutEnrollmentModel currentUserEnrollment = null;
            //var currentUserEnrollment = await _context.UserWorkoutEnrollments.Where(uwe => uwe.UserId == userId).OrderByDescending(uwe => uwe.Id).FirstOrDefaultAsync();

            // --------------------------------------------------------
            // --------------------------------------------------------
            // --------------------------------------------------------
            // Call the WORKOUT SERIVCE through API GATEWAY url ----------
            string jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            string workoutRequestEndpoint = string.Format(_workoutServiceEndpoint, userId);
            var workoutResponse = await client.GetAsync(workoutRequestEndpoint);

            if (workoutResponse.IsSuccessStatusCode)
            {
                var workoutResponseString = await workoutResponse.Content.ReadAsStringAsync();
                currentUserEnrollment = JsonSerializer.Deserialize<UserWorkoutEnrollmentModel>(workoutResponseString);
                Console.WriteLine("BBBBBBBBB ----- workoutResponse ----", currentUserEnrollment);
            }
            Console.WriteLine("CCCCCCCC ----- workoutResponse ----", workoutResponse);

            // Call the WORKOUT SERIVCE through API GATEWAY url
            // --------------------------------------------------------
            // --------------------------------------------------------
            // --------------------------------------------------------

            if (currentUserEnrollment == null)
            {
                throw new Exception("No UserWorkoutEnrollment found for the given userId");
            }

            //var currentWorkoutPlan = currentUserEnrollment.WorkoutPlan;
            var currentWorkoutPlan = "";
            if (currentWorkoutPlan == null)
            {
                throw new Exception("WorkoutPlan is null in the current UserWorkoutEnrollment");
            }

            //var currentWorkoutPlanMET = currentWorkoutPlan.TotalMET;
            var currentWorkoutPlanMET = 0;
            if (currentWorkoutPlanMET == null)
            {
                throw new Exception("TotalMET is null in the current WorkoutPlan");
            }

            //get his latest weight
            //var currentWeight = await _context.UserWeightsLogs.Where(uw => uw.UserId == userId).OrderByDescending(uw => uw.Id).FirstOrDefaultAsync();

            // --------------------------------------------------------
            // --------------------------------------------------------
            // --------------------------------------------------------
            // Call the USER SERIVCE through API GATEWAY url ----------
            string requestEndpoint = string.Format(_userServiceEndpoint, userId);

            var response = await client.GetAsync(requestEndpoint);

            double currentWeight = 0;

            Console.WriteLine("DDDDDDDD ----- user service ----", currentWeight);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var latestWeightData = JsonSerializer.Deserialize<UserWeightDto>(responseString);
                currentWeight = latestWeightData.Weight;
            }

            Console.WriteLine("Current Weight------>", currentWeight);
            // Call the USER SERIVCE through API GATEWAY url
            // --------------------------------------------------------
            // --------------------------------------------------------
            // --------------------------------------------------------

            if (currentWeight == null)
            {
                throw new Exception("No UserWeightsLog found for the given userId");
            }

            var currentWeightValue = currentWeight;
            //var currentWeightValue = currentWeight.Weight;
            if (currentWeightValue == null)
            {
                throw new Exception("Weight is null in the current UserWeightsLog");
            }

            //get workout days
            //var workoutDays = currentUserEnrollment.Days;
            var workoutDays = 20;

            // get prediction date
            var predictionDate = DateTime.Now.AddDays(workoutDays);

            var avgDurationForWorkout = 20;

            // calculate calories burned
            var caloriesBurned = (currentWorkoutPlanMET * currentWeightValue * 3.5 * (avgDurationForWorkout * workoutDays)) / 200;

            // calculate weight loss
            var weightLoss = caloriesBurned / 7700;

            // average calories gain
            var avgCaloriesGain = 2000;

            // average weight gain
            var avgWeightGain = (avgCaloriesGain * workoutDays) / 7700;

            // calculate predicted weight
            var predictedWeight = currentWeightValue + avgWeightGain - weightLoss; // in kg

            // create new prediction
            var newPrediction = new PredictionModel
            {
                UserId = userId,
                Date = predictionDate,
                Weight = predictedWeight
            };

            _context.Predictions.Add(newPrediction);
            await _context.SaveChangesAsync();

            return newPrediction;
        }

        //get all predictions by user id
        public async Task<List<PredictionModel>> GetPredictionsByUserIdAsync(int id)
        {


            return await _context.Predictions.Where(p => p.UserId == id).OrderByDescending(p => p.Id).ToListAsync();
        }

    }
}

