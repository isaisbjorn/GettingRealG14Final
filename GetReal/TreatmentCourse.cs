
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class TreatmentCourse
    {
        public int TreatmentCourseId { get; set; }
        public string Issue { get; set; } = "Ny klient";
        public string Development { get; set; } = "Noter til behandlingen";
        public DateTime Created { get; init; } = DateTime.Now;
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<AssignedExercise> AssignedExercises { get; set; } = new List<AssignedExercise>();
    }
}

