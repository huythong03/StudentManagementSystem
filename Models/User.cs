namespace StudentManagementSystem.Models
{
	public class User
	{
		public string IdStudent { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Note { get; set; }
		public bool Status { get; set; }
		public System.DateTime CreatedAt { get; set; }
		public System.DateTime ModifiedAt { get; set; }
	}
}