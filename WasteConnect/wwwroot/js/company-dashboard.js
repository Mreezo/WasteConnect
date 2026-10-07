document.addEventListener("DOMContentLoaded", function () {

    const slides =
        document.querySelectorAll(".company-slide");

    const dots =
        document.querySelectorAll(".slider-dot");

    const previousButton =
        document.querySelector(".slider-prev");

    const nextButton =
        document.querySelector(".slider-next");


    if (slides.length === 0) {
        return;
    }


    let currentSlide = 0;

    let autoSlide;


    function showSlide(index) {

        if (index >= slides.length) {
            index = 0;
        }

        if (index < 0) {
            index = slides.length - 1;
        }


        slides.forEach(slide => {
            slide.classList.remove("active");
        });


        dots.forEach(dot => {
            dot.classList.remove("active");
        });


        slides[index].classList.add("active");

        if (dots[index]) {
            dots[index].classList.add("active");
        }


        currentSlide = index;
    }


    function nextSlide() {

        showSlide(currentSlide + 1);
    }


    function previousSlide() {

        showSlide(currentSlide - 1);
    }


    function startAutoSlide() {

        autoSlide = setInterval(function () {

            nextSlide();

        }, 6000);
    }


    function restartAutoSlide() {

        clearInterval(autoSlide);

        startAutoSlide();
    }


    if (nextButton) {

        nextButton.addEventListener("click", function () {

            nextSlide();

            restartAutoSlide();

        });
    }


    if (previousButton) {

        previousButton.addEventListener("click", function () {

            previousSlide();

            restartAutoSlide();

        });
    }


    dots.forEach((dot, index) => {

        dot.addEventListener("click", function () {

            showSlide(index);

            restartAutoSlide();

        });

    });


    showSlide(0);

    startAutoSlide();

});