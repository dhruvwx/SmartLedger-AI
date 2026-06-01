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

    fetch("https://localhost:7178/api/Expense/dashboard",
    {
        method: "GET",

        headers:
        {
            "Authorization":
            `Bearer ${localStorage.getItem("token")}`
            // send jwt token
        }
    })

    .then(response => response.json())

    .then(data =>
    {
        console.log(data);

        document.getElementById("totalSpent")
        .innerText = `₹${data.totalSpent}`;

        document.getElementById("monthlySpent")
        .innerText = `₹${data.monthlySpent}`;

        document.getElementById("topCategory")
        .innerText = data.topCategory;
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

    fetch("https://localhost:7178/api/Expense",
    {
        method: "GET",

        headers:
        {
            "Authorization":
            `Bearer ${localStorage.getItem("token")}`
            // send jwt token
        }
    })

    .then(response => response.json())

    .then(data =>
    {
        console.log(data);
        // see api response

        const tableBody =
        document.getElementById("expenseTableBody");
        // tbody from html

        tableBody.innerHTML = "";
        // clear old rows

        data.forEach(expense =>
        {
            // loop through each expense

            tableBody.innerHTML +=
            `
            <tr>
                <td>${expense.description}</td>

                <td>₹${expense.amount}</td>

                <td>${expense.categoryName}</td>

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
    const description =
    document.getElementById("description").value;

    const amount =
    document.getElementById("amount").value;

    fetch("https://localhost:7178/api/Expense",
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

        document.getElementById("description").value = "";
        document.getElementById("amount").value = "";
        // clear inputs
    })

    .catch(error =>
    {
        console.log(error);
    });
});


window.deleteExpense = function(id)
{
    fetch(`https://localhost:7178/api/Expense/${id}`,
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
    })

    .catch(error =>
    {
        console.log(error);
    });
}