document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // ELEMENTS
    // =====================================================

    const modal =
        document.getElementById("communityAlertModal");

    const closeButton =
        document.getElementById("closeAlertModal");

    const cancelButton =
        document.getElementById("cancelAlertModal");

    const modalTitle =
        document.getElementById("modalAlertTitle");

    const modalIcon =
        document.getElementById("modalAlertIcon");

    const form =
        document.getElementById("communityAlertForm");

    const alertIdInput =
        document.getElementById("alertId");

    const alertTypeInput =
        document.getElementById("alertType");

    const areaType =
        document.getElementById("areaType");

    const wardField =
        document.getElementById("wardField");

    const specificAreaField =
        document.getElementById("specificAreaField");

    const wardNumber =
        document.getElementById("wardNumber");

    const specificArea =
        document.getElementById("specificArea");

    const alertReason =
        document.getElementById("alertReason");

    const alertDescription =
        document.getElementById("alertDescription");

    const startDateTime =
        document.getElementById("startDateTime");

    const endDateTime =
        document.getElementById("endDateTime");

    const priority =
        document.getElementById("priority");

    const publishButton =
        form.querySelector(".publish-btn");


    // =====================================================
    // IMPORTANT
    //
    // ONLY buttons with .open-alert-modal can create alerts.
    //
    // We DO NOT use:
    // document.querySelectorAll("[data-alert-type]")
    //
    // because Edit, Status and Delete also have
    // data-alert-type.
    // =====================================================

    const createAlertButtons =
        document.querySelectorAll(".open-alert-modal");

    const editButtons =
        document.querySelectorAll(".edit-alert-btn");

    const statusSelects =
        document.querySelectorAll(".status-select");

    const deleteButtons =
        document.querySelectorAll(".delete-alert-btn");


    // =====================================================
    // CREATE / EDIT MODE
    // =====================================================

    let editingAlert = false;


    // =====================================================
    // OPEN CREATE ALERT MODAL
    // =====================================================

    createAlertButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            function (event) {

                event.preventDefault();
                event.stopPropagation();

                resetAlertForm();

                editingAlert = false;

                const alertType =
                    button.dataset.alertType;

                alertTypeInput.value =
                    alertType;

                configureModal(
                    alertType,
                    false
                );

                modal.classList.add("active");

                document.body.style.overflow =
                    "hidden";
            }
        );

    });


    // =====================================================
    // CONFIGURE MODAL
    // =====================================================

    function configureModal(
        alertType,
        isEditing
    ) {

        if (alertType === "Electricity") {

            modalIcon.textContent = "⚡";

            modalTitle.textContent =
                isEditing
                    ? "Edit Electricity Alert"
                    : "Create Electricity Alert";
        }

        else if (alertType === "Water") {

            modalIcon.textContent = "💧";

            modalTitle.textContent =
                isEditing
                    ? "Edit Water Alert"
                    : "Create Water Alert";
        }

        else if (
            alertType === "Bin Collection"
        ) {

            modalIcon.textContent = "🗑️";

            modalTitle.textContent =
                isEditing
                    ? "Edit Bin Collection Alert"
                    : "Create Bin Collection Alert";
        }
    }


    // =====================================================
    // AREA TYPE
    // =====================================================

    areaType.addEventListener(
        "change",
        updateAreaFields
    );


    function updateAreaFields() {

        wardField.style.display =
            "none";

        specificAreaField.style.display =
            "none";


        if (areaType.value === "Ward") {

            wardField.style.display =
                "block";
        }

        else if (
            areaType.value === "Area"
        ) {

            specificAreaField.style.display =
                "block";
        }
    }


    // =====================================================
    // RESET FORM
    // =====================================================

    function resetAlertForm() {

        form.reset();

        alertIdInput.value = "";

        alertTypeInput.value = "";

        editingAlert = false;

        wardField.style.display =
            "none";

        specificAreaField.style.display =
            "none";

        publishButton.textContent =
            "Publish Alert";
    }


    // =====================================================
    // CLOSE MODAL
    // =====================================================

    function closeModal() {

        modal.classList.remove("active");

        document.body.style.overflow =
            "";

        resetAlertForm();
    }


    closeButton.addEventListener(
        "click",
        function (event) {

            event.preventDefault();

            closeModal();
        }
    );


    cancelButton.addEventListener(
        "click",
        function (event) {

            event.preventDefault();

            closeModal();
        }
    );


    modal.addEventListener(
        "click",
        function (event) {

            if (event.target === modal) {

                closeModal();
            }
        }
    );


    document.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Escape" &&
                modal.classList.contains(
                    "active"
                )
            ) {

                closeModal();
            }
        }
    );


    // =====================================================
    // CREATE OR EDIT ALERT
    // =====================================================

    form.addEventListener(
        "submit",
        async function (event) {

            event.preventDefault();

            const originalText =
                publishButton.textContent;

            publishButton.disabled =
                true;

            publishButton.textContent =
                editingAlert
                    ? "Saving..."
                    : "Publishing...";


            try {

                const formData =
                    new FormData(form);


                const endpoint =
                    editingAlert
                        ? "/Admin/EditCommunityAlert"
                        : "/Admin/PublishCommunityAlert";


                const response =
                    await fetch(
                        endpoint,
                        {
                            method: "POST",
                            body: formData
                        }
                    );


                const responseText =
                    await response.text();


                console.log(
                    "Server response:",
                    responseText
                );


                let result;


                try {

                    result =
                        JSON.parse(
                            responseText
                        );
                }
                catch {

                    throw new Error(
                        responseText
                    );
                }


                if (!response.ok) {

                    alert(
                        result.message ||
                        "Unable to save the alert."
                    );

                    return;
                }


                alert(
                    result.message
                );


                window.location.reload();

            }
            catch (error) {

                console.error(
                    "Community alert error:",
                    error
                );


                alert(
                    error.message ||
                    "Something went wrong while saving the alert."
                );

            }
            finally {

                publishButton.disabled =
                    false;

                publishButton.textContent =
                    originalText;
            }
        }
    );


    // =====================================================
    // EDIT ALERT
    //
    // IMPORTANT:
    // This listener is completely separate from
    // createAlertButtons.
    // =====================================================

    editButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            function (event) {

                event.preventDefault();
                event.stopPropagation();


                editingAlert = true;


                const alertType =
                    button.dataset.alertType;


                alertIdInput.value =
                    button.dataset.id || "";

                alertTypeInput.value =
                    alertType;


                areaType.value =
                    button.dataset.areaType || "";


                wardNumber.value =
                    button.dataset.ward || "";


                specificArea.value =
                    button.dataset.area || "";


                alertReason.value =
                    button.dataset.reason || "";


                alertDescription.value =
                    button.dataset.description || "";


                startDateTime.value =
                    button.dataset.start || "";


                endDateTime.value =
                    button.dataset.end || "";


                priority.value =
                    button.dataset.priority ||
                    "Normal";


                updateAreaFields();


                configureModal(
                    alertType,
                    true
                );


                publishButton.textContent =
                    "Save Changes";


                /*
                 * IMPORTANT:
                 *
                 * Edit intentionally opens the alert form
                 * in EDIT MODE.
                 *
                 * It will NOT call PublishCommunityAlert.
                 *
                 * It calls EditCommunityAlert instead.
                 */

                modal.classList.add(
                    "active"
                );

                document.body.style.overflow =
                    "hidden";
            }
        );

    });


    // =====================================================
    // UPDATE STATUS
    //
    // DOES NOT OPEN MODAL
    // =====================================================

    statusSelects.forEach(function (select) {

        select.addEventListener(
            "change",
            async function (event) {

                event.preventDefault();
                event.stopPropagation();


                const previousStatus =
                    select.dataset.currentStatus ||
                    "";


                select.disabled =
                    true;


                const formData =
                    new FormData();


                formData.append(
                    "id",
                    select.dataset.id
                );


                formData.append(
                    "alertType",
                    select.dataset.alertType
                );


                formData.append(
                    "status",
                    select.value
                );


                const tokenInput =
                    document.querySelector(
                        'input[name="__RequestVerificationToken"]'
                    );


                if (tokenInput) {

                    formData.append(
                        "__RequestVerificationToken",
                        tokenInput.value
                    );
                }


                try {

                    const response =
                        await fetch(
                            "/Admin/UpdateCommunityAlertStatus",
                            {
                                method: "POST",
                                body: formData
                            }
                        );


                    const responseText =
                        await response.text();


                    let result;


                    try {

                        result =
                            JSON.parse(
                                responseText
                            );
                    }
                    catch {

                        throw new Error(
                            responseText
                        );
                    }


                    if (!response.ok) {

                        alert(
                            result.message ||
                            "Unable to update alert status."
                        );


                        if (previousStatus) {

                            select.value =
                                previousStatus;
                        }


                        return;
                    }


                    window.location.reload();

                }
                catch (error) {

                    console.error(
                        "Status update error:",
                        error
                    );


                    if (previousStatus) {

                        select.value =
                            previousStatus;
                    }


                    alert(
                        "Unable to update the alert status."
                    );

                }
                finally {

                    select.disabled =
                        false;
                }
            }
        );

    });


    // =====================================================
    // DELETE ALERT
    //
    // DOES NOT OPEN COMMUNITY ALERT MODAL
    // =====================================================

    deleteButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            async function (event) {

                event.preventDefault();
                event.stopPropagation();


                const confirmed =
                    confirm(
                        "Are you sure you want to delete this community alert?"
                    );


                if (!confirmed) {

                    return;
                }


                button.disabled =
                    true;


                const formData =
                    new FormData();


                formData.append(
                    "id",
                    button.dataset.id
                );


                formData.append(
                    "alertType",
                    button.dataset.alertType
                );


                const tokenInput =
                    document.querySelector(
                        'input[name="__RequestVerificationToken"]'
                    );


                if (tokenInput) {

                    formData.append(
                        "__RequestVerificationToken",
                        tokenInput.value
                    );
                }


                try {

                    const response =
                        await fetch(
                            "/Admin/DeleteCommunityAlert",
                            {
                                method: "POST",
                                body: formData
                            }
                        );


                    const responseText =
                        await response.text();


                    let result;


                    try {

                        result =
                            JSON.parse(
                                responseText
                            );
                    }
                    catch {

                        throw new Error(
                            responseText
                        );
                    }


                    if (!response.ok) {

                        alert(
                            result.message ||
                            "Unable to delete the alert."
                        );

                        return;
                    }


                    alert(
                        result.message ||
                        "Community alert deleted successfully."
                    );


                    window.location.reload();

                }
                catch (error) {

                    console.error(
                        "Delete alert error:",
                        error
                    );


                    alert(
                        "Unable to delete the community alert."
                    );

                }
                finally {

                    button.disabled =
                        false;
                }
            }
        );

    });

});