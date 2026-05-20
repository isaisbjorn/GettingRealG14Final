using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GetReal
{
	public class Client : IHasId
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public DateOnly Birthday { get; set; }
		public List<AssignedExercise> AssignedExercises { get; set; } = new List<AssignedExercise>();
		public List<TreatmentCourse> TreatmentCourses { get; set; } = new List<TreatmentCourse>();
		public Client(
			string firstName,
			string lastName,
			string phone,
			string email,
			DateOnly birthday)
		{
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
			Email = email;
			Birthday = birthday;
			TreatmentCourse treatmentCourse = new TreatmentCourse();
			TreatmentCourses.Add(treatmentCourse); // En ny klient starter altid med et behandlingsforløb
		}
		public string Print() 
		{
			return $"{FirstName} {LastName}, E-mail: {Email}, Tlf. {Phone}";
		}
	}
}