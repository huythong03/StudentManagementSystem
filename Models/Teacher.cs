namespace StudentManagementSystem.Models
{
	public class Teacher
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Major { get; set; } // chuyên ngành
		public string ProfessionalQualification { get; set; } // trình độ chuyên môn
		public bool? Gender { get; set; } // true: Male, false: Female
		public string Ethnicity { get; set; } // dân tộc
		public bool? PartyMember { get; set; } // đảng viên
		public string ForeignLanguageLevel { get; set; } // trình độ ngoại ngữ
		public string ITLevel { get; set; } // trình độ tin học
	}
}
