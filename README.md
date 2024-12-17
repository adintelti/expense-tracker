# expense-tracker

Example
The list of commands and their usage is given below:

# Adding a new expenses
expense-tracker add --description "Lunch" --amount 20
# Expense added successfully (ID: 1)

$ expense-tracker add --description "Dinner" --amount 10
# Expense added successfully (ID: 2)

$ expense-tracker list
# ID  Date       Description  Amount
# 1   2024-08-06  Lunch        $20
# 2   2024-12-08  Dinner       $10

$ expense-tracker list --month 12
# ID  Date       Description  Amount
# 2   2024-12-08  Dinner       $10

$ expense-tracker summary
# Total expenses: $30

$ expense-tracker summary --month 8
# Total expenses for August: $20

$ expense-tracker delete --id 2
# Expense deleted successfully

Exercise can be found at:
https://roadmap.sh/projects/expense-tracker
