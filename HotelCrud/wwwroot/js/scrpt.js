// scripts.js
document.addEventListener("DOMContentLoaded", function() {
    const loginForm = document.getElementById("login-form");
    const registerForm = document.getElementById("register-form");

    if (loginForm) {
        loginForm.addEventListener("submit", function(event) {
            const email = document.getElementById("logemail").value;
            const password = document.getElementById("logpass").value;

            if (!email || !password) {
                event.preventDefault();
                alert("Please fill out all fields.");
            }
        });
    }

    if (registerForm) {
        registerForm.addEventListener("submit", function(event) {
            const name = document.getElementById("logname").value;
            const email = document.getElementById("regemail").value; // Change ID to "regemail"
            const password = document.getElementById("regpass").value; // Change ID to "regpass"

            if (!name || !email || !password) {
                event.preventDefault();
                alert("Please fill out all fields.");
            }
        });
    }
});
