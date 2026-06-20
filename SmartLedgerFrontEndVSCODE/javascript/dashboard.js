const token = localStorage.getItem("token");
// gets saved jwt token

if (!token)
{
    // if token missing

    window.location.href = "index.html";
    // send user back to login
}
// protects dashboard page


const firstName = localStorage.getItem("firstName");
// gets logged in user's name

//refresh category 
let categoryChart = null;

document.getElementById("userName").innerText = firstName;
// inserts name into html


document.getElementById("logoutButton")
.addEventListener("click", function ()
{
    // logout button click

    localStorage.clear();
    // remove everything from browser storage

    window.location.href = "index.html";
    // back to login page
});

function loadDashboard()
{
    // function for dashboard api

    fetch(`${BASE_URL}/Expense/dashboard`,
    {
        method: "GET",

        headers:
        {
            "Authorization":
            `Bearer ${localStorage.getItem("token")}`
            // send jwt token
        }
    })

    .then(response => 
    {
        if(!handleUnauthoized(response))
        {
          return;
        }
        return response.json();
    })
    .then(data =>
    {
        console.log(data);

        document.getElementById("totalSpent")
        .innerText = `₹${data.totalSpent}`;

        document.getElementById("monthlySpent")
        .innerText = `₹${data.monthlySpent}`;

        document.getElementById("topCategory")
        .innerText = data.topCategory;

        document.getElementById("totalExpenses").innerText = data.totalExpenses
    })

    .catch(error =>
    {
        console.log(error);
    });
  }

  loadDashboard();
// load dashboard automatically


function loadExpenses()
{
    // get all expenses api

    fetch(`${BASE_URL}/Expense?pageNo=1&pageSize=5&sortBy=date&sortOrder=desc`,
    {
        method: "GET",

        headers:
        {
            "Authorization":
            `Bearer ${localStorage.getItem("token")}`
            // send jwt token
        }
    })

    .then(response => 
        {
            if(!handleUnauthoized(response))
                {
                    return;
                }
            return response.json();
        })
    .then(data =>
    {
        if(!data)
        {
            return;
        }
        console.log(data);
        // see api response

        const tableBody =
        document.getElementById("expenseTableBody");
        // tbody from html

        tableBody.innerHTML = "";
        // clear old rows

        tableBody.innerHTML = 
        `<tr>
            <td colspan="5" class="text-center"> Loading recent expenses... </td>
        </tr>
        `;

        if(data.length === 0){
            tableBody.innerHTML = 
                    `
                    <tr>
                        <td colspan="5" class="text-center text-muted"> NO EXPENSES YET
                        </td>
                    </tr>    
                    `;
                    return;
        }

        data.forEach(expense =>
        {
            // loop through each expense

            tableBody.innerHTML +=
            `
            <tr>
                <td>${expense.date.split("T")[0]}</td>

                <td>${expense.description}</td>

                <td>${expense.categoryName}</td>

                 <td>₹${expense.amount}</td>

                <td>
                    <button
                         class="btn btn-danger btn-sm"
                         onclick="deleteExpense(${expense.id})">

                              Delete

                    </button>
                </td>
            </tr>
            `;
        });
    })

    .catch(error =>
    {
        console.log(error);
    });
}

loadExpenses();


document.getElementById("addExpenseButton")
.addEventListener("click", function ()
{
    const addExpenseButton = document.getElementById("addExpenseButton");
    addExpenseButton.disabled = true;
    addExpenseButton.textContent = "Saving..."

    const description =
    document.getElementById("description").value;

    const amount =
    document.getElementById("amount").value;

    fetch(`${BASE_URL}/Expense`,
    {
        method: "POST",

        headers:
        {
            "Content-Type":
            "application/json",

            "Authorization":
            `Bearer ${token}`
        },

        body: JSON.stringify(
        {
            description: description,
            amount: parseFloat(amount)
        })
    })

    .then(response =>
    {
        if (!response.ok)
        {
            throw new Error("Expense Add Failed");
        }

        return response.json();
    })

    .then(data =>
    {
        console.log(data);

        alert("Expense Added");

        loadExpenses();
        // refresh expense table

        loadDashboard();
        // refresh cards

        loadCategoryChart();
        //refresh chart

        loadBudgetAlerts();
        //refresh alerts

        document.getElementById("description").value = "";
        document.getElementById("amount").value = "";
        // clear inputs

        addExpenseButton.disabled = false;
        addExpenseButton.textContent = "Add Expense"
    })

    .catch(error =>
    {
        addExpenseButton.disabled = false;
        addExpenseButton.textContent = "Add Expense";
        console.log(error);
    });
});


