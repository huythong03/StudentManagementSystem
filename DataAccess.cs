using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using StudentManagementSystem.Models;

namespace StudentManagementSystem
{
	public class DataAccess
	{
		private readonly string connectionString;

		public DataAccess()
		{
			connectionString = ConfigurationManager.ConnectionStrings["QuanLySinhVienConnection"]?.ConnectionString
				?? throw new ConfigurationErrorsException("Connection string 'QuanLySinhVienConnection' not found in app.config.");
		}

		private SqlConnection GetConnection()
		{
			try
			{
				SqlConnection conn = new SqlConnection(connectionString);
				conn.Open();
				return conn;
			}
			catch (SqlException ex)
			{
				throw new Exception("Failed to connect to the database.", ex);
			}
		}

		public bool ValidateLogin(string username, string password)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username AND Password = @Password AND Status = 1";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						cmd.Parameters.AddWithValue("@Password", password);
						return (int)cmd.ExecuteScalar() > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error validating login credentials.", ex);
			}
		}

		public string GetUserRole(string username)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT r.Name 
                        FROM [User] u 
                        JOIN UserRole ur ON u.IdStudent = ur.IdStudent 
                        JOIN Role r ON ur.IdRole = r.Id 
                        WHERE u.Username = @Username AND u.Status = 1";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						return cmd.ExecuteScalar()?.ToString() ?? "user";
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving user role.", ex);
			}
		}

		public string GetStudentIdByUsername(string username)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT IdStudent FROM [User] WHERE Username = @Username";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						return cmd.ExecuteScalar()?.ToString();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving student ID.", ex);
			}
		}

		public List<Student> GetStudents()
		{
			List<Student> students = new List<Student>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id, Name, BOF, IdProvince, Gender FROM [Student]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								students.Add(new Student
								{
									Id = reader["Id"].ToString(),
									Name = reader["Name"].ToString(),
									BOF = reader.GetDateTime(reader.GetOrdinal("BOF")),
									IdProvince = reader.GetInt32(reader.GetOrdinal("IdProvince")),
									Gender = reader.GetBoolean(reader.GetOrdinal("Gender"))
								});
							}
						}
					}
				}
				return students;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving students.", ex);
			}
		}

		public void AddStudent(Student student)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        INSERT INTO [Student] (Id, Name, BOF, IdProvince, Gender)
                        VALUES (@Id, @Name, @BOF, @IdProvince, @Gender)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", student.Id);
						cmd.Parameters.AddWithValue("@Name", student.Name);
						cmd.Parameters.AddWithValue("@BOF", student.BOF);
						cmd.Parameters.AddWithValue("@IdProvince", student.IdProvince);
						cmd.Parameters.AddWithValue("@Gender", student.Gender);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error adding student.", ex);
			}
		}

		public void UpdateStudent(Student student)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        UPDATE [Student]
                        SET Name = @Name, BOF = @BOF, IdProvince = @IdProvince, Gender = @Gender
                        WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", student.Id);
						cmd.Parameters.AddWithValue("@Name", student.Name);
						cmd.Parameters.AddWithValue("@BOF", student.BOF);
						cmd.Parameters.AddWithValue("@IdProvince", student.IdProvince);
						cmd.Parameters.AddWithValue("@Gender", student.Gender);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating student.", ex);
			}
		}

		public void DeleteStudent(string id)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Student] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error deleting student.", ex);
			}
		}

		public List<Province> GetProvinces()
		{
			List<Province> provinces = new List<Province>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id, Name FROM [Province]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								provinces.Add(new Province
								{
									Id = reader.GetInt32(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name"))
								});
							}
						}
					}
				}
				return provinces;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving provinces.", ex);
			}
		}

		public void AddProvince(Province province)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Province] (Id, Name) VALUES (@Id, @Name)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", province.Id);
						cmd.Parameters.AddWithValue("@Name", province.Name);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error adding province.", ex);
			}
		}

		public void UpdateProvince(Province province)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [Province] SET Name = @Name WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", province.Id);
						cmd.Parameters.AddWithValue("@Name", province.Name);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating province.", ex);
			}
		}

		public void DeleteProvince(int id)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Province] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error deleting province.", ex);
			}
		}

		public List<Subject> GetSubjects()
		{
			List<Subject> subjects = new List<Subject>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id, Name FROM [Subject]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								subjects.Add(new Subject
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name"))
								});
							}
						}
					}
				}
				return subjects;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving subjects.", ex);
			}
		}

		public void AddSubject(Subject subject)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Subject] (Id, Name) VALUES (@Id, @Name)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", subject.Id);
						cmd.Parameters.AddWithValue("@Name", subject.Name);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error adding subject.", ex);
			}
		}

		public void UpdateSubject(Subject subject)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [Subject] SET Name = @Name WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", subject.Id);
						cmd.Parameters.AddWithValue("@Name", subject.Name);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating subject.", ex);
			}
		}

		public void DeleteSubject(string id)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Subject] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error deleting subject.", ex);
			}
		}

		public List<User> GetUsers()
		{
			List<User> users = new List<User>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT IdStudent, Username, Password, Note, Status, CreatedAt, ModifiedAt FROM [User]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								users.Add(new User
								{
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									Username = reader.GetString(reader.GetOrdinal("Username")),
									Password = reader.GetString(reader.GetOrdinal("Password")),
									Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
									Status = reader.GetBoolean(reader.GetOrdinal("Status")),
									CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
									ModifiedAt = reader.GetDateTime(reader.GetOrdinal("ModifiedAt"))
								});
							}
						}
					}
				}
				return users;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving users.", ex);
			}
		}

		public void AddUser(User user)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        INSERT INTO [User] (IdStudent, Username, Password, Note, Status, CreatedAt, ModifiedAt)
                        VALUES (@IdStudent, @Username, @Password, @Note, @Status, @CreatedAt, @ModifiedAt)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", user.IdStudent);
						cmd.Parameters.AddWithValue("@Username", user.Username);
						cmd.Parameters.AddWithValue("@Password", user.Password);
						cmd.Parameters.AddWithValue("@Note", (object)user.Note ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@Status", user.Status);
						cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
						cmd.Parameters.AddWithValue("@ModifiedAt", user.ModifiedAt);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error adding user.", ex);
			}
		}

		public void UpdateUser(User user)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        UPDATE [User]
                        SET Username = @Username, Password = @Password, Note = @Note, Status = @Status, ModifiedAt = @ModifiedAt
                        WHERE IdStudent = @IdStudent";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", user.IdStudent);
						cmd.Parameters.AddWithValue("@Username", user.Username);
						cmd.Parameters.AddWithValue("@Password", user.Password);
						cmd.Parameters.AddWithValue("@Note", (object)user.Note ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@Status", user.Status);
						cmd.Parameters.AddWithValue("@ModifiedAt", user.ModifiedAt);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating user.", ex);
			}
		}

		public void DeleteUser(string idStudent)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [User] WHERE IdStudent = @IdStudent";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", idStudent);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error deleting user.", ex);
			}
		}

		public void UpdateUserPassword(string username, string newPassword)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [User] SET Password = @Password, ModifiedAt = @ModifiedAt WHERE Username = @Username";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						cmd.Parameters.AddWithValue("@Password", newPassword);
						cmd.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating user password.", ex);
			}
		}

		public List<Role> GetRoles()
		{
			List<Role> roles = new List<Role>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id, Name, Status FROM [Role]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								roles.Add(new Role
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									Status = reader.GetBoolean(reader.GetOrdinal("Status"))
								});
							}
						}
					}
				}
				return roles;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving roles.", ex);
			}
		}

		public void AddRole(Role role)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Role] (Id, Name, Status) VALUES (@Id, @Name, @Status)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", role.Id);
						cmd.Parameters.AddWithValue("@Name", role.Name);
						cmd.Parameters.AddWithValue("@Status", role.Status);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error adding role.", ex);
			}
		}

		public void UpdateRole(Role role)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [Role] SET Name = @Name, Status = @Status WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", role.Id);
						cmd.Parameters.AddWithValue("@Name", role.Name);
						cmd.Parameters.AddWithValue("@Status", role.Status);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating role.", ex);
			}
		}

		public void DeleteRole(string id)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Role] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error deleting role.", ex);
			}
		}

		public List<UserRole> GetUserRoles()
		{
			List<UserRole> userRoles = new List<UserRole>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id, IdStudent, IdRole FROM [UserRole]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								userRoles.Add(new UserRole
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									IdRole = reader.GetString(reader.GetOrdinal("IdRole"))
								});
							}
						}
					}
				}
				return userRoles;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving user roles.", ex);
			}
		}

		public void AddUserRole(UserRole userRole)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [UserRole] (Id, IdStudent, IdRole) VALUES (@Id, @IdStudent, @IdRole)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						//cmd.Image = new Image();
						cmd.Parameters.AddWithValue("@Id", userRole.Id);
						cmd.Parameters.AddWithValue("@IdStudent", userRole.IdStudent);
						cmd.Parameters.AddWithValue("@IdRole", userRole.IdRole);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error adding user role.", ex);
			}
		}

		public void UpdateUserRole(UserRole userRole)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [UserRole] SET IdStudent = @IdStudent, IdRole = @IdRole WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", userRole.Id);
						cmd.Parameters.AddWithValue("@IdStudent", userRole.IdStudent);
						cmd.Parameters.AddWithValue("@IdRole", userRole.IdRole);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error updating user role.", ex);
			}
		}

		public void DeleteUserRole(string id)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [UserRole] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error deleting user role.", ex);
			}
		}

		public List<Subject> GetAvailableSubjects(string username)
		{
			List<Subject> subjects = new List<Subject>();
			string studentId = GetStudentIdByUsername(username);
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT s.Id, s.Name 
                        FROM [Subject] s
                        WHERE s.Id NOT IN (
                            SELECT IdSubject 
                            FROM [Enrol] 
                            WHERE IdStudent = @IdStudent
                        )";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								subjects.Add(new Subject
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name"))
								});
							}
						}
					}
				}
				return subjects;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving available subjects.", ex);
			}
		}

		public void EnrollSubject(string username, string subjectId)
		{
			string studentId = GetStudentIdByUsername(username);
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Enrol] (IdStudent, IdSubject) VALUES (@IdStudent, @IdSubject)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						cmd.Parameters.AddWithValue("@IdSubject", subjectId);
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error enrolling subject.", ex);
			}
		}

		public List<Enrol> GetEnrolledSubjects(string username)
		{
			List<Enrol> enrolledSubjects = new List<Enrol>();
			string studentId = GetStudentIdByUsername(username);
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT e.IdStudent, e.IdSubject, s.Name, e.Mark
                        FROM [Enrol] e
                        JOIN [Subject] s ON e.IdSubject = s.Id
                        WHERE e.IdStudent = @IdStudent";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								enrolledSubjects.Add(new Enrol
								{
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									IdSubject = reader.GetString(reader.GetOrdinal("IdSubject")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									Mark = reader.IsDBNull(reader.GetOrdinal("Mark")) ? null : reader.GetDecimal(reader.GetOrdinal("Mark"))
								});
							}
						}
					}
				}
				return enrolledSubjects;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving enrolled subjects.", ex);
			}
		}

		public List<Enrol> GetAllEnrollments()
		{
			List<Enrol> enrollments = new List<Enrol>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT e.IdStudent, e.IdSubject, s.Name, e.Mark
                        FROM [Enrol] e
                        JOIN [Subject] s ON e.IdSubject = s.Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								enrollments.Add(new Enrol
								{
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									IdSubject = reader.GetString(reader.GetOrdinal("IdSubject")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									Mark = reader.IsDBNull(reader.GetOrdinal("Mark")) ? null : reader.GetDecimal(reader.GetOrdinal("Mark"))
								});
							}
						}
					}
				}
				return enrollments;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error retrieving all enrollments.", ex);
			}
		}

		public List<StudentGrade> SearchStudentGrades(string studentId)
		{
			List<StudentGrade> grades = new List<StudentGrade>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT s.Name AS StudentName, sub.Name AS SubjectName, e.Mark
                        FROM [Student] s
                        LEFT JOIN [Enrol] e ON s.Id = e.IdStudent
                        LEFT JOIN [Subject] sub ON e.IdSubject = sub.Id
                        WHERE s.Id = @StudentId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@StudentId", studentId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								grades.Add(new StudentGrade
								{
									StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
									SubjectName = reader.IsDBNull(reader.GetOrdinal("SubjectName")) ? null : reader.GetString(reader.GetOrdinal("SubjectName")),
									Mark = reader.IsDBNull(reader.GetOrdinal("Mark")) ? null : reader.GetDecimal(reader.GetOrdinal("Mark"))
								});
							}
						}
					}
				}
				return grades;
			}
			catch (SqlException ex)
			{
				throw new Exception("Error searching student grades.", ex);
			}
		}
	}
}