namespace StudentManagementSystem.Models
{
	public class Teacher
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Major { get; set; }
		public string ProfessionalQualification { get; set; }
		public bool? Gender { get; set; } // true: Male, false: Female
		public string Ethnicity { get; set; }
		public bool? PartyMember { get; set; }
		public string ForeignLanguageLevel { get; set; }
		public string ITLevel { get; set; }
	}
}
