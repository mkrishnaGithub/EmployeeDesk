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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static EmployeeDesk.Models.Enums;

namespace EmployeeDesk.ViewModels
{
    public class EmployeeViewModel : BindableBase, INotifyPropertyChanged, IDataErrorInfo
    {
        #region Properties  

        Regex regexemail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
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

     
        #region Employee Properties 

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

     
        private int _selectedGender;
        public int SelectedGender
        {
            get { return _selectedGender; }
            set { SetProperty(ref _selectedGender, value); }
        }
        private GenderEnum _gender;

        public GenderEnum Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }

        private int _selectedStatus;
        public int SelectedStatus
        {
            get { return _selectedStatus; }
            set { SetProperty(ref _selectedStatus, value); }
        }
        private EmployeeStatus _status;

        public EmployeeStatus Status
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
        private bool _isPostBtnEnable=true;

        public bool IsPostBtnEnable
        {
            get { return _isPostBtnEnable; }
            set { SetProperty(ref _isPostBtnEnable, value); }
        }
        private bool _isUpdateDeleteBtnEnable = false;

        public bool IsUpdateDeleteBtnEnable
        {
            get { return _isUpdateDeleteBtnEnable; }
            set { SetProperty(ref _isUpdateDeleteBtnEnable, value); }
        }

        //private string _showPostMessage = "Fill the form to register an employee!";

        //public string ShowPostMessage
        //{
        //    get { return _showPostMessage; }
        //    set { SetProperty(ref _showPostMessage, value); }
        //}
        #endregion

        #region ICommands  
        public DelegateCommand GetButtonClicked { get; set; }
        public DelegateCommand ShowRegistrationForm { get; set; }
        public DelegateCommand PostButtonClick { get; set; }
        public DelegateCommand<Employee> UpdateButtonClicked { get; set; }
        public DelegateCommand<Employee> DeleteButtonClicked { get; set; }
        public DelegateCommand DataGridRowSelected { get; set; }

        public string Error { get { return null; } }

        public string this[string paramName]
        {
            get
            {
                string result = null;
                switch (paramName)
                {
                    case "Name" :
                        if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
                        {
                            result = "Employee name cannot be empty";                            
                        }
                        
                        break;
                    case "Email":
                      
                        if (string.IsNullOrEmpty(Email) || (!string.IsNullOrEmpty(Email) && !regexemail.IsMatch(Email)))
                        {
                            result = "Email should be valid one";                           
                        }                        
                        break;
                }
                IsPostBtnEnable= CheckPostButtonEnable();
                return result;
            }
        }
        #endregion

        #region Constructor          
        public EmployeeViewModel()
        {
            GetEmployeeDetails();
            GetButtonClicked = new DelegateCommand(GetEmployeeDetails);
            UpdateButtonClicked = new DelegateCommand<Employee>(UpdateEmployeeDetails);
            DeleteButtonClicked = new DelegateCommand<Employee>(DeleteEmployeeDetails);
            PostButtonClick = new DelegateCommand(CreateNewEmployee);
            ShowRegistrationForm = new DelegateCommand(RegisterEmployee);
            DataGridRowSelected = new DelegateCommand(CheckForUpdateDeleteButton);
        }
        #endregion

       
        private void RegisterEmployee()
        {
            IsShowForm = true;
        }

        private void CheckForUpdateDeleteButton()
        {
            IsUpdateDeleteBtnEnable = SelectedEmployee != null && SelectedEmployee.Id > 0;            
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
                gender = ((GenderEnum)Enum.ToObject(typeof(GenderEnum), SelectedGender)).ToString(),
                status = ((EmployeeStatus)Enum.ToObject(typeof(EmployeeStatus), SelectedStatus)).ToString(),
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
                        MessageBox.Show(newEmployee.name + "'s details has added successfully !");
                        GetEmployeeDetails();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update" + newEmployee.name + "'s details.");                       
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to update");                   
                }

            }
            else
            {
                MessageBox.Show("Failed to update");               
            }
        }


        /// <summary>  
        /// Updates employee's record  
        /// </summary>  
        /// <param name="employee"></param>  
        private void UpdateEmployeeDetails(Employee employee)
        {
            EmployeeData updatedEmployee = null;
            if (employee != null)
            {
                updatedEmployee = new EmployeeData()
                {
                    name = employee.Name,
                    email = employee.Email,
                    gender = employee.Gender,
                    status = employee.Status
                };
            }
            var employeeDetails = ApiController.PutCall(ApiUrls.emplist + "/" + employee.Id, updatedEmployee);
            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show(employee.Name + "'s details has updated successfully !");              
            }
            else
            {
                MessageBox.Show("Failed to update" + employee.Name + "'s details.");
            }
            GetEmployeeDetails();
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
                MessageBox.Show(employee.Name + "'s details has deleted successfully !");                
            }
            else
            {
                MessageBox.Show("Failed to delete" + employee.Name + "'s details.");
            }
            GetEmployeeDetails();
        }
        private bool CheckPostButtonEnable()
        {                       
            if(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || (!string.IsNullOrEmpty(Email) && !regexemail.IsMatch(Email)))
            {
                return false;
            }
            return true;
        }
      
    }
}

