using Kingmaker_Kingdom_Sheet.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Kingmaker_Kingdom_Sheet.Shared.Models;

namespace KingmakerKingdomSheet.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class KingdomInfoController : ControllerBase
	{
		private readonly ILogger<KingdomInfoController> _logger;
		public KingdomInfoController(ILogger<KingdomInfoController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public Kingdom Get()
		{
			var kingdom = new Kingdom
			{
				Name = "Calrastia",
				Alignment = "CN",
				Government = "Autocracy",
				Economy = 121,
				Loyalty = 95,
				Stability = 92,
				Consumption = 4,
				IncomeModifier = 25,
				Unrest = 0,
				UnrestPerTurn = 0
			};
			return kingdom;
		}

		[HttpPost]
		public Kingdom Post(Kingdom kingdom)
		{
			kingdom.Consumption++;
			return kingdom;
		}
	}
}
