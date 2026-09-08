document.addEventListener("DOMContentLoaded", function () {

    const modal = document.getElementById("communityAlertModal");
    const closeButton = document.getElementById("closeAlertModal");
    const cancelButton = document.getElementById("cancelAlertModal");

    const modalTitle = document.getElementById("modalAlertTitle");
    const modalIcon = document.getElementById("modalAlertIcon");

    const alertTypeInput = document.getElementById("alertType");

    const areaType = document.getElementById("areaType");
    const wardField = document.getElementById("wardField");
    const specificAreaField = document.getElementById("specificAreaField");

    const alertButtons =
        document.querySelectorAll("[data-alert-type]");

    const form =
        document.getElementById("communityAlertForm");


    alertButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            const alertType =
                button.dataset.alertType;

            alertTypeInput.value = alertType;

            configureModal(alertType);

            modal.classList.add("active");

            document.body.style.overflow = "hidden";
        });

    });


    function configureModal(alertType) {

        if (alertType === "Electricity") {

            modalTitle.textContent =
                "Create Electricity Alert";

            modalIcon.textContent = "⚡";

        }
        else if (alertType === "Water") {

            modalTitle.textContent =
                "Create Water Alert";

            modalIcon.textContent = "💧";

        }
        else if (alertType === "Bin Collection") {

            modalTitle.textContent =
                "Create Bin Collection Alert";

            modalIcon.textContent = "🗑️";
        }

    }


    areaType.addEventListener("change", function () {

        const selectedValue =
            areaType.value;

        wardField.style.display = "none";
        specificAreaField.style.display = "none";

        if (selectedValue === "Ward") {

            wardField.style.display = "block";

        }
        else if (selectedValue === "Area") {

            specificAreaField.style.display = "block";
        }

    });


    function closeModal() {

        modal.classList.remove("active");

        document.body.style.overflow = "";

        form.reset();

        wardField.style.display = "none";
        specificAreaField.style.display = "none";
    }


    closeButton.addEventListener(
        "click",
        closeModal
    );


    cancelButton.addEventListener(
        "click",
        closeModal
    );


    modal.addEventListener("click", function (event) {

        if (event.target === modal) {

            closeModal();
        }

    });


    document.addEventListener("keydown", function (event) {

        if (event.key === "Escape" &&
            modal.classList.contains("active")) {

            closeModal();
        }

    });


    form.addEventListener("submit", async function (event) {

        event.preventDefault();

        const publishButton =
            form.querySelector(".publish-btn");

        const originalButtonText =
            publishButton.textContent;

        publishButton.disabled = true;
        publishButton.textContent = "Publishing...";

        try {

            const formData =
                new FormData(form);

            const response = await fetch(
                "/Admin/PublishCommunityAlert",
                {
                    method: "POST",
                    body: formData
                }
            );

            const result =
                await response.json();

            if (!response.ok) {

                alert(
                    result.message ||
                    "Unable to publish the alert."
                );

                return;
            }

            alert(result.message);

            closeModal();

        }
        catch (error) {

            console.error(
                "Community alert error:",
                error
            );

            alert(
                "Something went wrong while publishing the alert."
            );
        }
        finally {

            publishButton.disabled = false;
            publishButton.textContent =
                originalButtonText;
        }

    });

});