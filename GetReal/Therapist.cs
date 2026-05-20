using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GetReal
{
	public class Therapist : IHasId
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Therapist( 
            string firstName,
            string lastName,
            string title,
            string email,
            string userName,
            string password)
		{
                FirstName = firstName;
                LastName = lastName;
                Title = title;
                Email = email;
                UserName = userName;
                Password = password;
            }
    }
}