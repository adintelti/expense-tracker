using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;

namespace expense_tracker
{
    internal partial class Program
    {
        private const string _fileName = "data.json";
        private const string _outputFileName = "expenses.csv";
        private static EnumCommand _currentCommand;
        private static List<Expense> _listOfExpenses = new List<Expense>();

        static void Main(string[] args)
        {
            LoadFromJsonFile();

            string commandArg = args[0].ToLower();
            if (!ValidateCommandArg(commandArg))
            {
                Console.WriteLine("Invalid command");
                return;
            }

            switch (_currentCommand)
            {
                case EnumCommand.add:
                    int expenseId = _listOfExpenses.Count() + 1;

                    var descriptionResult = GetParamValue("description", args[1], args[2]);
                    if (!descriptionResult.Item1)
                    {
                        return;
                    }

                    string description = descriptionResult.Item2.ToLower();

                    var amountResult = GetParamValue("amount", args[3], args[4]);
                    if (!amountResult.Item1)
                    {
                        return;
                    }
                    double.TryParse(amountResult.Item2, out double expenseAmount);

                    Expense myNewExpense = new Expense(expenseId, description, expenseAmount);
                    _listOfExpenses.Add(myNewExpense);
                    SaveToJsonFile(_listOfExpenses);

                    Console.WriteLine($"Expense added successfully (ID: {expenseId})");

                    ShowListContent();
                    break;
                case EnumCommand.list:
                    if (args.Length > 1)
                    {
                        var monthResult = GetParamValue("month", args[1], args[2]);
                        if (!monthResult.Item1)
                        {
                            return;
                        }
                        ShowFilteredListContent(monthResult.Item2);
                    }
                    else
                        ShowListContent();
                    break;
                case EnumCommand.summary:
                    if (args.Length > 1)
                    {
                        var monthResult = GetParamValue("month", args[1], args[2]);
                        if (!monthResult.Item1)
                        {
                            return;
                        }
                        ShowFilteredSummaryByMonth(monthResult.Item2);
                    }
                    else
                        ShowSummary();
                    break;
                case EnumCommand.delete:
                    var expenseIdResult = GetParamValue("id", args[1], args[2]);
                    if (!expenseIdResult.Item1)
                    {
                        return;
                    }
                    int.TryParse(expenseIdResult.Item2, out int expenseIdToDelete);

                    Expense existentExpense = _listOfExpenses.Find(t => t.ExpenseId == expenseIdToDelete);
                    if (existentExpense == null)
                    {
                        Console.WriteLine($"Delete: Expense not found by (ID: {expenseIdToDelete})");
                        return;
                    }

                    _listOfExpenses.Remove(existentExpense);
                    SaveToJsonFile(_listOfExpenses);

                    Console.WriteLine($"Expense deleted successfully (ID: {expenseIdToDelete})");

                    ShowListContent();
                    break;
                case EnumCommand.update:
                    var expenseIdToUpdateResult = GetParamValue("id", args[1], args[2]);
                    if (!expenseIdToUpdateResult.Item1)
                    {
                        return;
                    }
                    int.TryParse(expenseIdToUpdateResult.Item2, out int expenseIdToUpdate);

                    Expense existentExpenseToUpdate = _listOfExpenses.Find(t => t.ExpenseId == expenseIdToUpdate);
                    if (existentExpenseToUpdate == null)
                    {
                        Console.WriteLine($"Update: Expense not found by (ID: {expenseIdToUpdate})");
                        return;
                    }

                    var newDescriptionResult = GetParamValue("description", args[3], args[4]);
                    if (!newDescriptionResult.Item1)
                    {
                        return;
                    }

                    string newDescription = newDescriptionResult.Item2;

                    var newAmountResult = GetParamValue("amount", args[5], args[6]);
                    if (!newAmountResult.Item1)
                    {
                        return;
                    }
                    double.TryParse(newAmountResult.Item2, out double newAmount);

                    existentExpenseToUpdate.Description = newDescription;
                    existentExpenseToUpdate.Amount = newAmount;
                    SaveToJsonFile(_listOfExpenses);

                    Console.WriteLine($"Expense updated successfully (ID: {expenseIdToUpdate})");

                    ShowListContent();
                    break;
                case EnumCommand.export:
                    ExportToCsv(_listOfExpenses);
                    Console.WriteLine("CSV file created successfully!");
                    break;
                case EnumCommand.cleanup:
                    _listOfExpenses.Clear();
                    SaveToJsonFile(_listOfExpenses);
                    Console.WriteLine("List cleaned up successfully");
                    ShowListContent();
                    break;
                default:
                    break;
            }
        }

        #region params methods

        private static bool ValidateCommandArg(string commandArg)
        {
            switch (commandArg)
            {
                case "add":
                    _currentCommand = EnumCommand.add;
                    return true;
                case "list":
                    _currentCommand = EnumCommand.list;
                    return true;
                case "summary":
                    _currentCommand = EnumCommand.summary;
                    return true;
                case "update":
                    _currentCommand = EnumCommand.update;
                    return true;
                case "delete":
                    _currentCommand = EnumCommand.delete;
                    return true;
                case "export":
                    _currentCommand = EnumCommand.export;
                    return true;
                case "cleanup":
                    _currentCommand = EnumCommand.cleanup;
                    return true;
                default:
                    break;
            }

            return false;
        }

