using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingmaker_Kingdom_Sheet.Shared.Models
{
	public class Edicts
	{
		public string Holiday { get; set; }
		public string Promotion { get; set; }
		public string Taxation { get; set; }
		public string Expansion { get; set; }
		public string Recruitment { get; set; }

		public bool UseRecruitment { get; set; }
	}
}
