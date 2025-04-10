const themeToggleBtn = document.getElementById("theme-toggle");
const lightIcon = document.getElementById("light-icon");
const darkIcon = document.getElementById("dark-icon");
const body = document.body;
const navBar = document.getElementById("navbar");

document.addEventListener("DOMContentLoaded", function () {
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme === "dark") {
        enableDarkMode();
        lightIcon.style.display = "none";
        darkIcon.style.display = "inline";
    }

    themeToggleBtn.addEventListener("click", () => {
        if (body.classList.contains("dark-mode")) {
            enableLightMode();
            localStorage.setItem("theme", "light");
            lightIcon.style.display = "inline";
            darkIcon.style.display = "none";
        } else {
            enableDarkMode();
            localStorage.setItem("theme", "dark");
            lightIcon.style.display = "none";
            darkIcon.style.display = "inline";
        }
    });
});

function enableDarkMode() {
    body.classList.add("dark-mode");
    navBar.classList.add("nav-dark-mode");

    // Change all .text-dark elements to white
    document.querySelectorAll(".text-dark").forEach(el => {
        el.classList.add("text-light");
        el.classList.remove("text-dark");
    });
}

function enableLightMode() {
    body.classList.remove("dark-mode");
    navBar.classList.remove("nav-dark-mode");

    // Change all .text-light elements back to dark
    document.querySelectorAll(".text-light").forEach(el => {
        el.classList.add("text-dark");
        el.classList.remove("text-light");
    });
}
