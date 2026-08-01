document.addEventListener("DOMContentLoaded", function () {
    const filterButtons =
        document.querySelectorAll(".report-filter");

    const reportRows =
        document.querySelectorAll(".ward-report-row");

    const filteredEmptyState =
        document.getElementById("filteredEmptyState");

    if (filterButtons.length === 0 || reportRows.length === 0) {
        return;
    }

    filterButtons.forEach(button => {
        button.addEventListener("click", function () {
            filterButtons.forEach(item =>
                item.classList.remove("active"));

            this.classList.add("active");

            const selectedFilter =
                this.dataset.filter;

            let visibleRows = 0;

            reportRows.forEach(row => {
                const status =
                    row.dataset.status;

                const priority =
                    row.dataset.priority;

                let shouldShow = false;

                switch (selectedFilter) {
                    case "all":
                        shouldShow = true;
                        break;

                    case "high":
                        shouldShow = priority === "high";
                        break;

                    case "in-progress":
                        shouldShow =
                            status === "in-progress" ||
                            status === "inprogress";
                        break;

                    default:
                        shouldShow = status === selectedFilter;
                        break;
                }

                row.style.display =
                    shouldShow ? "" : "none";

                if (shouldShow) {
                    visibleRows++;
                }
            });

            if (filteredEmptyState) {
                filteredEmptyState.style.display =
                    visibleRows === 0
                        ? "block"
                        : "none";
            }
        });
    });
});