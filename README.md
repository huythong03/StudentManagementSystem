# Student Management System

## Overview
The **Student Management System** is a desktop application built with C# and WPF (Windows Presentation Foundation) to manage student-related data for educational institutions. It provides a user-friendly interface for administrators to handle student records, course enrollments, grades, user roles, and enrollment requests. The system uses SQL Server for data storage and is under active development with new features being added.

## Features
- **Student Management**: Add, update, delete, and view student profiles (ID, name, date of birth, province, gender). Students must be at least 15 years old.
- **Course Management**: Create, update, delete, and view academic subjects.
- **Enrollment Management**: Allow students to enroll in subjects, with an approval process for enrollment requests (pending, approved, rejected).
- **Grade Management**: Record and view student grades for enrolled subjects, with search functionality by student name or ID.
- **User Management**: Manage user accounts (add, update, delete) with username/password authentication and role-based access (admin, user).
- **Role-Based Access**: Assign roles to users (e.g., admin, student) and manage permissions.
- **Province Management**: Maintain a list of provinces associated with student records.
- **Dashboard Statistics**: View total number of students and pending enrollment requests.
- **Secure Authentication**: Login system with validation for active users (status = 1).
- **Debug Logging**: Detailed logging of operations and errors for development and troubleshooting.

## Technologies Used
- **Programming Language**: C# (.NET Framework 4.8)
- **UI Framework**: Windows Presentation Foundation (WPF)
- **Database**: Microsoft SQL Server
- **Data Access**: ADO.NET with SQL Server (`System.Data.SqlClient`)
- **Configuration**: `App.config` for database connection string
- **Development Environment**: Visual Studio 2022
- **Namespace**: `StudentManagementSystem.Models` for data models (`Student`, `User`, `Role`, `Subject`, etc.)

## Prerequisites
Before setting up the project, ensure you have the following installed:
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with the following workloads:
  - .NET Desktop Development
  - Data Storage and Processing (for SQL Server integration)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer edition)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (optional, for database management)
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework)

## Installation
Follow these steps to set up the project locally:

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/huythong03/StudentManagementSystem.git
   cd StudentManagementSystem
