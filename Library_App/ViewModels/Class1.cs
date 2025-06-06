using Library_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_App.ViewModels
{
    internal class Class1
    {
        public int IdRole { get; set; }

        public string NameRole { get; set; } = null!;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
