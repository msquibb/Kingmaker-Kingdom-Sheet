using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingmaker_Kingdom_Sheet.Shared.Models
{
	public class Kingdom
	{
		public Kingdom()
		{
			Edicts = new Edicts();
		}

		[Required]
		[StringLength(30, ErrorMessage = "Name is too long (30 characters max).")]
		public string Name { get; set; }

		[Required]
		public string Alignment { get; set; }

		
		public string Government { get; set; }

		public int ControlDC { get; set; }

		public int Economy { get; set; }
		public int Loyalty { get; set; }
		public int Stability { get; set; }
		
		public int Consumption { get; set; }
		public int IncomeModifier { get; set; }
		public int Unrest { get; set; }
		public int UnrestPerTurn { get; set; }

		public int Fame { get; set; }
		public int Infamy { get; set; }
		public int UnspentReputation { get; set; }

		public int Corruption { get; set; }
		public int Crime { get; set; }
		public int Law { get; set; }
		public int Lore { get; set; }
		public int Productivity { get; set; }
		public int Society { get; set; }

		public Edicts Edicts { get; set; }

		public bool UseHybridRules { get; set; }
	}
}
