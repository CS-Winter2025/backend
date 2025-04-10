namespace BlackBoxTests.Data
{
    public class Uris
    {
        public static readonly string Login = "Users/Login";
        public static readonly string Signup = "Users/Register";

        // Invoices
        public static readonly string Charges = "/Invoices?area=Charges";
        public static readonly string ChargesCreate = "/Invoices/Create";
        public static readonly string ChargesDetails = "/Invoices/Details";
        public static readonly string ChargesEdit = "/Invoices/Edit";
        public static readonly string ChargesDelete = "/Invoices/Delete";

        public static readonly string Residents = "Residents";
        public static readonly string Assets = "Assets";

        public static readonly string Services = "Services?area=Services";
        public static readonly string ServicesCreate = "/Services/Create";
        public static readonly string ServicesDetails = "/Services/Details";
        public static readonly string ServicesEdit = "/Services/Edit";
        public static readonly string ServicesDelete = "/Services/Delete";

        public static readonly string Employees = "Employees?area=Employees";
        public static readonly string EmployeesCreate = "/Employees/Create";
        public static readonly string EmployeesDetails = "/Employees/Details";
        public static readonly string EmployeesEdit = "/Employees/Edit";
        public static readonly string EmployeesDelete = "/Employees/Delete";

        public static readonly string Users = "Users";
    }
}
