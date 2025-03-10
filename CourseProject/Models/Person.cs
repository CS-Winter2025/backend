using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace CourseProject.Models
{
    public class Person : RootObj
    {
        public string Name { get; set; }
    }
}
