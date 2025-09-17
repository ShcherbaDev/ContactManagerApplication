using System;
using CsvHelper.Configuration.Attributes;

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

	public class ContactDto
	{
		[Name("Name")]
		public string Name { get; set; }

		[Name("Date of birth")]
		public DateTime DateOfBirth { get; set; }

		[Name("Married")]
		public bool IsMarried { get; set; }

		[Name("Phone")]
		public string Phone { get; set; }

		[Name("Salary")]
		public decimal Salary { get; set; }


		public static Contact ToContactMap(ContactDto contactDto)
		{
			return new Contact
			{
				Name = contactDto.Name,
				BirthDate = contactDto.DateOfBirth,
				IsMarried = contactDto.IsMarried,
				Phone = contactDto.Phone,
				Salary = contactDto.Salary
			};
		}
	}
}
