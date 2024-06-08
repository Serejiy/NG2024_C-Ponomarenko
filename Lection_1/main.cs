using System;
using System.Collections.Generic;
using System.Linq;

public class Transaction
{
    public string TransactionId { get; }
    public double Amount { get; }
    public DateTime Timestamp { get; }

    public Transaction(string transactionId, double amount)
    {
        TransactionId = transactionId;
        Amount = amount;
        Timestamp = DateTime.Now;
    }

    public string GetTransactionDetails()
    {
        return $"Transaction ID: {TransactionId}, Amount: {Amount}, Timestamp: {Timestamp}";
    }
}
public class Balance
{
    public string BalanceId { get; }
    public string ClientId { get; }
    public double BalanceAmount { get; private set; }
    public List<Transaction> Transactions { get; }

    public Balance(string balanceId, string clientId, double initialAmount)
    {
        BalanceId = balanceId;
        ClientId = clientId;
        BalanceAmount = initialAmount;
        Transactions = new List<Transaction>();
    }

    public double GetBalance()
    {
        return BalanceAmount;
    }

    public void UpdateBalance(double amount)
    {
        BalanceAmount += amount;
        Transactions.Add(new Transaction(Guid.NewGuid().ToString(), amount));
    }

    public List<Transaction> GetTransactions()
    {
        return Transactions;
    }
}

// User class
public class User
{
    public string UserId { get; }
    public string Name { get; }
    public string Role { get; }
    public Balance AccountBalance { get; }

    public User(string userId, string name, string role, Balance accountBalance)
    {
        UserId = userId;
        Name = name;
        Role = role;
        AccountBalance = accountBalance;
    }

    public void Deposit(double amount)
    {
        AccountBalance.UpdateBalance(amount);
        Console.WriteLine($"Amount deposited: {amount}. New balance: {AccountBalance.GetBalance()}");
    }

    public void Withdraw(double amount)
    {
        if (AccountBalance.BalanceAmount >= amount)
        {
            AccountBalance.UpdateBalance(-amount);
            Console.WriteLine($"Amount withdrawn: {amount}. New balance: {AccountBalance.GetBalance()}");
        }
        else
        {
            Console.WriteLine("Insufficient balance.");
        }
    }

    public double GetBalance()
    {
        return AccountBalance.GetBalance();
    }

    public string GetAccountDetails()
    {
        return $"User ID: {UserId}, Name: {Name}, Role: {Role}, Balance: {AccountBalance.GetBalance()}";
    }

    public void ViewTransactions()
    {
        Console.WriteLine("Transactions:");
        if (AccountBalance.GetTransactions().Count == 0)
        {
            Console.WriteLine("No transactions found.");
        }
        else
        {
            foreach (var transaction in AccountBalance.GetTransactions())
            {
                Console.WriteLine(transaction.GetTransactionDetails());
            }
        }
    }
}

// Admin class
public class Admin : User
{
    public Admin(string userId, string name, Balance accountBalance)
        : base(userId, name, "Admin", accountBalance) { }

    public void AddUser(List<User> users, User newUser)
    {
        if (users.Exists(u => u.UserId == newUser.UserId))
        {
            Console.WriteLine("User with this ID already exists.");
        }
        else
        {
            users.Add(newUser);
            Console.WriteLine($"User {newUser.Name} added with User ID: {newUser.UserId}");
        }
    }

