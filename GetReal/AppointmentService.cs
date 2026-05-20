using System;
using System.Collections.Generic;
using System.Linq;
using GetReal;

namespace GetReal
{
	public class AppointmentService
	{
		private readonly IRepository<Client> _clientRepository;

		public AppointmentService(IRepository<Client> clientRepository)
		{
			_clientRepository = clientRepository;
		}

        public void AddAppointment(int clientId, int treatmentCourseId, Appointment appointment)
        {
            var client = _clientRepository.GetById(clientId);
            if (client == null)
                throw new Exception("Client not found");

            var treatmentCourse = client.TreatmentCourses
                .FirstOrDefault(t => t.TreatmentCourseId == treatmentCourseId);
            if (treatmentCourse == null)
                throw new Exception("TreatmentCourse not found");

            // Tjekker om der et møde i samme tidsrum
            bool hasOverlap = treatmentCourse.Appointments.Any(a =>
                a.Date == appointment.Date &&
                a.StartTime < appointment.EndTime &&
                a.EndTime > appointment.StartTime
            );

            if (hasOverlap)
                throw new Exception("Der er allerede en aftale på dette tidspunkt!");

            appointment.AppointmentId = GenerateAppointmentId();
            treatmentCourse.Appointments.Add(appointment);
            _clientRepository.Update(client);
        }

        public Appointment GetAppointmentById(int clientId, int treatmentCourseId, int appointmentId)
		{
			var client = _clientRepository.GetById(clientId);
			if (client == null)
				throw new Exception("Client not found");

			var treatmentCourse = client.TreatmentCourses
				.FirstOrDefault(t => t.TreatmentCourseId == treatmentCourseId);
			if (treatmentCourse == null)
				throw new Exception("TreatmentCourse not found");

			return treatmentCourse.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
		}

		public void UpdateAppointment(int clientId, int treatmentCourseId, Appointment updatedAppointment)
		{
			var client = _clientRepository.GetById(clientId);
			if (client == null)
				throw new Exception("Client not found");

			var treatmentCourse = client.TreatmentCourses
				.FirstOrDefault(t => t.TreatmentCourseId == treatmentCourseId);
			if (treatmentCourse == null)
				throw new Exception("TreatmentCourse not found");

			var appointment = treatmentCourse.Appointments
				.FirstOrDefault(a => a.AppointmentId == updatedAppointment.AppointmentId);
			if (appointment == null)
				throw new Exception("Appointment not found");

			appointment.Date = updatedAppointment.Date;
			appointment.StartTime = updatedAppointment.StartTime;
			appointment.EndTime = updatedAppointment.EndTime;
			appointment.IsPaid = updatedAppointment.IsPaid;
			appointment.TherapistId = updatedAppointment.TherapistId;

			_clientRepository.Update(client);
		}

		public void DeleteAppointment(int clientId, int treatmentCourseId, int appointmentId)
		{
			var client = _clientRepository.GetById(clientId);
			if (client == null)
				throw new Exception("Client not found");

			var treatmentCourse = client.TreatmentCourses
				.FirstOrDefault(t => t.TreatmentCourseId == treatmentCourseId);
			if (treatmentCourse == null)
				throw new Exception("TreatmentCourse not found");

			var appointment = treatmentCourse.Appointments
				.FirstOrDefault(a => a.AppointmentId == appointmentId);
			if (appointment == null)
				throw new Exception("Appointment not found");

			treatmentCourse.Appointments.Remove(appointment);
			_clientRepository.Update(client);
		}

        public int GenerateAppointmentId()
        {
            var appointments = _clientRepository.GetAll()
                .SelectMany(c => c.TreatmentCourses)
                .SelectMany(t => t.Appointments)
                .ToList();

            return appointments.Count > 0 ? appointments.Max(a => a.AppointmentId) + 1 : 1;
        }
    }
}