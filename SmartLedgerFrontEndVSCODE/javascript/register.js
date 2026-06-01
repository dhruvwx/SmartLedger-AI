document.getElementById("registerButton")
.addEventListener("click", function ()
{

    const button =
document.getElementById("registerButton");

button.disabled = true;
button.innerText = "Registering...";


    const firstName =
    document.getElementById("firstName").value;

    const lastName =
    document.getElementById("lastName").value;

    const email =
    document.getElementById("email").value;

    const password =
    document.getElementById("password").value;


    fetch("https://localhost:7178/api/Auth/Register",
    {
        method: "POST",

        headers:
        {
            "Content-Type":
            "application/json"
        },

        body: JSON.stringify(
        {
            firstName: firstName,
            lastName: lastName,
            email: email,
            password: password
        })
    })

    .then(response =>
    {
        if (!response.ok)
        {
            throw new Error("Registration Failed");
        }

        alert("Registration Successful");

        button.disabled = false;
button.innerText = "Register";

        window.location.href =
        "index.html";
    })

    .catch(error =>
{
    button.disabled = false;
    button.innerText = "Register";

    console.log(error);
});
});