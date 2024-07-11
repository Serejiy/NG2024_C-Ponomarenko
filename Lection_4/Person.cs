namespace PhoneBookApp
{
    public class Person
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public Person(string fullName, string phoneNumber)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }
    }
}