    public void RemoveUser(List<User> users, string userId)
    {
        var user = users.Find(u => u.UserId == userId);
        if (user != null)
        {
            users.Remove(user);
            Console.WriteLine($"User {user.Name} removed.");
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }

    public void ViewUserDetails(List<User> users, string searchKeyword)
    {
        var matchingUsers = users.Where(u => u.UserId.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase) || u.Name.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase)).ToList();
        if (matchingUsers.Count > 0)
        {
            foreach (var user in matchingUsers)
            {
                Console.WriteLine(user.GetAccountDetails());
                user.ViewTransactions();
            }
        }
        else
        {
            Console.WriteLine("No users found matching the search criteria.");
        }
    }

    public void ViewUserProfile(List<User> users, string searchKeyword)
    {
        var user = users.Find(u => u.UserId.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase) || u.Name.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            Console.WriteLine(user.GetAccountDetails());
            user.ViewTransactions();
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var users = new List<User>();
        var admin = new Admin(Guid.NewGuid().ToString(), "SuperAdmin", new Balance(Guid.NewGuid().ToString(), "SuperAdmin", 0));

        users.Add(admin);

        while (true)
        {
            Console.WriteLine("\n1. Add User");
            Console.WriteLine("2. Remove User");
            Console.WriteLine("3. Search and View User Details");
            Console.WriteLine("4. View Specific User Profile");
            Console.WriteLine("5. Deposit");
            Console.WriteLine("6. Withdraw");
            Console.WriteLine("7. Exit");

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter user name: ");
                    var name = Console.ReadLine();
                    Console.Write("Enter initial balance: ");
                    if (!double.TryParse(Console.ReadLine(), out double initialBalance))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number for the initial balance.");
                        continue;
                    }
                    Console.Write("Enter role (Admin/Regular): ");
                    var role = Console.ReadLine();
                    var userId = Guid.NewGuid().ToString();
                    var balance = new Balance(Guid.NewGuid().ToString(), userId, initialBalance);
                    User newUser;
                    if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        newUser = new Admin(userId, name, balance);
                    }
                    else
                    {
                        newUser = new User(userId, name, "Regular", balance);
                    }
                    admin.AddUser(users, newUser);
                    break;

                case "2":
                    Console.Write("Enter user ID to remove: ");
                    var removeUserId = Console.ReadLine();
                    admin.RemoveUser(users, removeUserId);
                    break;

                case "3":
                    Console.Write("Enter user ID or name to search: ");
                    var searchKeyword = Console.ReadLine();
                    admin.ViewUserDetails(users, searchKeyword);
                    break;

                case "4":
                    Console.Write("Enter user ID or name to view profile: ");
                    var profileSearchKeyword = Console.ReadLine();
                    admin.ViewUserProfile(users, profileSearchKeyword);
                    break;

                case "5":
                    Console.Write("Enter user ID or name for deposit: ");
                    var depositSearchKeyword = Console.ReadLine();
                    var depositUser = users.Find(u => u.UserId.Contains(depositSearchKeyword, StringComparison.OrdinalIgnoreCase) || u.Name.Contains(depositSearchKeyword, StringComparison.OrdinalIgnoreCase));
                    if (depositUser != null)
                    {
                        Console.Write("Enter amount to deposit: ");
                        if (double.TryParse(Console.ReadLine(), out double depositAmount))
                        {
                            depositUser.Deposit(depositAmount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User not found.");
                    }
                    break;

                case "6":
                    Console.Write("Enter user ID or name for withdrawal: ");
                    var withdrawSearchKeyword = Console.ReadLine();
                    var withdrawUser = users.Find(u => u.UserId.Contains(withdrawSearchKeyword, StringComparison.OrdinalIgnoreCase) || u.Name.Contains(withdrawSearchKeyword, StringComparison.OrdinalIgnoreCase));
                    if (withdrawUser != null)
                    {
                        Console.Write("Enter amount to withdraw: ");
                        if (double.TryParse(Console.ReadLine(), out double withdrawAmount))
                        {
                            withdrawUser.Withdraw(withdrawAmount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User not found.");
                    }
                    break;

                case "7":
                    return;

                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }

            Console.WriteLine("\nCurrent Users:");
            foreach (var user in users)
            {
                Console.WriteLine($"User ID: {user.UserId}, Name: {user.Name}, Role: {user.Role}");
            }
        }
    }
}

