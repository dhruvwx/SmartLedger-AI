const token = localStorage.getItem("token");
if(!token)
{
  window.location.href = "index.html"
}

let allExpenses = [];
let editingExpenseId = null;
let editingExpense = null;
let currentPage = 1;
let pageSize = 5;

function loadCategories()
{
  fetch("https://localhost:7178/api/Category" , 
    {
      method: "GET",
      headers: 
      {
        "Authorization":`Bearer ${token}`
      }
     })
     .then(response => response.json())
     .then(categories => 
      {
        const filterDropdown = document.getElementById("filterCategory");

        categories.forEach(category =>
          {
            const option = document.createElement("option");
            option.value = category.id;
            option.textContent = category.categoryName;
            filterDropdown.appendChild(option);
           });
     }) 
     .catch(error => {console.log(error);});
}  
loadCategories();

function loadExpenses(page = 1)
  {
    currentPage = page;
    const categoryId = document.getElementById("filterCategory").value;
    const month = document.getElementById("filterMonth").value;
    const minAmount = document.getElementById("minAmount").value;
    const maxAmount = document.getElementById("maxAmount").value;
    const sortBy = document.getElementById("sortBy").value;
    const sortOrder = document.getElementById("sortOrder").value;
  
    let url = `https://localhost:7178/api/Expense?pageNo=${currentPage}&pageSize=${pageSize}`;
    if(categoryId)
      {
        url += `&categoryId=${categoryId}`;
    }
    if(month){
      url += `&month=${month}`;
    }
    if(minAmount){
      url += `&minAmount=${minAmount}`;
    }
    if(maxAmount){
      url += `&maxAmount=${maxAmount}`;
    }
    if(sortBy){
      url += `&sortBy=${sortBy}`;
    }
    if(sortOrder){
      url += `&sortOrder=${sortOrder}`;
    }

    fetch(url , 
      {
        method:"GET",
        headers: {
          "Authorization": `Bearer ${token}`
        }
      })
      .then(response => response.json())
      .then(expenses =>
          {
            console.log(expenses);
            allExpenses = expenses;
            document.getElementById("pageNumber").textContent = `Page ${currentPage}`;

            const tableBody = document.getElementById("expenseTableBody");
            tableBody.innerHTML = 
            `
            <tr>
              <td colspan="5" class="text-center"> Loading Expenses... </td>
            </tr>
            `;

            if(expenses.length === 0)
            {
              tableBody.innerHTML =
              `
              <tr>
              <td colspan="5" class="text-center text-muted"> No Expenses Found </td>
              </tr>
              `;
              return;
            }

            tableBody.innerHTML = "";

            expenses.forEach(expense =>
                {
                  const row = document.createElement("tr");
                  row.innerHTML = 
                    `
                    <td>${expense.description}</td>
                      <td>${expense.amount}</td>
                      <td>${expense.date.split("T")[0]}</td>
                      <td>${expense.categoryName}</td>
                      <td>
                           <button class="btn btn-warning me-2" onclick="editExpense(${expense.id})"> Edit</button>
                           <button class="btn btn-danger" onclick="deleteExpense(${expense.id})">Delete</button>  
                      </td>
                     `;
                  tableBody.appendChild(row);
                  });
          })
      .catch(error => {console.log(error);});
  }

loadExpenses();

document.getElementById("saveExpenseButton").addEventListener("click" , function()
    {
      const description = document.getElementById("description").value;
      const amount = document.getElementById("amount").value;
      const date = document.getElementById("date").value;
     
      const expenseData = 
        {
          description: description,
          amount: parseFloat(amount),
        };
        if(editingExpense !== null)
{
    expenseData.categoryId =
    editingExpense.categoryId;
}
        if(date)
          {
             expenseData.date = date;
          }
        console.log(expenseData);


let url = "https://localhost:7178/api/Expense";
let httpMethod = "POST";
          if(editingExpenseId !== null)
            {
              url = `https://localhost:7178/api/Expense/${editingExpenseId}`;
              httpMethod = "PUT";

             }

        fetch(url ,
          {
            method: httpMethod,
            headers: 
            {
              "Content-Type": "application/json",
              "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(expenseData)
          })
         
        .then(async response =>
           {
             console.log(response);
              const responseText = await response.text();
              console.log(responseText);

              if(response.ok)
                {
                    if(editingExpenseId === null)
                            {
                              alert("Expense Saved");
                            }
                      else
                            {
                             alert("Expense Updated");
                      }   
                    document.getElementById("description").value = "";
                    document.getElementById("amount").value = "";
                    document.getElementById("date").value = "";

                    editingExpenseId = null;

                    document.getElementById("saveExpenseButton").textContent = "Save Expense";

                    loadExpenses();
                }
           })
        .catch(error => {console.log(error);});

    }); 


    //delete expense
    function deleteExpense(expenseId)
      {
        const confirmed = confirm("Are you sure you want to delete this expense?");


        if(!confirmed)
          {
            return;
          }

        fetch(`https://localhost:7178/api/Expense/${expenseId}` ,
          {
            method: "DELETE",
            headers: {
              "Authorization": `Bearer ${token}`
            }
          })
          .then(response => {
            if(response.ok)
              {
                alert("Expense deleted");
                loadExpenses();
              }
          })
          .catch(error => {console.log(error);});
      }


//edit expense
function editExpense(expenseId)
    {
      const expense = allExpenses.find(e => e.id === expenseId);
      editingExpense = expense;
      editingExpenseId = expenseId;

      document.getElementById("description").value = expense.description;
      document.getElementById("amount").value = expense.amount;
      document.getElementById("date").value = expense.date.split("T")[0];
      document.getElementById("saveExpenseButton").textContent = "Update Expense";
    }


    //apply filter button
    document.getElementById("applyFiltersButton").addEventListener("click" , function()
    {
      loadExpenses(1);

  });

  document.getElementById("clearFiltersButton").addEventListener("click" , function()
  {
    document.getElementById("filterCategory").value = "";
    document.getElementById("filterMonth").value = "";
    document.getElementById("minAmount").value = "";
    document.getElementById("maxAmount").value = "";
    document.getElementById("sortBy").value = "";
    document.getElementById("sortOrder").value = "";

    loadExpenses(1);
  });

  //next page button
  document.getElementById("nextPage").addEventListener("click" , function()
  {
    currentPage++;
    loadExpenses(currentPage);
  });
   document.getElementById("prevPage").addEventListener("click" , function()
    {
      if(currentPage > 1)
        {
          currentPage--;
          loadExpenses(currentPage);
        }
   });

