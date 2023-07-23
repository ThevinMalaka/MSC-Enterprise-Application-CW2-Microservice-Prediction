using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using predictionService.Entities;
using predictionService.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace predictionService.Controllers
{
    [Route("[controller]")]
    public class PredictionController : Controller
    {
        private readonly PredictionService _predictionService;

        public PredictionController(PredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet("{userId}")]
        public async Task<List<PredictionModel>> Get(int userId)
        {
            //get all predictions from the database
            return await _predictionService.GetPredictionsByUserIdAsync(userId);
        }
        
    }
}

