using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class ExerciseTemplate : IHasId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BodyPart BodyPart { get; set; }
       
        public ExerciseTemplate(int id, string name, string description, BodyPart bodyPart)
        {
            Id = id;
            Name = name;
            Description = description;
            BodyPart = bodyPart;
        }
       
    }
}
