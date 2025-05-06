function validateRegisterForm() {
    const username = document.querySelector('[name="username"]').value;
    const email = document.querySelector('[name="email"]').value;
    const password = document.querySelector('[name="password"]').value;

    if (username.length < 3 || password.length < 6) {
        alert("Username must be at least 3 characters and password 6 characters.");
        return false;
    }
    return true;
}

function validateLoginForm() {
    const username = document.querySelector('[name="username"]').value;
    const password = document.querySelector('[name="password"]').value;

    if (!username || !password) {
        alert("Both fields are required.");
        return false;
    }
    return true;
}
