using System;
using System.Collections.Generic;
using System.Text;
using GetReal;

namespace GetReal
{
	public class ClientService
	{
		private readonly IRepository<Client> _clientRepository;

		public ClientService(IRepository<Client> clientRepository)
		{
			_clientRepository = clientRepository;
		}
		public void CreateClient(
			string firstName,
			string lastName,
			string phone,
			string email,
			DateOnly birthday)
		{
			Client client = new Client(firstName, lastName, phone, email, birthday);
			client.TreatmentCourses[0].TreatmentCourseId = GenerateTreatmentId();
			_clientRepository.Add(client);
		}
        public Client? GetClientById(int id) => _clientRepository.GetById(id);
        public void UpdateClient(Client client)
        {
            _clientRepository?.Update(client);
        }
        public void RemoveClient(int id) => _clientRepository.Remove(id);
        public IEnumerable<Client> SearchClient(string searchString)
        {
            searchString = searchString.ToLower();
            var clients = _clientRepository.GetAll().Where(f => f.FirstName.ToLower().Contains(searchString) ||
            f.LastName.ToLower().Contains(searchString) ||
            f.Phone.Contains(searchString) ||
            f.Email.ToLower().Contains(searchString));
            return clients;
        }
		public void CreateTreatmentCourse(Client client)
		{
            TreatmentCourse treatment = new TreatmentCourse();
			int Id = GenerateTreatmentId();
			treatment.TreatmentCourseId = Id;
            client.TreatmentCourses.Add(treatment);
        }
        public int GenerateTreatmentId()
		{
			var clients = _clientRepository.GetAll();
			if (clients.Count == 0)
			{
				return 1;
			}
			else
			{
				return clients
				.SelectMany(c => c.TreatmentCourses)
				.Max(t => t.TreatmentCourseId) + 1;
			}
		}
		public List<Client> GetAllClients()
		{
			return _clientRepository.GetAll(); 
		}
    }
}