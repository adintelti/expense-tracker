namespace expense_tracker
{
    internal class Expense
    {
        public Expense(int expenseId, string description, double amount)
        {
            ExpenseId = expenseId;
            Description = description;
            Amount = amount;
        }

        public int ExpenseId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.Now;
    }
}
