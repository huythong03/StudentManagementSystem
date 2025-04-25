namespace StudentManagementSystem.Models
{
	public class Enrol
	{
		public string IdStudent { get; set; }
		public string IdSubject { get; set; }
		public string Name { get; set; } // Subject Name
		public string StudentName { get; set; }
		public decimal? Mark { get; set; }
	}
}