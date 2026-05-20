using System;
using GetReal;

namespace GetReal
{
	public class AppointmentMenu
	{
        private readonly ClientService _clientService;
        private readonly AppointmentService _appointmentService;
        private readonly TherapistService _therapistService;
        private readonly EntitySelector _entitySelector;
        public AppointmentMenu(ClientService clientService, AppointmentService appointmentService, TherapistService therapistService)
		{
            _clientService = clientService;
            _appointmentService = appointmentService;
            _therapistService = therapistService;
            _entitySelector = new EntitySelector(clientService, appointmentService, therapistService);
        }

		public void Show()
		{
			bool running = true;
			while (running)
			{
				Console.Clear();
				Console.WriteLine("=== Appointment Menu ===");
				Console.WriteLine("1. Opret aftale");
				Console.WriteLine("2. Rediger aftale");
				Console.WriteLine("3. Slet aftale");
				Console.WriteLine("4. Søg efter aftale for klient");
                Console.WriteLine("0. Tilbage");
				Console.Write("\nVælg et punkt: ");

				string input = Console.ReadLine();

				switch (input)
				{
					case "1":
						Console.Clear();
						Console.WriteLine("=== Opret Aftale ===\n");

                        Client? selectedClient = _entitySelector.SelectClient();

                        if (selectedClient == null)
                        {
                            Console.WriteLine("\nIngen klienter fundet");
                            UIHelper.Wait();
                            break;
                        }

                        Console.Clear();
                        Console.WriteLine(selectedClient.Print());
                        TreatmentCourse selectedTreatment = (selectedClient.TreatmentCourses.Count() > 1) ? _entitySelector.SelectTreatment(selectedClient) : selectedClient.TreatmentCourses[0];

						DateOnly date = UIHelper.AskDate("Dato (dd-MM-yyyy):");
						
                        var (startTime, endTime) = UIHelper.AskTimeRange("Starttidspunkt? (HH:mm)", "Sluttidspunkt? (HH:mm)");

                        bool isPaid = UIHelper.AskYesNo("\"Er aftalen betalt");


                        Console.Write("Vælg behandler\n");
                        Therapist therapist = _entitySelector.SelectTherapist();
                        if (therapist == null)
                        {
                            Console.WriteLine("\nIngen behandlere fundet");
                            UIHelper.Wait();
                            break;
                        }

                        Appointment appointment = new Appointment
						{
							Date = date,
							StartTime = startTime,
							EndTime = endTime,
							IsPaid = isPaid,
							TherapistId = therapist.Id
						};

						try
						{
							_appointmentService.AddAppointment(selectedClient.Id, selectedTreatment.TreatmentCourseId, appointment);
                            Console.Clear();
                            Console.WriteLine("\n=== Aftale Oprettet ✓ ===");
							Console.WriteLine($"Dato: {appointment.Date}");
							Console.WriteLine($"Starttidspunkt: {appointment.StartTime}");
							Console.WriteLine($"Sluttidspunkt: {appointment.EndTime}");
							Console.WriteLine($"Betalt: {(appointment.IsPaid ? "Ja" : "Nej")}");
							Console.WriteLine($"Behandler: {therapist.FirstName} {therapist.LastName}");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"\nFejl: {ex.Message}");
						}

                        UIHelper.Wait();
                        break;

					case "2":
						Console.Clear();
						Console.WriteLine("=== Rediger Aftale ===\n");
                        
						selectedClient = _entitySelector.SelectClient();
                        if (selectedClient == null)
                        {
                            Console.WriteLine("\nIngen klienter fundet");
                            UIHelper.Wait();
                            break;
                        }
                        Console.WriteLine(selectedClient.Print());
                        selectedTreatment = (selectedClient.TreatmentCourses.Count() > 1) ? _entitySelector.SelectTreatment(selectedClient) : selectedClient.TreatmentCourses[0];
                        if (selectedTreatment.Appointments.Count() > 0)
                        {
                            foreach (var appt in selectedTreatment.Appointments)
                            {
                                Console.WriteLine($"Aftale Id: {appt.AppointmentId} Dato: {appt.Date} Starttid: {appt.StartTime} Sluttid: {appt.EndTime}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Klienten har ingen aftaler");
                            UIHelper.Wait();
							break;
                        }

						int editAppointmentId = UIHelper.AskInt("Aftale Id?");


                        try
						{
							Appointment existingAppointment = _appointmentService.GetAppointmentById(selectedClient.Id, selectedTreatment.TreatmentCourseId, editAppointmentId);
							existingAppointment.Date = UIHelper.AskDate("Ny dato(ddMMyyyy):");
                            (startTime, endTime) = UIHelper.AskTimeRange("Starttidspunkt? (HHmm)", "Sluttidspunkt? (HHmm)");
                            existingAppointment.StartTime = startTime;
                            existingAppointment.EndTime = endTime;
							existingAppointment.IsPaid = UIHelper.AskYesNo("\"Er aftalen betalt");

                            Console.Write("Vælg behandler\n");
                            therapist = _entitySelector.SelectTherapist();
                            if (therapist == null)
                            {
                                Console.WriteLine("\nIngen behandlere fundet");
                                UIHelper.Wait();
                                break;
                            }
                            existingAppointment.TherapistId = therapist.Id;

                            _appointmentService.UpdateAppointment(selectedClient.Id, selectedTreatment.TreatmentCourseId, existingAppointment);
							Console.WriteLine("\nAftale opdateret! ✓");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"\nFejl: {ex.Message}");
						}

						Console.ReadKey();
						break;

					case "3":
						Console.Clear();
						Console.WriteLine("=== Slet Aftale ===\n");

                        selectedClient = _entitySelector.SelectClient();
                        if (selectedClient == null)
                        {
                            Console.WriteLine("\nIngen klienter fundet");
                            UIHelper.Wait();
                            break;
                        }
                        Console.WriteLine(selectedClient.Print());
                        selectedTreatment = (selectedClient.TreatmentCourses.Count() > 1) ? _entitySelector.SelectTreatment(selectedClient) : selectedClient.TreatmentCourses[0];
                        if (selectedTreatment.Appointments.Count() > 0)
                        {
                            foreach (var appt in selectedTreatment.Appointments)
                            {
                                Console.WriteLine($"Aftale Id: {appt.AppointmentId} Dato: {appt.Date} Starttid: {appt.StartTime} Sluttid: {appt.EndTime}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Klienten har ingen aftaler");
                            UIHelper.Wait();
                            break;
                        }

                        int deleteAppointmentId = UIHelper.AskInt("Aftale Id?");

                        bool isConfirmed = UIHelper.AskYesNo($"Er du sikker på du vil slette aftalen med aftale Id {deleteAppointmentId}");

						if (isConfirmed)
						{
							try
							{
                                _appointmentService.DeleteAppointment(selectedClient.Id, selectedTreatment.TreatmentCourseId, deleteAppointmentId);
								Console.WriteLine("\nAftale slettet!");
							}
							catch (Exception ex)
							{
								Console.WriteLine($"\nFejl: {ex.Message}");
							}
						}
						else
						{
							Console.WriteLine("\nSletning annulleret.");
						}

                        UIHelper.Wait();
                        break;

					case "4":
						Console.Clear();
						Console.WriteLine("=== Søg efter Aftale ===\n");

                        selectedClient = _entitySelector.SelectClient();
                        if (selectedClient == null)
                        {
                            Console.WriteLine("\nIngen klienter fundet");
                            UIHelper.Wait();
                            break;
                        }
                        Console.WriteLine(selectedClient.Print());
                        selectedTreatment = (selectedClient.TreatmentCourses.Count() > 1) ? _entitySelector.SelectTreatment(selectedClient) : selectedClient.TreatmentCourses[0];
                        if (selectedTreatment.Appointments.Count() > 0)
                        { 
                            foreach (var appt in selectedTreatment.Appointments)
                            {
                                therapist = _therapistService.GetTherapistById(appt.TherapistId);
                                Console.WriteLine($"Aftale Id: {appt.AppointmentId} Dato: {appt.Date} Starttid: {appt.StartTime} Sluttid: {appt.EndTime} Behandler: {therapist.FirstName} {therapist.LastName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Klienten har ingen aftaler");
                            UIHelper.Wait();
                            break;
                        }

                        int searchAppointmentId = UIHelper.AskInt("Aftale Id?");

                        try
						{
							Appointment foundAppointment = _appointmentService.GetAppointmentById(selectedClient.Id, selectedTreatment.TreatmentCourseId, searchAppointmentId);
                            therapist = _therapistService.GetTherapistById(foundAppointment.TherapistId);
                            Console.Clear();
                            Console.WriteLine("\n=== Aftale Fundet ===");
							Console.WriteLine($"Aftale ID: {foundAppointment.AppointmentId}");
							Console.WriteLine($"Dato: {foundAppointment.Date}");
							Console.WriteLine($"Starttidspunkt: {foundAppointment.StartTime}");
							Console.WriteLine($"Sluttidspunkt: {foundAppointment.EndTime}");
							Console.WriteLine($"Betalt: {(foundAppointment.IsPaid ? "Ja" : "Nej")}");
							Console.WriteLine($"Behandler: {therapist.FirstName} {therapist.LastName}");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"\nFejl: {ex.Message}");
						}

                        UIHelper.Wait();
                        break;

					case "0":
						running = false;
						break;

					default:
						Console.WriteLine("Ugyldigt valg, prøv igen.");
                        UIHelper.Wait();
                        break;
				}
			}
		}
	}
}