        private static (bool, string) GetParamValue(string paramName, string paramText, string paramValue)
        {
            if (string.IsNullOrEmpty(paramText) && !paramText.ToLower().StartsWith($"--{paramName}") && string.IsNullOrEmpty(paramValue))
            {
                Console.WriteLine($"Add: {paramName} cannot be invalid, null or empty");
                return (false, string.Empty);
            }

            return (true, paramValue);
        }

        #endregion

        #region output methods

        private static void ShowFilteredListContent(string month)
        {
            if (month.Length == 0 || month.Length > 2)
            {
                Console.WriteLine("Invalid length for month filter");
            }
            int.TryParse(month, out int monthNumber);

            if (monthNumber < 1 || monthNumber > 12)
            {
                Console.WriteLine("Invalid month filter");
                return;
            }

            string formatedMonth = monthNumber.ToString("00");
            if (!_listOfExpenses.Any(items => items.ExpenseDate.Month == monthNumber))
            {
                Console.WriteLine($"Expense List with month {formatedMonth} is currently empty");
                return;
            }

            //Header
            Console.WriteLine($"# ExpenseId\tExpenseDate\t\tDescription\tAmount");
            foreach (var expense in _listOfExpenses.Where(items => items.ExpenseDate.Month == monthNumber))
            {
                //Data
                Console.WriteLine($"# {expense.ExpenseId}\t\t{expense.ExpenseDate}\t{expense.Description}\t\t{expense.Amount}");
            }
        }

        private static void ShowListContent()
        {
            if (!_listOfExpenses.Any())
            {
                Console.WriteLine("Expenses List is current empty");
                return;
            }

            //Header
            Console.WriteLine($"# ExpenseId\tExpenseDate\t\tDescription\tAmount");
            foreach (var expense in _listOfExpenses)
            {
                //Data
                Console.WriteLine($"# {expense.ExpenseId}\t\t{expense.ExpenseDate}\t{expense.Description}\t\t{expense.Amount}");
            }
        }

        private static void ShowFilteredSummaryByMonth(string month)
        {
            if (month.Length == 0 || month.Length > 2)
            {
                Console.WriteLine("Invalid length for month filter");
            }
            int.TryParse(month, out int monthNumber);

            if (monthNumber < 1 || monthNumber > 12)
            {
                Console.WriteLine("Invalid month filter");
                return;
            }

            string formatedMonth = monthNumber.ToString("00");
            if (!_listOfExpenses.Any(items => items.ExpenseDate.Month == monthNumber))
            {
                Console.WriteLine($"Expense List with month {formatedMonth} is currently empty");
                return;
            }

            double totalMonthExpenses = 0;
            foreach (var expense in _listOfExpenses.Where(items => items.ExpenseDate.Month == monthNumber))
            {
                totalMonthExpenses += expense.Amount;
            }

            Console.WriteLine($"# Total expenses for month {formatedMonth}: ${totalMonthExpenses}");
        }

        private static void ShowSummary()
        {
            if (!_listOfExpenses.Any())
            {
                Console.WriteLine("Expenses List is current empty");
                return;
            }

            double totalExpenses = 0;
            foreach (var expense in _listOfExpenses)
            {
                totalExpenses += expense.Amount;
            }

            Console.WriteLine($"# Total expenses: ${totalExpenses}");
        }

        private static void ExportToCsv(List<Expense> listOfExpenses)
        {
            var csv = new StringBuilder();

            // Header
            csv.AppendLine("ExpenseId,Description,Amount,ExpenseDate");
            foreach (var expense in listOfExpenses)
            {
                csv.AppendLine($"{expense.ExpenseId},{expense.Description},{expense.Amount},{expense.ExpenseDate:yyyy-MM-dd}");
            }

            File.WriteAllText(_outputFileName, csv.ToString(), Encoding.UTF8);
        }

        #endregion

        #region json file methods

        private static void LoadFromJsonFile()
        {
            if (!File.Exists(_fileName))
            {
                Console.WriteLine($"File {_fileName} does not exists yet, so it could not be found at this time.");
                return;
            }

            string jsonContent = File.ReadAllText(_fileName);
            if (jsonContent == null)
            {
                Console.WriteLine($"{_fileName} is empty and could not be loaded");
                return;
            }
            _listOfExpenses = JsonSerializer.Deserialize<List<Expense>>(jsonContent);
        }

        private static void SaveToJsonFile(List<Expense> listOfExpenses)
        {
            try
            {
                string jsonContent = JsonSerializer.Serialize(listOfExpenses);
                File.WriteAllText(_fileName, jsonContent);
                Console.WriteLine($"{_fileName} saved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fail to save {_fileName} file, Details: {ex.Message}");
            }
        }

        #endregion
    }
}
