namespace KASCFlightLogging.Models.ViewModels // Adjust the namespace accordingly
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }        

        public string Roles { get; set; }
        public bool IsLockedOut { get; set; }
    }
}