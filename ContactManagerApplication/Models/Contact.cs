using System;

namespace ContactManagerApplication.Models
{
	public class Contact
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime BirthDate { get; set; }
		public bool IsMarried { get; set; }
		public string Phone { get; set; }
		public decimal Salary { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}
