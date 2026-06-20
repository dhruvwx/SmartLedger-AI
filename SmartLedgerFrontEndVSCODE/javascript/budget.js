  const token = localStorage.getItem("token");
  let allBudgets = [];
  let editingBudgetId = null;

 if(!token)
 {
   window.location.href = "index.html"
 }

 function loadCategories()
  {
    fetch(`${BASE_URL}/Category` , { //object

      method:"GET",
      headers: {
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
    .then(categories => 
      {
        if(!categories)
        {
          return;
        }
        const categoryDropdown = document.getElementById("category");
                     console.log(categories);


        categories.forEach(category => 
                    {
                      const option = document.createElement("option");
                      option.value = category.id;
                      option.textContent = category.categoryName;
                      categoryDropdown.appendChild(option);
                     });


       })
    .catch(error => {console.log(error)});
  }

  loadCategories();

  //SAVE BUDGET
  document.getElementById("saveBudgetButton")
  .addEventListener("click", function()
  {
    const saveButton = document.getElementById("saveBudgetButton");
    saveButton.disabled = true;
    saveButton.textContent = "Saving...";

    const amount = document.getElementById("budgetAmount").value;
    const month = document.getElementById("month").value;
    const year = document.getElementById("year").value;
    const categoryId = document.getElementById("category").value;

    const budgetData =
{
    monthMaxAmountLimit : parseFloat(amount),
    month : parseInt(month),
    year : parseInt(year),
    categoryId : parseInt(categoryId)
};

console.log(budgetData);
let url = `${BASE_URL}/Budget`;
let httpMethod = "POST";

if (editingBudgetId !== null)
  {
    url = `${BASE_URL}/Budget/${editingBudgetId}`;
    httpMethod = "PUT";

}

    fetch(url , 
  {
      method:httpMethod,
      headers:
      {
                "Content-Type":"application/json",
                "Authorization":`Bearer ${token}`
                },
      body: JSON.stringify(budgetData)
     })
    .then(async response => {
       console.log(response);
       const errortext = await response.text();
       console.log(errortext);

                        if(response.ok)
                        {  

                          if(editingBudgetId === null)
                            {
                                alert("Budget Saved");
                            }
                            else
                            {
                                alert("Budget Updated");
                            }

                           document.getElementById("budgetAmount").value = "";
                           document.getElementById("year").value = "";
                           document.getElementById("month").value = "";
                           document.getElementById("category").value = "";

                           editingBudgetId = null;
                           saveButton.disabled = false;
                          saveButton.textContent = "Save Budget";

                           loadBudgets(); //refreshes table
                        }

  })
    .catch(error => {
      console.log(error);
      saveButton.disabled = false;
      saveButton.textContent = "Save Budget";
    })
});


//loadBudgets
function loadBudgets()
{
  fetch(`${BASE_URL}/Budget` , 
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
        return response.json();
      })
     .then(budgets => 
          {
            if(!budgets)
            {
              return;
            }
            allBudgets = budgets;
            const tableBody = document.getElementById("budgetTableBody");

            tableBody.innerHTML = 
            `
            <tr>
              <td colspan="5" class="text-center"> Loading Budgets... </td>
            </tr>
            `;
          
            tableBody.innerHTML="";

             if(budgets.length === 0)
                {
                  tableBody.innerHTML = 
                  `
                  <tr>
                    <td colspan="5" class="text-center text-muted"> No Budgets Created Yet </td>
                  </tr>
                  `;
                  return;
                }

            budgets.forEach(budget => 
              {
                const row = document.createElement("tr");
                row.innerHTML = 
                            `
                            <td>${budget.monthMaxAmountLimit}</td>
                            <td>${budget.categoryName}</td>
                            <td>${budget.spentAmount}</td>
                            <td>${budget.remainingAmount}</td>

                            <td>

                              <button class="btn btn-warning me-2" onclick="editBudget(${budget.budgetId})">
                                              Edit
                              </button>

                              <button class="btn btn-danger" onclick="deleteBudget(${budget.budgetId})">
                                             Delete
                              </button>

                            </td>
                            `;
                tableBody.appendChild(row);
                
              });

          })
     .catch(error => {console.log(error);});
}
loadBudgets();


//delete budget
function deleteBudget(budgetId)
    {
      const confirmed = confirm("Are you sure you want to delete this budget?");
      if(!confirmed){ return; }

      fetch(`${BASE_URL}/Budget/${budgetId}` , 
        {
          method:"DELETE",
          headers:
            {
              "Authorization":`Bearer ${token}`
              }
         })
         .then(response =>
          {
            if(response.ok)
              {
                alert("BUDGET DELETEDD")
                loadBudgets();
                }
           })
           .catch(error => {console.log(error);});

    }

//EDIT BUDGET
function editBudget(budgetId)
  {
    const budget = allBudgets.find(b=> b.budgetId === budgetId);

    editingBudgetId = budgetId;

    document.getElementById("budgetAmount").value =
        budget.monthMaxAmountLimit;

    document.getElementById("month").value =
        budget.month;

    document.getElementById("year").value =
        budget.year;

    document.getElementById("category").value =
        budget.categoryId;

    //change button text
    document.getElementById("saveBudgetButton")
        .textContent = "Update Budget";
      
  }