"use strict";

// Enhance the small-screen menu. The HTML keeps every link visible if JavaScript
// is unavailable, and aria-expanded tells assistive technology whether it is open.
const navigation = document.querySelector(".navigation");
const menuButton = document.querySelector(".menu-toggle");
const navLinks = document.querySelector("#nav-links");
if (navigation && menuButton && navLinks) {
    menuButton.hidden = false;
    navigation.classList.add("menu-ready");
    const closeMenu = () => {
        navLinks.classList.remove("is-open");
        menuButton.setAttribute("aria-expanded", "false");
    };
    menuButton.addEventListener("click", () => {
        const isOpen = navLinks.classList.toggle("is-open");
        menuButton.setAttribute("aria-expanded", String(isOpen));
    });
    navLinks.addEventListener("click", (event) => {
        if (event.target.closest("a")) closeMenu();
    });
    navigation.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && menuButton.getAttribute("aria-expanded") === "true") {
            closeMenu();
            menuButton.focus();
        }
    });

    // Highlight the section currently being read, without changing browser history.
    if ("IntersectionObserver" in window) {
        const observer = new IntersectionObserver((entries) => {
            for (const entry of entries) {
                if (!entry.isIntersecting) continue;
                navLinks.querySelectorAll("a").forEach((link) => {
                    if (link.hash === "#" + entry.target.id) link.setAttribute("aria-current", "location");
                    else link.removeAttribute("aria-current");
                });
            }
        }, { rootMargin: "-15% 0px -55% 0px", threshold: 0 });
        document.querySelectorAll("main section[id]").forEach((section) => observer.observe(section));
    }
}

// The contact form now posts directly to MVC and works without JavaScript.
