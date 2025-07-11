 document.addEventListener("DOMContentLoaded", function () {
   document.querySelectorAll('.navbar-nav .nav-link').forEach(link => {
    link.addEventListener('click', () => {
      const navbarCollapse = document.querySelector('.navbar-collapse');
      const bsCollapse = bootstrap.Collapse.getInstance(navbarCollapse);

      if (bsCollapse) {
        bsCollapse.hide();
      }
    });
  });
  
   function formatNumber(num) {
  return num.toLocaleString() + '+';
  }

  function animateCounters() {
    const counters = document.querySelectorAll('.counter');
    counters.forEach(counter => {
      const target = +counter.getAttribute('data-target');
      let count = 0;

      const updateCount = () => {
        const current = +counter.innerText.replace(/[+,]/g, '');
        const increment = target / 100;

        if (current < target) {
          const newCount = Math.ceil(current + increment);
          counter.innerText = formatNumber(newCount);
          setTimeout(updateCount, 20);
        } else {
          counter.innerText = formatNumber(target);
        }
      };
      counter.innerText = '0+';
      updateCount();
    });
  }


  const section = document.querySelector('.counter-section');
  let observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        animateCounters();
      }
    });
  }, {
    threshold: 0.5 
  });

  if (section) {
    observer.observe(section);
  }
  const swiper = new Swiper('.mySwiper', {
        slidesPerView: 1,
        spaceBetween: 20,
        loop: true,
        grabCursor: true, 
        autoplay: {
            delay: 3000,
            disableOnInteraction: false,
        },
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            576: {
                slidesPerView: 2,
            },
            768: {
                slidesPerView: 2,
            },
            992: {
                slidesPerView: 3,
            },
            1200: {
                slidesPerView: 4,
            }
        }
    });
      window.onload = function () {
  window.scrollTo(0, 0);
};
 const navbar = document.querySelector(".navbar");
 const scrollToTopBtn = document.getElementById("scrollToTopBtn");
  window.addEventListener("scroll", () => {
    scrollToTopBtn.style.display = window.scrollY > 300 ? "flex" : "none";
    navbar.classList.toggle("scrolled", window.scrollY > 50);
  });

  scrollToTopBtn.addEventListener("click", () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  });
   const phoneToggleBtn = document.getElementById("phoneToggleBtn");
  const phoneDropdown = document.getElementById("phoneDropdown");

  phoneToggleBtn?.addEventListener("click", () => {
    phoneDropdown.style.display = phoneDropdown.style.display === "flex" ? "none" : "flex";
  });

  document.addEventListener("click", (e) => {
    if (!phoneToggleBtn?.contains(e.target) && !phoneDropdown?.contains(e.target)) {
      phoneDropdown.style.display = "none";
    }
  });


    AOS.init({
    offset: 120,
    duration: 1000,
    easing: 'ease-in-out',
  });
});
