// Loader al cargar página
window.addEventListener("load", function () {
    const loader = document.getElementById("page-loader");
    if (loader) {
        loader.style.opacity = "0";
        loader.style.transition = "opacity 0.8s ease";
        setTimeout(() => loader.style.display = "none", 800);
    }

    // Aplicar efecto fade-in al body
    document.body.style.opacity = 0;
    document.body.style.transition = "opacity 0.8s ease";
    setTimeout(() => document.body.style.opacity = 1, 100);
});

// Fade-out al navegar con enlaces
document.querySelectorAll("a").forEach(link => {
    link.addEventListener("click", function (e) {
        const href = this.getAttribute("href");

        // Solo si es un enlace válido y no tiene target _blank ni ancla
        if (
            href &&
            !href.startsWith("#") &&
            !this.hasAttribute("target")
        ) {
            e.preventDefault();
            document.body.style.opacity = 0;
            setTimeout(() => {
                window.location.href = href;
            }, 500);
        }
    });
});
