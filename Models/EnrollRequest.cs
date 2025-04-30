namespace StudentManagementSystem.Models
{
	public class EnrollRequest
	{
		public int RequestId { get; set; }
		public string IdStudent { get; set; }
		public string StudentName { get; set; }
		public string IdSubject { get; set; }
		public string SubjectName { get; set; }
		public DateTime RequestDate { get; set; }
		public string Status { get; set; } // Pending, Approved, Rejected (Đang chờ xử lý, Đã phê duyệt, Bị từ chối)
	}
}