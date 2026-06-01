  const token = localStorage.getItem("token");

 if(!token)
 {
   window.location.href = "index.html"
 }

 function loadCategories()
  {
    fetch("https://localhost:7178/api/Category" , { //object

      method:"GET",
      headers: {
                  "Authorization":`Bearer ${token}`
                }
    })
    .then(response => response.json())
    .then(categories => 
      {const categoryDropdown = document.getElementById("category");
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

    fetch("https://localhost:7178/api/Budget" , 
  {
      method:"POST",
      headers:
      {
                "Content-Type":"application/json",
                "Authorization":`Bearer ${token}`
                },
      body: JSON.stringify(
        {
        monthMaxAmountLimit : parseFloat(amount),
        month : parseInt(month),
        year : parseInt(year),
        categoryId : parseInt(categoryId)
        
        })
     })
    .then(async response => {
       console.log(response);
       const errortext = await response.text();
       console.log(errortext);

                        if(response.ok){
                          alert("Budget Saved");
                        }

  })
    .catch(error => {console.log(error)})
});


//loadBudgets
function loadBudgets()
{
  fetch("https://localhost:7178/api/Budget" , 
    {
      method: "GET",
      headers:{
                "Authorization":`Bearer ${token}`
              }
     })
     .then(response => response.json())
     .then(budgets => 
          {
            const tableBody = document.getElementById("budgetTableBody");
            tableBody.innerHTML="";

            budgets.forEach(budget => 
              {
                const row = document.createElement("tr");
                row.innerHTML = 
                            `
                            <td>${budget.monthMaxAmountLimit}</td>
                            <td>${budget.categoryName}</td>
                            <td>${budget.spentAmount}</td>
                            <td>${budget.remainingAmount}</td>
                            `;
                tableBody.appendChild(row);
                
              });

          })
     .catch(error => {console.log(error);});
}
loadBudgets();