using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVCWebAppUsingADO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MVCWebAppUsingADO.Controllers
{
    public class EmployeeController : Controller
    {
        SqlConnection connection = null;
        SqlCommand command = null;
        List<Employee> listOfEmployees = null;
        IConfiguration _config = null;
        public EmployeeController(IConfiguration config)
        {
            _config = config;

        }
        private SqlConnection GetConnection()
        {
            connection = new SqlConnection( _config.GetConnectionString("MyConnection"));
            return connection;
      
        }
        public IActionResult Index()
        {

            try
            {
                connection = GetConnection();
                command = new SqlCommand("Select * from Employee");
                command.Connection = connection;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    listOfEmployees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee() { Id = (int)reader[0], Name = reader[1].ToString(), Dept = reader[2].ToString(), Salary = (int)reader[3] };
                        listOfEmployees.Add(employee);
                    }

                }
                else
                {
                    ViewBag.msg = "There are  no records";
                    return View();
                }
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex;

            }

            finally
            {
                connection.Close();
                command.Dispose();
                connection.Dispose();
            }
            return View(listOfEmployees);

        }
        public IActionResult Details(int id)
        { 
            Employee employee = Search(id);

            return View(employee);
            
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            connection = GetConnection();
            command = new SqlCommand("Insert into employee(name, dept, salary)values(@name, @dept,@salary)");
            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@dept", employee.Dept);
            command.Parameters.AddWithValue("@salary", employee.Salary);
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {

            Employee employee = Search(id);

            return View(employee);
            

            //connection = GetConnection();
            //command = new SqlCommand("Select * from Employee where id=" + id);
            //command.Connection = connection;
            //connection.Open();
            //SqlDataReader reader = command.ExecuteReader();
            //if (reader.HasRows)
            //{
            //    Employee employee = new Employee();
            //    while (reader.Read())
            //    {
            //        employee = new Employee() { Id = (int)reader[0], Name = reader[1].ToString(), Dept = reader[2].ToString(), Salary = (int)reader[3] };

            //    }

            //    return View(employee);
            //}
            //else
            //{
            //    ViewBag.msg = "There is no records with this ID";
            //    return View();
            //}
        }

        [HttpPost]
        public IActionResult Edit(int id, Employee employee)
        {
            connection = GetConnection();
            command = new SqlCommand("update employee set dept =@dept, salary=@salary where id=@id");
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@dept", employee.Dept);
            command.Parameters.AddWithValue("@salary", employee.Salary);
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Employee employee = Search(id);

            return View(employee);
        }

        [HttpPost]
        public IActionResult Delete(int id,Employee employee)
        {
            connection = GetConnection();
            command = new SqlCommand("Delete employee where id=@id");
            command.Parameters.AddWithValue("@id", employee.Id);
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        private Employee Search(int id)
        {
            Employee employee = new Employee();
            connection = GetConnection();
            command = new SqlCommand("Select * from Employee where id=" + id);
            command.Connection = connection;
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    employee = new Employee() { Id = (int)reader[0], Name = reader[1].ToString(), Dept = reader[2].ToString(), Salary = (int)reader[3] };

                }
            }
            connection.Close();
            return employee;
            
        }
    }
}
