using EmployeeDesk.Controller;
using EmployeeDesk.Models;
using EmployeeDesk.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmployeeDesk.ViewModels
{
    public class EmployeeViewModel : BindableBase, INotifyPropertyChanged
    {
        #region Properties  

        private List<Employee> _employees;

        public List<Employee> Employees
        {
            get { return _employees; }
            set { SetProperty(ref _employees, value); }
        }

        private Employee _selectedEmployee;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref _selectedEmployee, value); }
        }

        private bool _isLoadData;

        public bool IsLoadData
        {
            get { return _isLoadData; }
            set { SetProperty(ref _isLoadData, value); }
        }

        private string _responseMessage = "Welcome to REST API Tutorials";

        public string ResponseMessage
        {
            get { return _responseMessage; }
            set { SetProperty(ref _responseMessage, value); }
        }

        #region [Create Employee Properties]  

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }


        private string _email;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _gender;

        public string Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        #endregion
        private bool _isShowForm;

        public bool IsShowForm
        {
            get { return _isShowForm; }
            set { SetProperty(ref _isShowForm, value); }
        }

        private string _showPostMessage = "Fill the form to register an employee!";

        public string ShowPostMessage
        {
            get { return _showPostMessage; }
            set { SetProperty(ref _showPostMessage, value); }
        }
        #endregion

        #region ICommands  
        public DelegateCommand GetButtonClicked { get; set; }
        public DelegateCommand ShowRegistrationForm { get; set; }
        public DelegateCommand PostButtonClick { get; set; }
        public DelegateCommand<Employee> UpdateButtonClicked { get; set; }
        public DelegateCommand<Employee> DeleteButtonClicked { get; set; }
        #endregion

        #region Constructor          
        public EmployeeViewModel()
        {

            GetButtonClicked = new DelegateCommand(GetEmployeeDetails);
            UpdateButtonClicked = new DelegateCommand<Employee>(UpdateEmployeeDetails);
            DeleteButtonClicked = new DelegateCommand<Employee>(DeleteEmployeeDetails);
            PostButtonClick = new DelegateCommand(CreateNewEmployee);
            ShowRegistrationForm = new DelegateCommand(RegisterEmployee);
        }
        #endregion

       
        private void RegisterEmployee()
        {
            IsShowForm = true;
        }

        /// <summary>  
        /// Fetches employee details  
        /// </summary>  
        private async void GetEmployeeDetails()
        {
            var employeeDetails = ApiController.GetCall(ApiUrls.emplist);
            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Employees = employeeDetails.Result.Content.ReadAsAsync<List<Employee>>().Result;
                var result = await employeeDetails.Result.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var resp = JsonSerializer.Deserialize<EmployeeResponse>(result, options);
                Employees = resp.data;
                IsLoadData = true;
            }
        }

        /// <summary>  
        /// Adds new employee  
        /// </summary>  
        private async void CreateNewEmployee()
        {
            EmployeeData newEmployee = new EmployeeData()
            {
                name = Name,
                email = Email,
                gender = Gender,
                status = Status

            };
            var employeeDetails = ApiController.PostCall(ApiUrls.emplist, newEmployee);
            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var result = await employeeDetails.Result.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var resp = JsonSerializer.Deserialize<CreateEmployeeResponse>(result, options);
                    if (resp != null && resp.code == 201)
                    {
                        ShowPostMessage = newEmployee.name + "'s details has successfully been added!";
                    }
                    else
                    {
                        ShowPostMessage = "Failed to update" + newEmployee.name + "'s details.";
                    }
                }
                catch (Exception e)
                {
                    ShowPostMessage = "Exception :" + e.InnerException != null ? e.InnerException.Message : "Error happened";
                }

            }
            else
            {
                ShowPostMessage = "Failed to update" + newEmployee.name + "'s details.";

            }
        }


        /// <summary>  
        /// Updates employee's record  
        /// </summary>  
        /// <param name="employee"></param>  
        private void UpdateEmployeeDetails(Employee employee)
        {
            var employeeDetails = ApiController.PutCall(ApiUrls.emplist + "/" + employee.Id, employee);
            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ResponseMessage = employee.Name + "'s details has successfully been updated!";
            }
            else
            {
                ResponseMessage = "Failed to update" + employee.Name + "'s details.";
            }
        }

        /// <summary>  
        /// Deletes employee's record  
        /// </summary>  
        /// <param name="employee"></param>  
        private void DeleteEmployeeDetails(Employee employee)
        {
            var employeeDetails = ApiController.DeleteCall(ApiUrls.emplist + "/" + employee.Id);
            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ResponseMessage = employee.Name + "'s details has successfully been deleted!";
            }
            else
            {
                ResponseMessage = "Failed to delete" + employee.Name + "'s details.";
            }
        }
      
    }
}

