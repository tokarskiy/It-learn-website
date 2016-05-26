using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ItLearn.Models
{
    public class UserInfo
    {
        public string Name;
        public string Country;
        public Role Role;
        public Gender Gender;
        public string Info;
    }

    public enum Gender
    {
        MALE, FEMALE, UNDEFINED
    }

    public enum Role
    {
        USER, MODERATOR, ADMIN
    }
}