using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class ExerciseTemplateService
    {
        private IRepository<ExerciseTemplate> _repository;

        public ExerciseTemplateService(IRepository<ExerciseTemplate> repository)
        {
            _repository = repository;
        }

        public void AddExercise(ExerciseTemplate exercise)
        {
            _repository.Add(exercise);
        }
        public ExerciseTemplate? GetExerciseTemplateById(int id) => _repository.GetById(id);

        public List<ExerciseTemplate> GetAllExercises()
        {
            return _repository.GetAll();
        }

        public void UpdateExercise(ExerciseTemplate exercise)
        {
            _repository.Update(exercise);
        }

        public void DeleteExercise(int id)
        {
            _repository.Remove(id);
        }
    }
}
