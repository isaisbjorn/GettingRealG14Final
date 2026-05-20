using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
	public class Appointment
	{
		public int AppointmentId { get; set; }
		public DateOnly Date { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public bool IsPaid { get; set; }
		public int TherapistId { get; set; }
	}
}