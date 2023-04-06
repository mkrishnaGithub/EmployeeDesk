using EmployeeDesk.Models;
using EmployeeDesk.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;

namespace EmployeeDeskTest
{
    [TestClass]
    public class EmployeeDeskTest
    {
        [TestMethod]
        public void Test_GetEmpDetails()
        {
            var empVm = new EmployeeViewModel();
            empVm.GetEmployeeDetails();
        }
        [TestMethod]
        public void Test_RegisterEmployee()
        {
            var empVm = new EmployeeViewModel()
            {
                Name = "Murali",
                Email = "muralikv@gmail.com",
                Gender = 0,
                Status = 0
            };
            empVm.CreateEmployee();
        }        
        [TestMethod]
        public void Test_UpdateEmployee()
        {
            var objemp = new Employee()
            {
                Id= 781183,
                Name = "Murali",
                Email = "muralikv@gmail.com",
                Gender = "Male",
                Status = "Active",                                
            };
            new EmployeeViewModel().UpdateEmployeeDetails(objemp);
        }
        [TestMethod]
        public void Test_DeleteEmployee()
        {
            var objemp = new Employee()
            {
                Name="Murali",
                Id = 781183               
            };
            new EmployeeViewModel().DeleteEmployee(objemp);
        }

        [TestMethod]
        public void Test_RegisterEmployeeFail()
        {
            var empVm = new EmployeeViewModel()
            {
                Name = "",
                Email = "",
                Gender = 0,
                Status = 0
            };
            empVm.CreateEmployee();
        }
    }
}
