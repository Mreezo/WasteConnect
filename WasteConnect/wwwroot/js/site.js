const roleCards = document.querySelectorAll(".role-card");

roleCards.forEach(card => {
    card.addEventListener("click", () => {
        roleCards.forEach(c => c.classList.remove("active"));
        card.classList.add("active");
    });
});