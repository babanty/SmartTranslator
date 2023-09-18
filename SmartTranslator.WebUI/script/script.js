/* 
1. Обработка текстовых областей (textarea) и их счетчиков.
*/
const textareas = document.querySelectorAll("textarea");
textareas.forEach((textarea) => {
  textarea.addEventListener("input", function () {
    const parent =
      this.closest(".context") ||
      this.closest(".source") ||
      this.closest(".popup__input-section") ||
      this.closest(".popup__title-box");

    if (parent) {
      const counterCurrent = parent.querySelector(".counterCurrent");
      const counterLimit = parent.querySelector(".counterLimit");

      if (counterCurrent) {
        const currentLength = this.value.length;
        const maxLength = parseInt(counterLimit.textContent.split("/")[1]);

        if (currentLength > maxLength) {
          this.value = this.value.substring(0, maxLength);
        }

        counterCurrent.textContent = this.value.length;
      }
    }
    adjustFontSize(this);
  });

  if (!textarea.closest(".popup__title-box")) {
    textarea.style.overflow = "hidden";
  }
});

/* 
2. Регулировка размера шрифта для textarea.
*/
function adjustFontSize(textarea) {
  const style = getComputedStyle(textarea);
  let fontSize = parseInt(style.fontSize);

  if (textarea.scrollHeight > textarea.clientHeight) {
    if (fontSize > 16) {
      fontSize -= 2;
      textarea.style.fontSize = fontSize + "px";
      adjustFontSize(textarea);
    } else {
      textarea.style.height = textarea.scrollHeight + "px";
    }
  } else if (textarea.scrollHeight <= textarea.clientHeight && fontSize < 24) {
    fontSize += 2;
    textarea.style.fontSize = fontSize + "px";
    if (
      textarea.scrollHeight <= textarea.clientHeight &&
      textarea.clientHeight > 320
    ) {
      textarea.style.height = "320px";
    }
  }
}
textareas.forEach((textarea) => {
  adjustFontSize(textarea);
});

/* 
3. Обработка выпадающего меню (dropdown).
*/
document.addEventListener("DOMContentLoaded", function () {
  const toggles = document.querySelectorAll(".dropdown-toggle");
  toggles.forEach((toggle) => {
    toggle.addEventListener("click", function () {
      const list = this.nextElementSibling;

      if (parseFloat(getComputedStyle(list).opacity) === 1) {
        list.style.opacity = "0";
        list.style.maxHeight = "0";
        this.querySelector(".lang-box__arrow, .style__info").classList.remove(
          "flipped"
        );
      } else {
        const allLists = document.querySelectorAll(
          ".dropdown-menut, .dropdown-menu"
        );
        allLists.forEach((el) => {
          el.style.opacity = "0";
          el.style.maxHeight = "0";
        });

        const allArrows = document.querySelectorAll(
          ".lang-box__arrow, .style__info"
        );
        allArrows.forEach((arrow) => arrow.classList.remove("flipped"));

        list.style.opacity = "1";
        list.style.maxHeight = "70px";
        this.querySelector(".lang-box__arrow, .style__info").classList.add(
          "flipped"
        );
      }
    });
  });

  document.addEventListener("click", function (event) {
    const isClickInside =
      event.target.closest(".dropdown-toggle") ||
      event.target.closest(".dropdown-menut") ||
      event.target.closest(".dropdown-menu");

    if (!isClickInside) {
      const allLists = document.querySelectorAll(
        ".dropdown-menut, .dropdown-menu"
      );
      allLists.forEach((el) => {
        el.style.opacity = "0";
        el.style.maxHeight = "0";
      });

      const allArrows = document.querySelectorAll(
        ".lang-box__arrow, .style__info"
      );
      allArrows.forEach((arrow) => arrow.classList.remove("flipped"));
    }
  });

  const listItems = document.querySelectorAll(".lang-box__item, .style__item");
  listItems.forEach((item) => {
    item.addEventListener("click", function () {
      const titleElement = this.closest(".lang-box, .style").querySelector(
        ".lang-box__title, .style__title"
      );
      const oldTitle = titleElement.textContent;
      titleElement.textContent = this.textContent;
      this.textContent = oldTitle;
      this.closest(".dropdown-menut, .dropdown-menu").style.opacity = "0";
      this.closest(".dropdown-menut, .dropdown-menu").style.maxHeight = "0";
      this.closest(".lang-box, .style")
        .querySelector(".lang-box__arrow, .style__info")
        .classList.remove("flipped");
    });
  });
});

/* 
4. Обработка кнопок копирования текста.
*/
document.addEventListener("DOMContentLoaded", function () {
  const copyButtons = document.querySelectorAll(".source__copy, .target__copy");

  copyButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.preventDefault();

      let textarea;

      if (button.classList.contains("source__copy")) {
        textarea = button
          .closest(".source")
          .querySelector(".translator-box__text");
      } else {
        textarea = button
          .closest(".target")
          .querySelector(".translator-box__text");
      }

      if (textarea) {
        textarea.select();
        document.execCommand("copy");

        textarea.blur();
        window.getSelection().removeAllRanges();

        button.classList.add("pulse-animation");
        button.addEventListener("animationend", function () {
          button.classList.remove("pulse-animation");
        });
      }
    });
  });
});

/* 
5. Обработка пользовательской кнопки во всплывающем окне.
*/
document.addEventListener("DOMContentLoaded", function () {
  const customButton = document.querySelector(".popup__button--custom");
  const inputSection = document.querySelector(".popup__input-section");

  customButton.addEventListener("click", () => {
    const isActive = customButton.classList.toggle("active");
    inputSection.style.opacity = isActive ? "1" : "0";
    inputSection.style.pointerEvents = isActive ? "auto" : "none";
  });

  document
    .querySelectorAll(".popup__button:not(.popup__button--custom)")
    .forEach((button) => {
      button.addEventListener("click", () => {
        inputSection.style.opacity = "0";
        inputSection.style.pointerEvents = "none";
        customButton.classList.remove("active");
      });
    });
});

/* 
6. Обработка закрытия всплывающего окна.
*/
document.addEventListener("DOMContentLoaded", function () {
  document.body.classList.remove("no-scroll");
  let popup = document.querySelector(".popup");
  let closePopup = function () {
    popup.classList.remove("popup_active");
  };
  let buttons = [
    ".popup__button--yes",
    ".popup__button--no",
    ".popup__button--unsure",
    ".popup__icon-button",
  ];
  buttons.forEach((selector) => {
    let button = document.querySelector(selector);
    if (button) {
      button.addEventListener("click", closePopup);
    }
  });
});
