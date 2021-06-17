using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Models.Accounts
{
    public class UpdateAccountRequest
    {
   
        private string _title;
        private string _name;
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _email;


    
    
        public string Title
        {
            get => _title;
            set => _title = replaceEmptyWithNull(value);
        }
        public string Name
        {
            get => _name;
            set => _name = replaceEmptyWithNull(value);
        }

        [EnumDataType(typeof(RoleList))]
        public string Role
        {
            get => _role;
            set => _role = replaceEmptyWithNull(value);
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}