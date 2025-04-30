namespace StudentManagementSystem.Models
{
	public class Student
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public DateTime BOF { get; set; }
		public int IdProvince { get; set; }
		public bool Gender { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}