using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
	public class AssignedExercise : IHasId
	{
		public int Id { get; set; }
		public int Repetitions { get; set; }
		public int Sets { get; set; }
		public int ExerciseTemplateId { get; set; }
	}
}