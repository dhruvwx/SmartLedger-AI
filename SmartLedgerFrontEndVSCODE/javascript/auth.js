const loginForm = document.getElementById("loginForm");
// finds form using id="loginForm"

loginForm.addEventListener("submit", function (event) 
    {

    // listens when login form submits

    event.preventDefault();
    // stops page refresh
    // default HTML form refreshes page
    // we stop that because JS will handle login

    const email = document.getElementById("email").value;
    // gets current email textbox value

    const password = document.getElementById("password").value;
    // gets current password textbox value

    fetch(`${BASE_URL}/Auth/Login`, {
    // calling login api endpoint

    method: "POST",
    // same as swagger POST request

    headers: {
        "Content-Type": "application/json"
        // tells backend:
        // "I am sending JSON data"
    },

    body: JSON.stringify({
        // converts JS object → JSON

        email: email,
        // left side = backend property name
        // right side = variable value

        password: password
    })
})

.then(response => response.json())
/*
response comes from backend

example:
200 OK
JWT token

response.json()
converts response → readable object
*/

.then(data => {

    console.log(data);
    // check backend response

    localStorage.setItem("token", data.token);
    // save jwt token in browser storage

    localStorage.setItem("firstName", data.firstName);
    // save first name

    localStorage.setItem("role", data.role);
    // save role
    window.location.href = "dashboard.html";
// redirects browser to dashboard page
    // temporary success message

})

})

.catch(error => {

    console.error("FULL ERROR:", error);
// gives better debugging

});
