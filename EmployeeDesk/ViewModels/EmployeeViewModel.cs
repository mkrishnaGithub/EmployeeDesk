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

        public int CurrentSelectedPage = 1;

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
        private bool _isSaveBtnEnable=true;

        public bool IsSaveBtnEnable
        {
            get { return _isSaveBtnEnable; }
            set { SetProperty(ref _isSaveBtnEnable, value); }
        }
        private bool _isUpdateDeleteBtnEnable = false;

        public bool IsUpdateDeleteBtnEnable
        {
            get { return _isUpdateDeleteBtnEnable; }
            set { SetProperty(ref _isUpdateDeleteBtnEnable, value); }
        }
        #endregion

        #region Pagination properties
        public int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }
        public int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            set { SetProperty(ref _totalPages, value); }
        }
        private bool _isPrvsBtnEnable = false;

        public bool IsPrvsBtnEnable
        {
            get { return _isPrvsBtnEnable; }
            set { SetProperty(ref _isPrvsBtnEnable, value); }
        }

        private bool _isFirstBtnEnable = false;

        public bool IsFirstBtnEnable
        {
            get { return _isFirstBtnEnable; }
            set { SetProperty(ref _isFirstBtnEnable, value); }
        }

        private bool _isLastBtnEnable = true;

        public bool IsLastBtnEnable
        {
            get { return _isLastBtnEnable; }
            set { SetProperty(ref _isLastBtnEnable, value); }
        }

        private bool _isNextBtnEnable = true;

        public bool IsNextBtnEnable
        {
            get { return _isNextBtnEnable; }
            set { SetProperty(ref _isNextBtnEnable, value); }
        }
       
        #endregion
        #region Validations
        public string Error { get { return null; } }

        public string this[string paramName]
        {
            get
            {
                string result = null;
                switch (paramName)
                {
                    case "Name":
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
                IsSaveBtnEnable = CheckPostButtonEnable();  //To enable the save button only when valid data exist
                return result;
            }
        }
        #endregion

        #region ICommands
        public DelegateCommand GetButtonClicked { get; set; }
        public DelegateCommand ShowRegistrationForm { get; set; }
        public DelegateCommand SaveButtonClick { get; set; }
        public DelegateCommand<Employee> UpdateButtonClicked { get; set; }
        public DelegateCommand<Employee> DeleteButtonClicked { get; set; }
        public DelegateCommand DataGridRowSelected { get; set; }

        public DelegateCommand NextPageBtnClicked { get; set; }
        public DelegateCommand PreviousPageBtnClicked { get; set; }
        public DelegateCommand LastPageBtnClicked { get; set; }
        public DelegateCommand FirstPageBtnClicked { get; set; }


        #endregion


        public EmployeeViewModel()
        {
            GetEmployeeDetails();
            GetButtonClicked = new DelegateCommand(GetEmployeeDetails);
            UpdateButtonClicked = new DelegateCommand<Employee>(UpdateEmployeeDetails);
            DeleteButtonClicked = new DelegateCommand<Employee>(DeleteEmployee);
            SaveButtonClick = new DelegateCommand(CreateEmployee);
            ShowRegistrationForm = new DelegateCommand(RegisterEmployee);
            DataGridRowSelected = new DelegateCommand(CheckForUpdateDeleteButton);

            NextPageBtnClicked = new DelegateCommand(LoadNextPageData);
            PreviousPageBtnClicked = new DelegateCommand(LoadPreviousPageData);
            FirstPageBtnClicked = new DelegateCommand(LoadFirstPageData);
            LastPageBtnClicked = new DelegateCommand(LoadLastPageData);
        }


        #region Command Methods

        private void RegisterEmployee()
        {
            IsShowForm = true;
        }
        /// <summary>
        /// To check update&Delete button visibility(enable/disable)
        /// </summary>
        private void CheckForUpdateDeleteButton()
        {
            IsUpdateDeleteBtnEnable = SelectedEmployee != null && SelectedEmployee.Id > 0;
        }
        /// <summary>  
        /// Get employee details  
        /// </summary>  
        public async void GetEmployeeDetails()
        {            
            var employeeDetails = ApiController.GetData(ApiUrls.emplist+(CurrentSelectedPage > 0 ? "?page="+CurrentSelectedPage : ""));
            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {                
                var result = await employeeDetails.Result.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var resp = JsonSerializer.Deserialize<EmployeeResponse>(result, options);
                if (resp != null && resp.data != null && resp.data.Count > 0)
                {
                    try
                    {
                        if (CurrentSelectedPage == 1)
                        {
                            IsFirstBtnEnable = false; IsPrvsBtnEnable = false;
                            IsNextBtnEnable = true; IsLastBtnEnable = true;
                        }
                        else if (CurrentSelectedPage == TotalPages)
                        {
                            IsFirstBtnEnable = true; IsPrvsBtnEnable = true;
                            IsNextBtnEnable = false; IsLastBtnEnable = false;
                        }
                        else
                        {
                            IsFirstBtnEnable = true; IsPrvsBtnEnable = true;
                            IsNextBtnEnable = true; IsLastBtnEnable = true;
                        }
                        CurrentPage = resp.meta != null && resp.meta.pagination != null ? resp.meta.pagination.page : 1;
                        TotalPages = resp.meta != null && resp.meta.pagination != null ? resp.meta.pagination.pages : 0;
                    }
                    catch (Exception e)
                    {
                        //TOODO
                    }
                    Employees = resp.data.OrderBy(x => x.Id).ToList();                     
                }
                
            }
        }

        /// <summary>  
        /// Adds new employee  
        /// </summary>  
        public async void CreateEmployee()
        {
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email))
            {
                EmployeeData newEmployee = new EmployeeData()
                {
                    name = Name,
                    email = Email,
                    gender = ((GenderEnum)Enum.ToObject(typeof(GenderEnum), SelectedGender)).ToString(),
                    status = ((EmployeeStatus)Enum.ToObject(typeof(EmployeeStatus), SelectedStatus)).ToString(),
                };
                var employeeDetails = ApiController.PostData(ApiUrls.emplist, newEmployee);
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
                            MessageBox.Show("Failed to add" + newEmployee.name + "'s details.");
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Failed to save employee details");
                    }

                }
                else
                {
                    MessageBox.Show("Failed to save employee details");
                }
            }
            else
                MessageBox.Show("please fill the required fields");
        }


        /// <summary>  
        /// Updates employee's record  
        /// </summary>  
        /// <param name="employee"></param>  
        public void UpdateEmployeeDetails(Employee employee)
        {
            if (employee != null && employee.Id > 0)
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
                var employeeDetails = ApiController.PutData(ApiUrls.emplist + "/" + employee.Id, updatedEmployee);
                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(employee.Name + "'s details has updated successfully !");
                }
                else
                {
                    MessageBox.Show("Failed to update" + employee.Name + "'s details.");
                }
            }
            else
            {
                MessageBox.Show("Failed to update");
            }
            GetEmployeeDetails();
        }

        /// <summary>  
        /// Deletes employee's record  
        /// </summary>  
        /// <param name="employee"></param>  
        public void DeleteEmployee(Employee employee)
        {
            if(employee!=null && employee.Id > 0)
            {
                var employeeDetails = ApiController.DeleteData(ApiUrls.emplist + "/" + employee.Id);
                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(employee.Name + "'s details has deleted successfully !");
                }
                else
                {
                    MessageBox.Show("Failed to delete" + employee.Name + "'s details.");
                }
            }
            else
            {
                MessageBox.Show("Failed to delete");
            }
            GetEmployeeDetails();
        }
        #endregion

        #region Pagination Command Methods
        private void LoadNextPageData()
        {
            if (CurrentSelectedPage != TotalPages)
            {
                CurrentSelectedPage++;
                GetEmployeeDetails();
            }            
        }
        private void LoadPreviousPageData()
        {
            if (CurrentSelectedPage > 1)
            {
                CurrentSelectedPage--;
                GetEmployeeDetails();
            }
        }
        private void LoadLastPageData()
        {
            CurrentSelectedPage= TotalPages;
            GetEmployeeDetails();
        }
        private void LoadFirstPageData()
        {
            CurrentSelectedPage=1;
            GetEmployeeDetails();
        }
        #endregion

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

