namespace StudentManagementSystem.Models
{
	public class Class
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string SubjectId { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}