window.deleteExpense = function(id)
{
    const confirmed = confirm("Delete this expense?");
    if (!confirmed)
    {
        return;
    }
    fetch(`${BASE_URL}/Expense/${id}`,
    {
        method: "DELETE",

        headers:
        {
            "Authorization":
            `Bearer ${token}`
        }
    })

    .then(response =>
    {
        if (!response.ok)
        {
            throw new Error("Delete Failed");
        }

        loadExpenses();
        // refresh expense table

        loadDashboard();
        // refresh dashboard cards

        loadCategoryChart();
        //refresh chart

        loadBudgetAlerts();
        //refresh alerts
    })

    .catch(error =>
    {
        console.log(error);
    });
}

//creating pie chart , from summary api from backend
 function loadCategoryChart()
 {
    fetch(`${BASE_URL}/Expense/summary` , 
        {
            method: "GET",
            headers:{
                "Authorization":`Bearer ${token}`
            }
        })
        .then(response => 
            {
                if(!handleUnauthoized(response))
                {
                    return;
                }
                response.json()
            })
        .then(summary => 
        {
            if(!summary)
        {
            return;
        }
            console.log(summary);

            const labels = summary.map(item => item.categoryName);

            const amounts = summary.map(item => item.totalSpent);

            if(categoryChart !== null)
            {
                categoryChart.destroy();
                // desstroying old  chart before creating new 
            }

            categoryChart =  new Chart(document.getElementById("categoryChart"),
            {
                type:"pie",
                data:
                {
                    labels: labels,
                    datasets:
                    [
                        {
                            data:amounts
                        }

                    ]

                },
                //controls appearance and behaviour 
                options:
                {
                    //resizes automatically 
                    responsive:true,
                    plugins:
                    {
                        //tells what label is of what color
                        legend:
                        {
                            position:"bottom"
                        }
                    }

                }

            });
        }).catch(error => {console.log(error);});
            
    }
loadCategoryChart();

function loadBudgetAlerts()
{
    fetch(`${BASE_URL}/Budget`,
        {
            method:"GET",
            headers:
            {
                "Authorization":`Bearer ${token}`
            }
        })
        .then(response => 
            {
        if(!handleUnauthoized(response))
        {
          return;
        }
        return response.json();
            })
        .then(budgets => 
        {
            if(!budgets)
        {
            return;
        }
            console.log(budgets);

            const container = document.getElementById("budgetAlerts");
            container.innerHTML = "";

            //budgets only that has some warning 
            const warnings = budgets.filter(b => b.warningMessage !== ""); 

            if(warnings.length == 0)
            {
                container.innerHTML =
                `
                    <div class="alert alert-success"> BUDGETS LIMIT IS NOT YET EXCEEDED , YOU CAN SPEND MORE</div>
                `;
                return;
            }

            warnings.forEach(budget =>
            {
                let alertType = "warning";

                if(budget.isExceeded)
                {
                    alertType = "danger";
                }

                container.innerHTML += 
                `
                <div class="alert alert-${alertType}"> 
                    <strong>${budget.categoryName}</strong>
                    -
                    ${budget.warningMessage}
                    <br>
                    Spent:Rs.${budget.spentAmount} 
                    <br>
                    Remaining:Rs${budget.remainingAmount}
                
                </div>
                `;
            });
        })
        .catch(error => {console.log(error);});
}

loadBudgetAlerts();