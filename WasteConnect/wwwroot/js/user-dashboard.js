document.addEventListener("DOMContentLoaded", function () {

    console.log("WasteConnect User Dashboard JavaScript loaded.");

    const alertBoxes =
        document.querySelectorAll(".community-alert-box");

    const alertModals =
        document.querySelectorAll(".community-alert-modal");

    const closeButtons =
        document.querySelectorAll(".close-alert-modal");


    console.log("Alert boxes found:", alertBoxes.length);
    console.log("Alert modals found:", alertModals.length);


    // ==========================================
    // OPEN MODAL
    // ==========================================

    alertBoxes.forEach(function (box) {

        box.addEventListener("click", function () {

            console.log("Alert box clicked.");

            const targetId =
                box.getAttribute("data-alert-target");

            console.log("Opening modal:", targetId);

            const targetModal =
                document.getElementById(targetId);


            if (!targetModal) {

                console.error(
                    "Modal not found:",
                    targetId
                );

                return;
            }


            // Close all modals first
            alertModals.forEach(function (modal) {

                modal.classList.remove("active");

            });


            // Remove selected state
            alertBoxes.forEach(function (item) {

                item.classList.remove("selected");

            });


            // Open selected modal
            targetModal.classList.add("active");

            box.classList.add("selected");

            document.body.classList.add(
                "alert-modal-open"
            );

        });

    });


    // ==========================================
    // CLOSE MODAL
    // ==========================================

    closeButtons.forEach(function (button) {

        button.addEventListener("click", function (event) {

            event.stopPropagation();

            const modal =
                button.closest(".community-alert-modal");


            if (modal) {

                modal.classList.remove("active");

            }


            alertBoxes.forEach(function (box) {

                box.classList.remove("selected");

            });


            document.body.classList.remove(
                "alert-modal-open"
            );

        });

    });


    // ==========================================
    // CLICK OUTSIDE POPUP
    // ==========================================

    alertModals.forEach(function (modal) {

        modal.addEventListener("click", function (event) {

            if (event.target === modal) {

                modal.classList.remove("active");


                alertBoxes.forEach(function (box) {

                    box.classList.remove("selected");

                });


                document.body.classList.remove(
                    "alert-modal-open"
                );

            }

        });

    });


    // ==========================================
    // ESCAPE KEY
    // ==========================================

    document.addEventListener("keydown", function (event) {

        if (event.key === "Escape") {

            alertModals.forEach(function (modal) {

                modal.classList.remove("active");

            });


            alertBoxes.forEach(function (box) {

                box.classList.remove("selected");

            });


            document.body.classList.remove(
                "alert-modal-open"
            );

        }

    });

});