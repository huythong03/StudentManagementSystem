using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;
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
				Debug.WriteLine("GetConnection: Database connection opened successfully.");
				return conn;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetConnection: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Failed to connect to the database.", ex);
			}
		}

		public int GetTotalStudents()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT dbo.GetTotalStudents()";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						int total = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"GetTotalStudents: Total students = {total}");
						return total;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetTotalStudents: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving total students.", ex);
			}
		}

		public string GetNextRoleId()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id FROM [Role] WHERE Id LIKE 'R%'";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						List<int> numbers = new List<int>();
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								string id = reader.GetString(reader.GetOrdinal("Id"));
								if (id.StartsWith("R") && int.TryParse(id.Substring(1), out int number))
								{
									numbers.Add(number);
								}
							}
						}

						int nextNumber = numbers.Count > 0 ? numbers.Max() + 1 : 1;
						string nextId = $"R{nextNumber}";
						Debug.WriteLine($"GetNextRoleId: Generated ID = {nextId}");
						return nextId;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetNextRoleId: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error generating next role ID.", ex);
			}
		}

		public string GetNextUserRoleId()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id FROM [UserRole] WHERE Id LIKE 'UR%'";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						List<int> numbers = new List<int>();
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								string id = reader.GetString(reader.GetOrdinal("Id"));
								if (id.StartsWith("UR") && int.TryParse(id.Substring(2), out int number))
								{
									numbers.Add(number);
								}
							}
						}

						int nextNumber = numbers.Count > 0 ? numbers.Max() + 1 : 1;
						string nextId = $"UR{nextNumber.ToString("D4")}";
						Debug.WriteLine($"GetNextUserRoleId: Generated ID = {nextId}");
						return nextId;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetNextUserRoleId: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error generating next user role ID.", ex);
			}
		}

		public List<Student> GetStudentIds()
		{
			List<Student> students = new List<Student>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id FROM [Student] WHERE Id IS NOT NULL ORDER BY Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								string id = reader.GetString(reader.GetOrdinal("Id"));
								if (!string.IsNullOrWhiteSpace(id))
								{
									students.Add(new Student { Id = id });
								}
							}
						}
					}
				}
				if (students.Count == 0)
				{
					Debug.WriteLine("GetStudentIds: No valid student IDs found.");
					throw new Exception("No valid student IDs found in the database.");
				}
				Debug.WriteLine($"GetStudentIds: Found {students.Count} student IDs.");
				return students;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetStudentIds: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving student IDs.", ex);
			}
		}

		public List<User> GetUserStudentIds()
		{
			List<User> users = new List<User>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT IdStudent FROM [User] WHERE IdStudent IS NOT NULL ORDER BY IdStudent";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								string idStudent = reader.GetString(reader.GetOrdinal("IdStudent"));
								if (!string.IsNullOrWhiteSpace(idStudent))
								{
									users.Add(new User { IdStudent = idStudent });
								}
							}
						}
					}
				}
				if (users.Count == 0)
				{
					Debug.WriteLine("GetUserStudentIds: No valid IdStudent found in [User].");
					throw new Exception("No valid IdStudent found in the User table.");
				}
				Debug.WriteLine($"GetUserStudentIds: Found {users.Count} IdStudent.");
				return users;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetUserStudentIds: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving IdStudent from User table.", ex);
			}
		}

		public bool UserRoleExists(string idStudent, string idRole)
		{
			if (string.IsNullOrWhiteSpace(idStudent) || string.IsNullOrWhiteSpace(idRole))
			{
				Debug.WriteLine($"UserRoleExists: Invalid input (IdStudent='{idStudent}', IdRole='{idRole}')");
				return false;
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT COUNT(*) FROM [UserRole] WHERE IdStudent = @IdStudent AND IdRole = @IdRole";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", idStudent);
						cmd.Parameters.AddWithValue("@IdRole", idRole);
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"UserRoleExists: IdStudent={idStudent}, IdRole={idRole}, Exists={count > 0}");
						return count > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UserRoleExists: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error checking user role existence.", ex);
			}
		}

		public User GetUserByUsername(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine("GetUserByUsername: Invalid username.");
				return null;
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT IdStudent, Username, Password, Note, Status, CreatedAt, ModifiedAt 
                        FROM [User] 
                        WHERE Username = @Username";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								User user = new User
								{
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									Username = reader.GetString(reader.GetOrdinal("Username")),
									Password = reader.GetString(reader.GetOrdinal("Password")),
									Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
									Status = reader.GetBoolean(reader.GetOrdinal("Status")),
									CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
									ModifiedAt = reader.GetDateTime(reader.GetOrdinal("ModifiedAt"))
								};
								Debug.WriteLine($"GetUserByUsername: Found user (Username={username}, IdStudent={user.IdStudent})");
								return user;
							}
						}
					}
				}
				Debug.WriteLine($"GetUserByUsername: No user found for Username={username}");
				return null;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetUserByUsername: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving user by username.", ex);
			}
		}

		public bool ValidateLogin(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				Debug.WriteLine($"ValidateLogin: Invalid input (Username='{username}', Password='[Hidden]')");
				return false;
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username AND Password = @Password AND Status = 1";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						cmd.Parameters.AddWithValue("@Password", password);
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"ValidateLogin: Username={username}, Valid={count > 0}");
						return count > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"ValidateLogin: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error validating login credentials.", ex);
			}
		}

		public string GetUserRole(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine("GetUserRole: Invalid username.");
				return "user";
			}

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
						string role = cmd.ExecuteScalar()?.ToString() ?? "user";
						Debug.WriteLine($"GetUserRole: Username={username}, Role={role}");
						return role;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetUserRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving user role.", ex);
			}
		}

		public string GetStudentIdByUsername(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine("GetStudentIdByUsername: Invalid username.");
				return null;
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT IdStudent FROM [User] WHERE Username = @Username";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						string idStudent = cmd.ExecuteScalar()?.ToString();
						Debug.WriteLine($"GetStudentIdByUsername: Username={username}, IdStudent={idStudent ?? "null"}");
						return idStudent;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetStudentIdByUsername: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
				Debug.WriteLine($"GetStudents: Found {students.Count} students.");
				return students;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetStudents: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving students.", ex);
			}
		}

		public void AddStudent(Student student)
		{
			if (student == null || string.IsNullOrWhiteSpace(student.Id) || string.IsNullOrWhiteSpace(student.Name))
			{
				Debug.WriteLine($"AddStudent: Invalid student data (Id='{student?.Id}', Name='{student?.Name}')");
				throw new ArgumentException("Student data is invalid.");
			}

			DateTime today = DateTime.Today;
			int age = today.Year - student.BOF.Year;
			if (today < student.BOF.AddYears(age))
			{
				age--;
			}

			if (age < 15)
			{
				Debug.WriteLine($"AddStudent: Student is too young (Id='{student.Id}', Age={age})");
				throw new ArgumentException("Student must be at least 15 years old.");
			}

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
						Debug.WriteLine($"AddStudent: Inserting Id={student.Id}, Name={student.Name}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddStudent: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error adding student.", ex);
			}
		}

		public void UpdateStudent(Student student)
		{
			if (student == null || string.IsNullOrWhiteSpace(student.Id) || string.IsNullOrWhiteSpace(student.Name))
			{
				Debug.WriteLine($"UpdateStudent: Invalid student data (Id='{student?.Id}', Name='{student?.Name}')");
				throw new ArgumentException("Student data is invalid.");
			}

			DateTime today = DateTime.Today;
			int age = today.Year - student.BOF.Year;
			if (today < student.BOF.AddYears(age))
			{
				age--;
			}

			if (age < 15)
			{
				Debug.WriteLine($"AddStudent: Student is too young (Id='{student.Id}', Age={age})");
				throw new ArgumentException("Student must be at least 15 years old.");
			}

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
						Debug.WriteLine($"UpdateStudent: Updating Id={student.Id}, Name={student.Name}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateStudent: No rows affected for Id={student.Id}");
							throw new Exception("Failed to update student. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateStudent: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error updating student.", ex);
			}
		}

		public void DeleteStudent(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.WriteLine("DeleteStudent: Invalid Id.");
				throw new ArgumentException("Student ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Student] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						Debug.WriteLine($"DeleteStudent: Deleting Id={id}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteStudent: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
				Debug.WriteLine($"GetProvinces: Found {provinces.Count} provinces.");
				return provinces;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetProvinces: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving provinces.", ex);
			}
		}

		public void AddProvince(Province province)
		{
			if (province == null || string.IsNullOrWhiteSpace(province.Name))
			{
				Debug.WriteLine($"AddProvince: Invalid province data (Id={province?.Id}, Name='{province?.Name}')");
				throw new ArgumentException("Province data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Province] (Id, Name) VALUES (@Id, @Name)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", province.Id);
						cmd.Parameters.AddWithValue("@Name", province.Name);
						Debug.WriteLine($"AddProvince: Inserting Id={province.Id}, Name={province.Name}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddProvince: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error adding province.", ex);
			}
		}

		public void UpdateProvince(Province province)
		{
			if (province == null || string.IsNullOrWhiteSpace(province.Name))
			{
				Debug.WriteLine($"UpdateProvince: Invalid province data (Id={province?.Id}, Name='{province?.Name}')");
				throw new ArgumentException("Province data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [Province] SET Name = @Name WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", province.Id);
						cmd.Parameters.AddWithValue("@Name", province.Name);
						Debug.WriteLine($"UpdateProvince: Updating Id={province.Id}, Name={province.Name}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateProvince: No rows affected for Id={province.Id}");
							throw new Exception("Failed to update province. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateProvince: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
						Debug.WriteLine($"DeleteProvince: Deleting Id={id}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteProvince: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
				Debug.WriteLine($"GetSubjects: Found {subjects.Count} subjects.");
				return subjects;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetSubjects: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving subjects.", ex);
			}
		}

		public void AddSubject(Subject subject)
		{
			if (subject == null || string.IsNullOrWhiteSpace(subject.Id) || string.IsNullOrWhiteSpace(subject.Name))
			{
				Debug.WriteLine($"AddSubject: Invalid subject data (Id='{subject?.Id}', Name='{subject?.Name}')");
				throw new ArgumentException("Subject data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Subject] (Id, Name) VALUES (@Id, @Name)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", subject.Id);
						cmd.Parameters.AddWithValue("@Name", subject.Name);
						Debug.WriteLine($"AddSubject: Inserting Id={subject.Id}, Name={subject.Name}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddSubject: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error adding subject.", ex);
			}
		}

		public void UpdateSubject(Subject subject)
		{
			if (subject == null || string.IsNullOrWhiteSpace(subject.Id) || string.IsNullOrWhiteSpace(subject.Name))
			{
				Debug.WriteLine($"UpdateSubject: Invalid subject data (Id='{subject?.Id}', Name='{subject?.Name}')");
				throw new ArgumentException("Subject data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [Subject] SET Name = @Name WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", subject.Id);
						cmd.Parameters.AddWithValue("@Name", subject.Name);
						Debug.WriteLine($"UpdateSubject: Updating Id={subject.Id}, Name={subject.Name}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateSubject: No rows affected for Id={subject.Id}");
							throw new Exception("Failed to update subject. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateSubject: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error updating subject.", ex);
			}
		}

		public void DeleteSubject(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.WriteLine("DeleteSubject: Invalid Id.");
				throw new ArgumentException("Subject ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Subject] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						Debug.WriteLine($"DeleteSubject: Deleting Id={id}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteSubject: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
				Debug.WriteLine($"GetUsers: Found {users.Count} users.");
				return users;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetUsers: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving users.", ex);
			}
		}

		public void AddUser(User user)
		{
			if (user == null || string.IsNullOrWhiteSpace(user.IdStudent) || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
			{
				Debug.WriteLine($"AddUser: Invalid user data (IdStudent='{user?.IdStudent}', Username='{user?.Username}')");
				throw new ArgumentException("User data is invalid.");
			}

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
						Debug.WriteLine($"AddUser: Inserting IdStudent={user.IdStudent}, Username={user.Username}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddUser: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error adding user.", ex);
			}
		}

		public void UpdateUser(User user)
		{
			if (user == null || string.IsNullOrWhiteSpace(user.IdStudent) || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
			{
				Debug.WriteLine($"UpdateUser: Invalid user data (IdStudent='{user?.IdStudent}', Username='{user?.Username}')");
				throw new ArgumentException("User data is invalid.");
			}

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
						Debug.WriteLine($"UpdateUser: Updating IdStudent={user.IdStudent}, Username={user.Username}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateUser: No rows affected for IdStudent={user.IdStudent}");
							throw new Exception("Failed to update user. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateUser: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error updating user.", ex);
			}
		}

		public void DeleteUser(string idStudent)
		{
			if (string.IsNullOrWhiteSpace(idStudent))
			{
				Debug.WriteLine("DeleteUser: Invalid IdStudent.");
				throw new ArgumentException("User ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [User] WHERE IdStudent = @IdStudent";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", idStudent);
						Debug.WriteLine($"DeleteUser: Deleting IdStudent={idStudent}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteUser: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error deleting user.", ex);
			}
		}

		public void UpdateUserPassword(string username, string newPassword)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
			{
				Debug.WriteLine($"UpdateUserPassword: Invalid input (Username='{username}', Password='[Hidden]')");
				throw new ArgumentException("Username or password is invalid.");
			}

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
						Debug.WriteLine($"UpdateUserPassword: Updating password for Username={username}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateUserPassword: No rows affected for Username={username}");
							throw new Exception("Failed to update password. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateUserPassword: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
					string query = @"
                        SELECT Id, Name, Status 
                        FROM [Role] 
                        WHERE Id LIKE 'R%' 
                        ORDER BY CAST(SUBSTRING(Id, 2, LEN(Id)) AS INT)";
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
				Debug.WriteLine($"GetRoles: Found {roles.Count} roles.");
				return roles;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetRoles: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving roles.", ex);
			}
		}

		public void AddRole(Role role)
		{
			if (role == null || string.IsNullOrWhiteSpace(role.Id) || string.IsNullOrWhiteSpace(role.Name))
			{
				Debug.WriteLine($"AddRole: Invalid role data (Id='{role?.Id}', Name='{role?.Name}')");
				throw new ArgumentException("Role data is invalid.");
			}

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
						Debug.WriteLine($"AddRole: Inserting Id={role.Id}, Name={role.Name}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error adding role.", ex);
			}
		}

		public void UpdateRole(Role role)
		{
			if (role == null || string.IsNullOrWhiteSpace(role.Id) || string.IsNullOrWhiteSpace(role.Name))
			{
				Debug.WriteLine($"UpdateRole: Invalid role data (Id='{role?.Id}', Name='{role?.Name}')");
				throw new ArgumentException("Role data is invalid.");
			}

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
						Debug.WriteLine($"UpdateRole: Updating Id={role.Id}, Name={role.Name}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateRole: No rows affected for Id={role.Id}");
							throw new Exception("Failed to update role. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error updating role.", ex);
			}
		}

		public void DeleteRole(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.WriteLine("DeleteRole: Invalid Id.");
				throw new ArgumentException("Role ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Role] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						Debug.WriteLine($"DeleteRole: Deleting Id={id}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
					string query = "SELECT Id, IdStudent, IdRole FROM [UserRole] ORDER BY CAST(SUBSTRING(Id, 3, LEN(Id)) AS INT)";
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
				Debug.WriteLine($"GetUserRoles: Found {userRoles.Count} user roles.");
				return userRoles;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetUserRoles: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving user roles.", ex);
			}
		}

		public void AddUserRole(UserRole userRole)
		{
			if (userRole == null || string.IsNullOrWhiteSpace(userRole.Id) || string.IsNullOrWhiteSpace(userRole.IdStudent) || string.IsNullOrWhiteSpace(userRole.IdRole))
			{
				Debug.WriteLine($"AddUserRole: Invalid UserRole (Id='{userRole?.Id}', IdStudent='{userRole?.IdStudent}', IdRole='{userRole?.IdRole}')");
				throw new ArgumentException("UserRole data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [UserRole] (Id, IdStudent, IdRole) VALUES (@Id, @IdStudent, @IdRole)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", userRole.Id);
						cmd.Parameters.AddWithValue("@IdStudent", userRole.IdStudent);
						cmd.Parameters.AddWithValue("@IdRole", userRole.IdRole);
						Debug.WriteLine($"AddUserRole: Inserting Id={userRole.Id}, IdStudent={userRole.IdStudent}, IdRole={userRole.IdRole}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine("AddUserRole: No rows affected.");
							throw new Exception("Failed to insert user role.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddUserRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Error adding user role: {ex.Message}", ex);
			}
		}

		public void UpdateUserRole(UserRole userRole)
		{
			if (userRole == null || string.IsNullOrWhiteSpace(userRole.Id) || string.IsNullOrWhiteSpace(userRole.IdStudent) || string.IsNullOrWhiteSpace(userRole.IdRole))
			{
				Debug.WriteLine($"UpdateUserRole: Invalid UserRole (Id='{userRole?.Id}', IdStudent='{userRole?.IdStudent}', IdRole='{userRole?.IdRole}')");
				throw new ArgumentException("UserRole data is invalid.");
			}

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
						Debug.WriteLine($"UpdateUserRole: Updating Id={userRole.Id}, IdStudent={userRole.IdStudent}, IdRole={userRole.IdRole}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateUserRole: No rows affected for Id={userRole.Id}");
							throw new Exception("Failed to update user role. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateUserRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Error updating user role: {ex.Message}", ex);
			}
		}

		public void DeleteUserRole(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.WriteLine("DeleteUserRole: Invalid Id.");
				throw new ArgumentException("UserRole ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [UserRole] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						Debug.WriteLine($"DeleteUserRole: Deleting Id={id}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteUserRole: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error deleting user role.", ex);
			}
		}

		public List<Subject> GetAvailableSubjects(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine("GetAvailableSubjects: Invalid username.");
				return new List<Subject>();
			}

			List<Subject> subjects = new List<Subject>();
			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"GetAvailableSubjects: No IdStudent found for Username={username}");
				return subjects;
			}

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
				Debug.WriteLine($"GetAvailableSubjects: Found {subjects.Count} available subjects for IdStudent={studentId}");
				return subjects;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetAvailableSubjects: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving available subjects.", ex);
			}
		}

		public void EnrollSubject(string username, string subjectId)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(subjectId))
			{
				Debug.WriteLine($"EnrollSubject: Invalid input (Username='{username}', SubjectId='{subjectId}')");
				throw new ArgumentException("Username or Subject ID is invalid.");
			}

			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"EnrollSubject: No IdStudent found for Username={username}");
				throw new Exception("Student ID not found for the given username.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [Enrol] (IdStudent, IdSubject) VALUES (@IdStudent, @IdSubject)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						cmd.Parameters.AddWithValue("@IdSubject", subjectId);
						Debug.WriteLine($"EnrollSubject: Inserting IdStudent={studentId}, IdSubject={subjectId}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"EnrollSubject: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error enrolling subject.", ex);
			}
		}

		public List<Enrol> GetEnrolledSubjects(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine("GetEnrolledSubjects: Invalid username.");
				return new List<Enrol>();
			}

			List<Enrol> enrolledSubjects = new List<Enrol>();
			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"GetEnrolledSubjects: No IdStudent found for Username={username}");
				return enrolledSubjects;
			}

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
				Debug.WriteLine($"GetEnrolledSubjects: Found {enrolledSubjects.Count} enrolled subjects for IdStudent={studentId}");
				return enrolledSubjects;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetEnrolledSubjects: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
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
                        SELECT e.IdStudent, e.IdSubject, s.Name AS SubjectName, st.Name AS StudentName, e.Mark
                        FROM [Enrol] e
                        JOIN [Subject] s ON e.IdSubject = s.Id
                        JOIN [Student] st ON e.IdStudent = st.Id";
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
									Name = reader.GetString(reader.GetOrdinal("SubjectName")),
									StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
									Mark = reader.IsDBNull(reader.GetOrdinal("Mark")) ? null : reader.GetDecimal(reader.GetOrdinal("Mark"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetAllEnrollments: Found {enrollments.Count} enrollments.");
				return enrollments;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetAllEnrollments: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving all enrollments.", ex);
			}
		}

		public List<StudentGrade> SearchStudentGrades(string searchValue)
		{
			if (string.IsNullOrWhiteSpace(searchValue))
			{
				Debug.WriteLine("SearchStudentGrades: Invalid searchValue.");
				return new List<StudentGrade>();
			}

			List<StudentGrade> grades = new List<StudentGrade>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT s.Id, s.Name AS StudentName, sub.Name AS SubjectName, e.Mark
                        FROM [Student] s
                        LEFT JOIN [Enrol] e ON s.Id = e.IdStudent
                        LEFT JOIN [Subject] sub ON e.IdSubject = sub.Id
                        WHERE s.Name LIKE '%' + @SearchValue + '%' OR s.Id = @SearchValue";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@SearchValue", searchValue);
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
				Debug.WriteLine($"SearchStudentGrades: Found {grades.Count} grades for SearchValue='{searchValue}'");
				return grades;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"SearchStudentGrades: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error searching student grades.", ex);
			}
		}

		public List<EnrollRequest> GetPendingEnrollRequests()
		{
			List<EnrollRequest> requests = new List<EnrollRequest>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                SELECT er.RequestId, er.IdStudent, s.Name AS StudentName, 
                       er.IdSubject, sub.Name AS SubjectName, er.RequestDate, er.Status
                FROM [EnrollRequests] er
                JOIN [Student] s ON er.IdStudent = s.Id
                JOIN [Subject] sub ON er.IdSubject = sub.Id
                WHERE er.Status = 'Pending'
                ORDER BY er.RequestDate";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								requests.Add(new EnrollRequest
								{
									RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
									IdSubject = reader.GetString(reader.GetOrdinal("IdSubject")),
									SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
									RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
									Status = reader.GetString(reader.GetOrdinal("Status"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetPendingEnrollRequests: Found {requests.Count} pending requests.");
				return requests;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetPendingEnrollRequests: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving pending enroll requests.", ex);
			}
		}

		public List<EnrollRequest> GetUserEnrollRequests(string username)
		{
			List<EnrollRequest> requests = new List<EnrollRequest>();
			string studentId = GetStudentIdByUsername(username);

			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"GetUserEnrollRequests: No IdStudent found for Username={username}");
				return requests;
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                SELECT er.RequestId, er.IdStudent, s.Name AS StudentName, 
                       er.IdSubject, sub.Name AS SubjectName, er.RequestDate, er.Status
                FROM [EnrollRequests] er
                JOIN [Student] s ON er.IdStudent = s.Id
                JOIN [Subject] sub ON er.IdSubject = sub.Id
                WHERE er.IdStudent = @IdStudent
                ORDER BY er.RequestDate DESC";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								requests.Add(new EnrollRequest
								{
									RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
									IdStudent = reader.GetString(reader.GetOrdinal("IdStudent")),
									StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
									IdSubject = reader.GetString(reader.GetOrdinal("IdSubject")),
									SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
									RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
									Status = reader.GetString(reader.GetOrdinal("Status"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetUserEnrollRequests: Found {requests.Count} requests for IdStudent={studentId}");
				return requests;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetUserEnrollRequests: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving user enroll requests.", ex);
			}
		}

		public int CreateEnrollRequest(string username, string subjectId)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(subjectId))
			{
				Debug.WriteLine($"CreateEnrollRequest: Invalid input (Username='{username}', SubjectId='{subjectId}')");
				throw new ArgumentException("Username or Subject ID is invalid.");
			}

			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"CreateEnrollRequest: No IdStudent found for Username={username}");
				throw new Exception("Student ID not found for the given username.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					// Kiểm tra xem yêu cầu đã tồn tại chưa
					string checkQuery = "SELECT COUNT(*) FROM [EnrollRequests] WHERE IdStudent = @IdStudent AND IdSubject = @IdSubject AND Status = 'Pending'";
					using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
					{
						checkCmd.Parameters.AddWithValue("@IdStudent", studentId);
						checkCmd.Parameters.AddWithValue("@IdSubject", subjectId);
						int existingCount = (int)checkCmd.ExecuteScalar();

						if (existingCount > 0)
						{
							Debug.WriteLine($"CreateEnrollRequest: Request already exists for IdStudent={studentId}, IdSubject={subjectId}");
							throw new Exception("You already have a pending request for this subject.");
						}
					}

					// Tạo yêu cầu mới
					string insertQuery = @"
                INSERT INTO [EnrollRequests] (IdStudent, IdSubject, Status)
                VALUES (@IdStudent, @IdSubject, 'Pending');
                SELECT SCOPE_IDENTITY();";

					using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						cmd.Parameters.AddWithValue("@IdSubject", subjectId);

						int requestId = Convert.ToInt32(cmd.ExecuteScalar());
						Debug.WriteLine($"CreateEnrollRequest: Created request with ID={requestId} for IdStudent={studentId}, IdSubject={subjectId}");
						return requestId;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"CreateEnrollRequest: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error creating enroll request.", ex);
			}
		}

		public bool CancelEnrollRequest(int requestId, string username)
		{
			if (requestId <= 0 || string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine($"CancelEnrollRequest: Invalid input (RequestId={requestId}, Username='{username}')");
				return false;
			}

			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"CancelEnrollRequest: No IdStudent found for Username={username}");
				return false;
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                DELETE FROM [EnrollRequests] 
                WHERE RequestId = @RequestId 
                AND IdStudent = @IdStudent 
                AND Status = 'Pending'";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@RequestId", requestId);
						cmd.Parameters.AddWithValue("@IdStudent", studentId);

						int rowsAffected = cmd.ExecuteNonQuery();
						bool success = rowsAffected > 0;
						Debug.WriteLine($"CancelEnrollRequest: RequestId={requestId}, Success={success}");
						return success;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"CancelEnrollRequest: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error canceling enroll request.", ex);
			}
		}

		public bool ApproveEnrollRequest(int requestId)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					// Bắt đầu transaction
					using (SqlTransaction transaction = conn.BeginTransaction())
					{
						try
						{
							// Lấy thông tin yêu cầu
							string getRequestQuery = @"
                        SELECT IdStudent, IdSubject 
                        FROM [EnrollRequests] 
                        WHERE RequestId = @RequestId 
                        AND Status = 'Pending'";

							string studentId = null;
							string subjectId = null;

							using (SqlCommand getCmd = new SqlCommand(getRequestQuery, conn, transaction))
							{
								getCmd.Parameters.AddWithValue("@RequestId", requestId);
								using (SqlDataReader reader = getCmd.ExecuteReader())
								{
									if (reader.Read())
									{
										studentId = reader.GetString(reader.GetOrdinal("IdStudent"));
										subjectId = reader.GetString(reader.GetOrdinal("IdSubject"));
									}
								}
							}

							if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(subjectId))
							{
								Debug.WriteLine($"ApproveEnrollRequest: Request not found or already processed (RequestId={requestId})");
								transaction.Rollback();
								return false;
							}

							// Kiểm tra xem sinh viên đã đăng ký môn này chưa
							string checkEnrolQuery = "SELECT COUNT(*) FROM [Enrol] WHERE IdStudent = @IdStudent AND IdSubject = @IdSubject";
							using (SqlCommand checkCmd = new SqlCommand(checkEnrolQuery, conn, transaction))
							{
								checkCmd.Parameters.AddWithValue("@IdStudent", studentId);
								checkCmd.Parameters.AddWithValue("@IdSubject", subjectId);
								int count = (int)checkCmd.ExecuteScalar();

								if (count > 0)
								{
									Debug.WriteLine($"ApproveEnrollRequest: Student already enrolled (IdStudent={studentId}, IdSubject={subjectId})");
									transaction.Rollback();
									return false;
								}
							}

							// Thêm vào bảng Enrol
							string insertEnrolQuery = "INSERT INTO [Enrol] (IdStudent, IdSubject) VALUES (@IdStudent, @IdSubject)";
							using (SqlCommand insertCmd = new SqlCommand(insertEnrolQuery, conn, transaction))
							{
								insertCmd.Parameters.AddWithValue("@IdStudent", studentId);
								insertCmd.Parameters.AddWithValue("@IdSubject", subjectId);
								insertCmd.ExecuteNonQuery();
							}

							// Cập nhật trạng thái yêu cầu
							string updateRequestQuery = "UPDATE [EnrollRequests] SET Status = 'Approved' WHERE RequestId = @RequestId";
							using (SqlCommand updateCmd = new SqlCommand(updateRequestQuery, conn, transaction))
							{
								updateCmd.Parameters.AddWithValue("@RequestId", requestId);
								updateCmd.ExecuteNonQuery();
							}

							// Commit transaction
							transaction.Commit();
							Debug.WriteLine($"ApproveEnrollRequest: Approved request (RequestId={requestId})");
							return true;
						}
						catch
						{
							transaction.Rollback();
							throw;
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"ApproveEnrollRequest: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error approving enroll request.", ex);
			}
		}

		public bool RejectEnrollRequest(int requestId)
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "UPDATE [EnrollRequests] SET Status = 'Rejected' WHERE RequestId = @RequestId AND Status = 'Pending'";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@RequestId", requestId);
						int rowsAffected = cmd.ExecuteNonQuery();
						bool success = rowsAffected > 0;
						Debug.WriteLine($"RejectEnrollRequest: RequestId={requestId}, Success={success}");
						return success;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"RejectEnrollRequest: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error rejecting enroll request.", ex);
			}
		}

		public int GetPendingEnrollmentRequestsCount()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT COUNT(*) FROM [EnrollRequests] WHERE Status = 'Pending'";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"GetPendingEnrollmentRequestsCount: Pending requests = {count}");
						return count;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetPendingEnrollmentRequestsCount: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving pending enrollment requests count.", ex);
			}
		}

		public int GetTotalEnrollments()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT COUNT(*) FROM [Enrol]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"GetTotalEnrollments: Total enrollments = {count}");
						return count;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetTotalEnrollments: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving total enrollments.", ex);
			}
		}

		public decimal GetAverageGrade()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT AVG(CAST(Mark AS DECIMAL(4,2))) FROM [Enrol] WHERE Mark IS NOT NULL";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						object result = cmd.ExecuteScalar();
						decimal average = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
						Debug.WriteLine($"GetAverageGrade: Average grade = {average}");
						return Math.Round(average, 1);
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetAverageGrade: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving average grade.", ex);
			}
		}

		public int GetActiveCourses()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT COUNT(*) FROM [Subject]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"GetActiveCourses: Active courses = {count}");
						return count;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetActiveCourses: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving active courses.", ex);
			}
		}

		public int GetNewStudentsThisYear()
		{
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT COUNT(*) 
                        FROM [Student] 
                        WHERE YEAR(CreatedAt) = YEAR(GETDATE())";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"GetNewStudentsThisYear: New students = {count}");
						return count;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetNewStudentsThisYear: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving new students count.", ex);
			}
		}

		// Class Management
		public List<Class> GetClasses()
		{
			List<Class> classes = new List<Class>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "SELECT Id, Name, SubjectId, StartDate, EndDate FROM [Class]";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								classes.Add(new Class
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									SubjectId = reader.GetString(reader.GetOrdinal("SubjectId")),
									StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
									EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetClasses: Found {classes.Count} classes.");
				return classes;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetClasses: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving classes.", ex);
			}
		}

		public void AddClass(Class classObj)
		{
			if (classObj == null || string.IsNullOrWhiteSpace(classObj.Id) || string.IsNullOrWhiteSpace(classObj.Name) || string.IsNullOrWhiteSpace(classObj.SubjectId))
			{
				Debug.WriteLine($"AddClass: Invalid class data (Id='{classObj?.Id}', Name='{classObj?.Name}', SubjectId='{classObj?.SubjectId}')");
				throw new ArgumentException("Class data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        INSERT INTO [Class] (Id, Name, SubjectId, StartDate, EndDate)
                        VALUES (@Id, @Name, @SubjectId, @StartDate, @EndDate)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", classObj.Id);
						cmd.Parameters.AddWithValue("@Name", classObj.Name);
						cmd.Parameters.AddWithValue("@SubjectId", classObj.SubjectId);
						cmd.Parameters.AddWithValue("@StartDate", classObj.StartDate);
						cmd.Parameters.AddWithValue("@EndDate", classObj.EndDate);
						Debug.WriteLine($"AddClass: Inserting Id={classObj.Id}, Name={classObj.Name}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AddClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error adding class.", ex);
			}
		}

		public void UpdateClass(Class classObj)
		{
			if (classObj == null || string.IsNullOrWhiteSpace(classObj.Id) || string.IsNullOrWhiteSpace(classObj.Name) || string.IsNullOrWhiteSpace(classObj.SubjectId))
			{
				Debug.WriteLine($"UpdateClass: Invalid class data (Id='{classObj?.Id}', Name='{classObj?.Name}', SubjectId='{classObj?.SubjectId}')");
				throw new ArgumentException("Class data is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        UPDATE [Class]
                        SET Name = @Name, SubjectId = @SubjectId, StartDate = @StartDate, EndDate = @EndDate
                        WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", classObj.Id);
						cmd.Parameters.AddWithValue("@Name", classObj.Name);
						cmd.Parameters.AddWithValue("@SubjectId", classObj.SubjectId);
						cmd.Parameters.AddWithValue("@StartDate", classObj.StartDate);
						cmd.Parameters.AddWithValue("@EndDate", classObj.EndDate);
						Debug.WriteLine($"UpdateClass: Updating Id={classObj.Id}, Name={classObj.Name}");
						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected == 0)
						{
							Debug.WriteLine($"UpdateClass: No rows affected for Id={classObj.Id}");
							throw new Exception("Failed to update class. No matching record found.");
						}
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"UpdateClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error updating class.", ex);
			}
		}

		public void DeleteClass(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.WriteLine("DeleteClass: Invalid Id.");
				throw new ArgumentException("Class ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [Class] WHERE Id = @Id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Id", id);
						Debug.WriteLine($"DeleteClass: Deleting Id={id}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"DeleteClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error deleting class.", ex);
			}
		}

		// Teacher Management
		public List<Teacher> GetTeachers()
		{
			var teachers = new List<Teacher>();
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var command = new SqlCommand("SELECT * FROM Teacher", connection);
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					teachers.Add(new Teacher
					{
						Id = reader["Id"].ToString(),
						Name = reader["Name"].ToString(),
						Email = reader["Email"].ToString(),
						Major = reader["Major"] == DBNull.Value ? null : reader["Major"].ToString(),
						ProfessionalQualification = reader["ProfessionalQualification"] == DBNull.Value ? null : reader["ProfessionalQualification"].ToString(),
						Gender = reader["Gender"] == DBNull.Value ? null : (bool?)reader["Gender"],
						Ethnicity = reader["Ethnicity"] == DBNull.Value ? null : reader["Ethnicity"].ToString(),
						PartyMember = reader["PartyMember"] == DBNull.Value ? null : (bool?)reader["PartyMember"],
						ForeignLanguageLevel = reader["ForeignLanguageLevel"] == DBNull.Value ? null : reader["ForeignLanguageLevel"].ToString(),
						ITLevel = reader["ITLevel"] == DBNull.Value ? null : reader["ITLevel"].ToString()
					});
				}
			}
			return teachers;
		}

		public void AddTeacher(Teacher teacher)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var command = new SqlCommand(
					"INSERT INTO Teacher (Id, Name, Email, Major, ProfessionalQualification, Gender, Ethnicity, PartyMember, ForeignLanguageLevel, ITLevel) " +
					"VALUES (@Id, @Name, @Email, @Major, @ProfessionalQualification, @Gender, @Ethnicity, @PartyMember, @ForeignLanguageLevel, @ITLevel)", connection);
				command.Parameters.AddWithValue("@Id", teacher.Id);
				command.Parameters.AddWithValue("@Name", teacher.Name);
				command.Parameters.AddWithValue("@Email", teacher.Email);
				command.Parameters.AddWithValue("@Major", (object)teacher.Major ?? DBNull.Value);
				command.Parameters.AddWithValue("@ProfessionalQualification", (object)teacher.ProfessionalQualification ?? DBNull.Value);
				command.Parameters.AddWithValue("@Gender", (object)teacher.Gender ?? DBNull.Value);
				command.Parameters.AddWithValue("@Ethnicity", (object)teacher.Ethnicity ?? DBNull.Value);
				command.Parameters.AddWithValue("@PartyMember", (object)teacher.PartyMember ?? DBNull.Value);
				command.Parameters.AddWithValue("@ForeignLanguageLevel", (object)teacher.ForeignLanguageLevel ?? DBNull.Value);
				command.Parameters.AddWithValue("@ITLevel", (object)teacher.ITLevel ?? DBNull.Value);
				command.ExecuteNonQuery();
			}
		}

		public void UpdateTeacher(Teacher teacher)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var command = new SqlCommand(
					"UPDATE Teacher SET Name = @Name, Email = @Email, Major = @Major, ProfessionalQualification = @ProfessionalQualification, " +
					"Gender = @Gender, Ethnicity = @Ethnicity, PartyMember = @PartyMember, " +
					"ForeignLanguageLevel = @ForeignLanguageLevel, ITLevel = @ITLevel " +
					"WHERE Id = @Id", connection);
				command.Parameters.AddWithValue("@Id", teacher.Id);
				command.Parameters.AddWithValue("@Name", teacher.Name);
				command.Parameters.AddWithValue("@Email", teacher.Email);
				command.Parameters.AddWithValue("@Major", (object)teacher.Major ?? DBNull.Value);
				command.Parameters.AddWithValue("@ProfessionalQualification", (object)teacher.ProfessionalQualification ?? DBNull.Value);
				command.Parameters.AddWithValue("@Gender", (object)teacher.Gender ?? DBNull.Value);
				command.Parameters.AddWithValue("@Ethnicity", (object)teacher.Ethnicity ?? DBNull.Value);
				command.Parameters.AddWithValue("@PartyMember", (object)teacher.PartyMember ?? DBNull.Value);
				command.Parameters.AddWithValue("@ForeignLanguageLevel", (object)teacher.ForeignLanguageLevel ?? DBNull.Value);
				command.Parameters.AddWithValue("@ITLevel", (object)teacher.ITLevel ?? DBNull.Value);
				command.ExecuteNonQuery();
			}
		}

		public void DeleteTeacher(string teacherId)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var command = new SqlCommand("DELETE FROM Teacher WHERE Id = @Id", connection);
				command.Parameters.AddWithValue("@Id", teacherId);
				command.ExecuteNonQuery();
			}
		}

		// Teacher-Class Assignment
		public void AssignTeacherToClass(string teacherId, string classId)
		{
			if (string.IsNullOrWhiteSpace(teacherId) || string.IsNullOrWhiteSpace(classId))
			{
				Debug.WriteLine($"AssignTeacherToClass: Invalid input (TeacherId='{teacherId}', ClassId='{classId}')");
				throw new ArgumentException("Teacher ID or Class ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "INSERT INTO [TeacherClass] (TeacherId, ClassId) VALUES (@TeacherId, @ClassId)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TeacherId", teacherId);
						cmd.Parameters.AddWithValue("@ClassId", classId);
						Debug.WriteLine($"AssignTeacherToClass: Assigning TeacherId={teacherId} to ClassId={classId}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"AssignTeacherToClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error assigning teacher to class.", ex);
			}
		}

		public void RemoveTeacherFromClass(string teacherId, string classId)
		{
			if (string.IsNullOrWhiteSpace(teacherId) || string.IsNullOrWhiteSpace(classId))
			{
				Debug.WriteLine($"RemoveTeacherFromClass: Invalid input (TeacherId='{teacherId}', ClassId='{classId}')");
				throw new ArgumentException("Teacher ID or Class ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [TeacherClass] WHERE TeacherId = @TeacherId AND ClassId = @ClassId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TeacherId", teacherId);
						cmd.Parameters.AddWithValue("@ClassId", classId);
						Debug.WriteLine($"RemoveTeacherFromClass: Removing TeacherId={teacherId} from ClassId={classId}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"RemoveTeacherFromClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error removing teacher from class.", ex);
			}
		}

		public List<Class> GetClassesByTeacher(string teacherId)
		{
			if (string.IsNullOrWhiteSpace(teacherId))
			{
				Debug.WriteLine("GetClassesByTeacher: Invalid TeacherId.");
				return new List<Class>();
			}

			List<Class> classes = new List<Class>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT c.Id, c.Name, c.SubjectId, c.StartDate, c.EndDate
                        FROM [Class] c
                        JOIN [TeacherClass] tc ON c.Id = tc.ClassId
                        WHERE tc.TeacherId = @TeacherId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TeacherId", teacherId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								classes.Add(new Class
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									SubjectId = reader.GetString(reader.GetOrdinal("SubjectId")),
									StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
									EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetClassesByTeacher: Found {classes.Count} classes for TeacherId={teacherId}");
				return classes;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetClassesByTeacher: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving classes for teacher.", ex);
			}
		}

		// Student-Class Enrollment
		public void EnrollStudentInClass(string username, string classId)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(classId))
			{
				Debug.WriteLine($"EnrollStudentInClass: Invalid input (Username='{username}', ClassId='{classId}')");
				throw new ArgumentException("Username or Class ID is invalid.");
			}

			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"EnrollStudentInClass: No IdStudent found for Username={username}");
				throw new Exception("Student ID not found for the given username.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					// Get the SubjectId for the class
					string subjectQuery = "SELECT SubjectId FROM [Class] WHERE Id = @ClassId";
					string subjectId;
					using (SqlCommand cmd = new SqlCommand(subjectQuery, conn))
					{
						cmd.Parameters.AddWithValue("@ClassId", classId);
						subjectId = cmd.ExecuteScalar()?.ToString();
						if (string.IsNullOrWhiteSpace(subjectId))
						{
							Debug.WriteLine($"EnrollStudentInClass: No SubjectId found for ClassId={classId}");
							throw new Exception("Class not found.");
						}
					}

					// Check if student is already enrolled in the subject
					string checkEnrolQuery = "SELECT COUNT(*) FROM [Enrol] WHERE IdStudent = @IdStudent AND IdSubject = @IdSubject";
					using (SqlCommand cmd = new SqlCommand(checkEnrolQuery, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						cmd.Parameters.AddWithValue("@IdSubject", subjectId);
						int count = (int)cmd.ExecuteScalar();
						if (count == 0)
						{
							// Enroll in subject if not already enrolled
							string enrolQuery = "INSERT INTO [Enrol] (IdStudent, IdSubject) VALUES (@IdStudent, @IdSubject)";
							using (SqlCommand enrolCmd = new SqlCommand(enrolQuery, conn))
							{
								enrolCmd.Parameters.AddWithValue("@IdStudent", studentId);
								enrolCmd.Parameters.AddWithValue("@IdSubject", subjectId);
								Debug.WriteLine($"EnrollStudentInClass: Enrolling IdStudent={studentId} in IdSubject={subjectId}");
								enrolCmd.ExecuteNonQuery();
							}
						}
					}

					// Enroll in class
					string query = "INSERT INTO [ClassEnrollment] (IdStudent, ClassId) VALUES (@IdStudent, @ClassId)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						cmd.Parameters.AddWithValue("@ClassId", classId);
						Debug.WriteLine($"EnrollStudentInClass: Enrolling IdStudent={studentId} in ClassId={classId}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"EnrollStudentInClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error enrolling student in class.", ex);
			}
		}

		public void RemoveStudentFromClass(string username, string classId)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(classId))
			{
				Debug.WriteLine($"RemoveStudentFromClass: Invalid input (Username='{username}', ClassId='{classId}')");
				throw new ArgumentException("Username or Class ID is invalid.");
			}

			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"RemoveStudentFromClass: No IdStudent found for Username={username}");
				throw new Exception("Student ID not found for the given username.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = "DELETE FROM [ClassEnrollment] WHERE IdStudent = @IdStudent AND ClassId = @ClassId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						cmd.Parameters.AddWithValue("@ClassId", classId);
						Debug.WriteLine($"RemoveStudentFromClass: Removing IdStudent={studentId} from ClassId={classId}");
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"RemoveStudentFromClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error removing student from class.", ex);
			}
		}

		public List<Class> GetClassesByStudent(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				Debug.WriteLine("GetClassesByStudent: Invalid username.");
				return new List<Class>();
			}

			string studentId = GetStudentIdByUsername(username);
			if (string.IsNullOrWhiteSpace(studentId))
			{
				Debug.WriteLine($"GetClassesByStudent: No IdStudent found for Username={username}");
				return new List<Class>();
			}

			List<Class> classes = new List<Class>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT c.Id, c.Name, c.SubjectId, c.StartDate, c.EndDate
                        FROM [Class] c
                        JOIN [ClassEnrollment] ce ON c.Id = ce.ClassId
                        WHERE ce.IdStudent = @IdStudent";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdStudent", studentId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								classes.Add(new Class
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									SubjectId = reader.GetString(reader.GetOrdinal("SubjectId")),
									StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
									EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetClassesByStudent: Found {classes.Count} classes for IdStudent={studentId}");
				return classes;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetClassesByStudent: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving classes for student.", ex);
			}
		}

		// Teacher Viewing Student Count
		public int GetStudentCountInClass(string teacherId, string classId)
		{
			if (string.IsNullOrWhiteSpace(teacherId) || string.IsNullOrWhiteSpace(classId))
			{
				Debug.WriteLine($"GetStudentCountInClass: Invalid input (TeacherId='{teacherId}', ClassId='{classId}')");
				throw new ArgumentException("Teacher ID or Class ID is invalid.");
			}

			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT COUNT(*) 
                        FROM [ClassEnrollment] ce
                        JOIN [TeacherClass] tc ON ce.ClassId = tc.ClassId
                        WHERE tc.TeacherId = @TeacherId AND ce.ClassId = @ClassId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TeacherId", teacherId);
						cmd.Parameters.AddWithValue("@ClassId", classId);
						int count = (int)cmd.ExecuteScalar();
						Debug.WriteLine($"GetStudentCountInClass: TeacherId={teacherId}, ClassId={classId}, Count={count}");
						return count;
					}
				}
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetStudentCountInClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving student count in class.", ex);
			}
		}

		public List<Student> GetStudentsInClass(string teacherId, string classId)
		{
			if (string.IsNullOrWhiteSpace(teacherId) || string.IsNullOrWhiteSpace(classId))
			{
				Debug.WriteLine($"GetStudentsInClass: Invalid input (TeacherId='{teacherId}', ClassId='{classId}')");
				throw new ArgumentException("Teacher ID or Class ID is invalid.");
			}

			List<Student> students = new List<Student>();
			try
			{
				using (SqlConnection conn = GetConnection())
				{
					string query = @"
                        SELECT s.Id, s.Name, s.BOF, s.IdProvince, s.Gender, s.CreatedAt
                        FROM [Student] s
                        JOIN [ClassEnrollment] ce ON s.Id = ce.IdStudent
                        JOIN [TeacherClass] tc ON ce.ClassId = tc.ClassId
                        WHERE tc.TeacherId = @TeacherId AND ce.ClassId = @ClassId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TeacherId", teacherId);
						cmd.Parameters.AddWithValue("@ClassId", classId);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								students.Add(new Student
								{
									Id = reader.GetString(reader.GetOrdinal("Id")),
									Name = reader.GetString(reader.GetOrdinal("Name")),
									BOF = reader.GetDateTime(reader.GetOrdinal("BOF")),
									IdProvince = reader.GetInt32(reader.GetOrdinal("IdProvince")),
									Gender = reader.GetBoolean(reader.GetOrdinal("Gender")),
									CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
								});
							}
						}
					}
				}
				Debug.WriteLine($"GetStudentsInClass: Found {students.Count} students for TeacherId={teacherId}, ClassId={classId}");
				return students;
			}
			catch (SqlException ex)
			{
				Debug.WriteLine($"GetStudentsInClass: Error - {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception("Error retrieving students in class.", ex);
			}
		}

		
	}
}