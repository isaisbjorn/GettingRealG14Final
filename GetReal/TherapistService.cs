using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class TherapistService
    {
        private IRepository<Therapist> _therapistRepository;

        public TherapistService(IRepository<Therapist> repository)
        {
            _therapistRepository = repository;
        }

        public void CreateTherapist(
            string firstName,
            string lastName,
            string title,
            string email,
            string userName,
            string password)
        {
            Therapist therapist = new Therapist(firstName, lastName, title, email, userName, password);

            _therapistRepository.Add(therapist);
        }
        public Therapist? GetTherapistById(int id) => _therapistRepository.GetById(id);

        public List<Therapist> GetAllTherapists()
        {
            return _therapistRepository.GetAll();
        }

        public void UpdateTherapist(Therapist therapist)
        {
            _therapistRepository.Update(therapist);
        }

        public void DeleteTherapist(int id)
        {
            _therapistRepository.Remove(id);
        }
        public IEnumerable<Therapist> SearchTherapist(string searchString)
        {
            searchString = searchString.ToLower();
            var therapists = _therapistRepository.GetAll().Where(f => f.FirstName.ToLower().Contains(searchString) ||
            f.LastName.ToLower().Contains(searchString));
            return therapists;
        }
    }
}