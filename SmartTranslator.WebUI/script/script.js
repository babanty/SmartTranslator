let translationEntity; // Глобальная переменная для хранения Id
let languageFrom = ""; // Глобальная переменная для исходного языка
let languageTo = ""; // Глобальная переменная для целевого языка
let TranslationStyle = "";

window.onload = function () {
  fetch("http://localhost:3000/api/translation")
    .then((response) => {
      if (!response.ok) {
        throw new Error("Ошибка запроса");
      }
      return response.json();
    })
    .then((data) => {
      translationEntity = data;
      console.log("Полученный Id:", translationEntity);
    })
    .catch((error) => {
      console.error("Произошла ошибка:", error);
    });
};
document.addEventListener("DOMContentLoaded", function () {
  window.sendText = function () {
    // Собираем информацию из полей
    const defaultId = "defaultId";
    const translationText = document.querySelector(".source_text").value;

    const context = document.querySelector(".context_text").value;
    const textData = {
      id: defaultId,
      BaseText: translationText,
      LanguageFrom: languageFrom,
      LanguageTo: languageTo,
      Context: context,
      TranslationStyle: TranslationStyle,
    };

    fetch("http://localhost:3000/api/translation", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(textData),
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Ошибка запроса: " + response.statusText);
        }
        return response.json();
      })
      .then((data) => {
        console.log("Ответ сервера:", data);
      })
      .catch((error) => {
        console.error("Произошла ошибка:", error);
      });
  };
});

const textareas = document.querySelectorAll("textarea");

document.addEventListener("DOMContentLoaded", function () {
  textareas.forEach((textarea) => {
    textarea.addEventListener("input", function () {
      const parent =
        this.closest(".context") ||
        this.closest(".source") ||
        this.closest(".popup__input-section") ||
        this.closest(".popup__title-box") ||
        this.closest(".setting-context__field_mobile") ||
        this.closest(".context__input-wrapper_mobile") ||
        this.closest(".source_mobile");

      if (parent) {
        let counterCurrent =
          parent.querySelector(".counterCurrent") ||
          parent.querySelector(".counterCurrent_mobile");
        let counterLimit =
          parent.querySelector(".counterLimit") ||
          parent.querySelector(".counterLimit_mobile");

        if (counterCurrent && counterLimit) {
          const currentLength = this.value.length;
          const maxLength = parseInt(counterLimit.textContent.split("/")[1]);

          if (currentLength > maxLength) {
            this.value = this.value.substring(0, maxLength);
          }

          counterCurrent.textContent = this.value.length;
        }
      }
    });

    if (!textarea.closest(".popup__title-box")) {
      textarea.style.overflow = "hidden";
    }
  });
});

/* 
2. Регулировка размера шрифта для textarea.
// */
document.addEventListener("DOMContentLoaded", function () {
  textareas.forEach((textarea) => {
    const initialFontSize = parseInt(
      window.getComputedStyle(textarea).fontSize
    );
    const initialHeight = textarea.clientHeight;
    let currentDecrements = 0;
    let previousTextLength = textarea.value.length;

    if (textarea.hasAttribute("readonly")) {
      adjustFontSize();
    } else {
      textarea.addEventListener("input", adjustFontSize);
    }

    function adjustFontSize() {
      let currentFontSize = parseInt(
        window.getComputedStyle(textarea).fontSize
      );
      const isTextBeingRemoved = textarea.value.length < previousTextLength;

      if (!isTextBeingRemoved) {
        while (
          textarea.scrollHeight > textarea.clientHeight &&
          currentDecrements < 3
        ) {
          textarea.style.fontSize = currentFontSize - 2 + "px";
          currentFontSize -= 2;
          currentDecrements++;
        }

        // Если размер шрифта достиг своего минимального значения, применяем autosize
        if (currentDecrements >= 3) {
          autosize(textarea);
        }
      } else {
        if (textarea.clientHeight <= initialHeight) {
          while (
            textarea.scrollHeight <= textarea.clientHeight &&
            currentFontSize < initialFontSize
          ) {
            textarea.style.fontSize = currentFontSize + 2 + "px";
            currentFontSize += 2;
            currentDecrements--;
          }
        }

        // Если текст был удален и размер шрифта достиг максимума, применяем autosize
        if (currentFontSize == initialFontSize) {
          autosize(textarea);
        }
      }

      previousTextLength = textarea.value.length;
    }
  });
});

/* 
3. Обработка выпадающего меню (dropdown).
*/

document.addEventListener("DOMContentLoaded", function () {
  let languageFrom, languageTo;

  // 1. Инициализация languageFrom и languageTo на основе текущих значений заголовков
  const fromTitleElement = document.querySelector(
    ".language_from .lang-box__title"
  );
  const toTitleElement = document.querySelector(
    ".language_to .lang-box__title"
  );

  if (fromTitleElement) {
    languageFrom = fromTitleElement.textContent;
  }

  if (toTitleElement) {
    languageTo = toTitleElement.textContent;
  }

  // 2. Функция для управления поведением выпадающего списка
  function dropdownFunctionality(
    toggleClass,
    arrowClass,
    menuClass,
    itemClass,
    titleClass
  ) {
    const toggles = document.querySelectorAll(toggleClass);

    toggles.forEach((toggle) => {
      toggle.addEventListener("click", function () {
        const list = this.nextElementSibling;
        const arrowElement = this.querySelector(arrowClass);
        if (arrowElement) {
          arrowElement.classList.toggle("flipped");
        }

        if (parseFloat(getComputedStyle(list).opacity) === 1) {
          list.style.opacity = "0";
          list.style.maxHeight = "0";
        } else {
          list.style.opacity = "1";
          list.style.maxHeight = "max-content";
        }
      });
    });

    // Закрытие dropdown при клике вне его области
    document.addEventListener("click", function (event) {
      const isClickInside =
        event.target.closest(toggleClass) || event.target.closest(menuClass);
      if (!isClickInside) {
        const allLists = document.querySelectorAll(menuClass);
        allLists.forEach((el) => {
          el.style.opacity = "0";
          el.style.maxHeight = "0";
        });

        const allArrows = document.querySelectorAll(arrowClass);
        allArrows.forEach((arrow) => arrow.classList.remove("flipped"));
      }
    });

    // Обновление заголовка dropdown и переменных languageFrom и languageTo
    const listItems = document.querySelectorAll(itemClass);
    listItems.forEach((item) => {
      item.addEventListener("click", function () {
        const parentMenu = this.closest(menuClass);
        const parentToggle = parentMenu
          ? parentMenu.previousElementSibling
          : null;

        if (parentToggle) {
          const titleElement = parentToggle.querySelector(titleClass);
          if (titleElement) {
            const oldTitle = titleElement.textContent;
            titleElement.textContent = this.textContent;
            this.textContent = oldTitle;

            if (parentToggle.classList.contains("language_from")) {
              languageFrom = titleElement.textContent;
            } else if (parentToggle.classList.contains("language_to")) {
              languageTo = titleElement.textContent;
            }
          }

          this.closest(menuClass).style.opacity = "0";
          this.closest(menuClass).style.maxHeight = "0";
          const arrowElement = parentToggle.querySelector(arrowClass);
          if (arrowElement) {
            arrowElement.classList.remove("flipped");
          }
        }
      });
    });
  }

  // Вызов функции dropdownFunctionality с нужными селекторами
  dropdownFunctionality(
    ".dropdown-toggle",
    ".style__info, .lang-box__arrow",
    ".dropdown-menu",
    ".style__item, .lang-box__item",
    ".style__title, .lang-box__title"
  );

  dropdownFunctionality(
    ".dropdown-toggle_mobile",
    ".style__info_mobile, .lang-box__arrow_mobile",
    ".dropdown-menu_mobile",
    ".style__item_mobile, .lang-box__item_mobile",
    ".style__title_mobile, .lang-box__title_mobile"
  );
});

/* 
4. Обработка кнопок копирования текста.
*/
document.addEventListener("DOMContentLoaded", function () {
  // Обновим селекторы для кнопок копирования на обоих версиях
  const copyButtons = document.querySelectorAll(
    ".source__copy, .target__copy, .source__copy_mobile, .target__copy_mobile"
  );

  copyButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.preventDefault();

      let textarea;

      if (button.classList.contains("source__copy")) {
        textarea = button
          .closest(".source")
          .querySelector(".translator-box__text");
      } else if (button.classList.contains("target__copy")) {
        textarea = button
          .closest(".target")
          .querySelector(".translator-box__text");
      } else if (button.classList.contains("source__copy_mobile")) {
        textarea = button
          .closest(".source_mobile")
          .querySelector(".translator-box__text_mobile");
      } else {
        textarea = button
          .closest(".target_mobile")
          .querySelector(".translator-box__text_mobile");
      }

      if (textarea) {
        navigator.clipboard
          .writeText(textarea.value || textarea.textContent)
          .then(function () {
            console.log("Text successfully copied to clipboard!");
          })
          .catch(function (err) {
            console.error("Could not copy text: ", err);
          });

        // Добавим анимацию для мобильной версии
        if (
          button.classList.contains("source__copy_mobile") ||
          button.classList.contains("target__copy_mobile")
        ) {
          button.classList.add("pulse-animation_mobile");
          button.addEventListener("animationend", function () {
            button.classList.remove("pulse-animation_mobile");
          });
        }
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

document.addEventListener("DOMContentLoaded", function () {
  const customButton = document.querySelector(
    ".context__button--custom_mobile"
  );
  const inputWrapper = document.querySelector(".context__input-wrapper_mobile");

  customButton.addEventListener("click", function () {
    // Переключение класса "active_mobile" для появления или скрытия inputWrapper
    inputWrapper.classList.toggle("active_mobile");
    this.classList.toggle("active_mobile");
  });
});
// настройки
document.addEventListener("DOMContentLoaded", function () {
  const titleBox = document.querySelector(".setting__title-box_mobile");
  const content = document.querySelector(".setting__content_mobile");
  const arrow = document.querySelector(".settings__title-button_mobile");
  const translateButton = document.querySelector(".source__button_mobile");
  const target = document.querySelector(".target_mobile");

  if (titleBox && content && arrow) {
    titleBox.addEventListener("click", function () {
      content.classList.toggle("active_mobile");
      arrow.classList.toggle("rotated_mobile");
    });
  }

  if (translateButton && target) {
    translateButton.addEventListener("click", function () {
      target.classList.add("active_mobile");
    });
  }
});

function openPopup(data) {
  // Заполнение текстовой области данными, если это необходимо
  const question = data.Contexts[0].Question;

  // Заполнение текстовой области данными
  document.querySelector(".popup__correction").value = question;
  document.querySelector(".context__correction_mobile").value = question;
  // Отображение поп-ап окна
  document.querySelector(".popup").classList.add("popup_active");
  document
    .querySelector(".context_mobile")
    .classList.add("context_mobile_active");
}

document.addEventListener("DOMContentLoaded", function () {
  document.body.classList.remove("noscroll");

  let closePopupFunction = function (popupElement) {
    if (popupElement) {
      popupElement.classList.remove("popup_active");
    }
  };

  let closeMobilePopupFunction = function (popupElement) {
    if (popupElement) {
      popupElement.classList.remove("popup_active_mobile");
    }
  };

  let buttons = [
    ".popup__button--yes",
    ".popup__button--no",
    ".popup__button--unsure",
    ".popup__icon-button",
    ".button-yes_mobile",
    ".button-no_mobile",
  ];

  buttons.forEach((selector) => {
    let button = document.querySelector(selector);
    if (button) {
      if (selector.includes("_mobile")) {
        button.addEventListener("click", () =>
          closeMobilePopupFunction(document.querySelector(".popup_mobile"))
        );
      } else {
        button.addEventListener("click", () =>
          closePopupFunction(document.querySelector(".popup"))
        );
      }
    }
  });

  // Добавление обработчиков событий для кнопок, чтобы вызывать fetchData при клике
  document.querySelector(".translator-box__button").addEventListener("click", fetchData);
  document.querySelector(".source__button_mobile").addEventListener("click", fetchData);
});

function fetchData() {
  fetch("http://localhost:3000/api/translation", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      // ваш запрос
    }),
  })
    .then((response) => response.json())
    .then((data) => {
      console.log(data); // выводим полученные данные в консоль для отладки
      if (
        data.State === "WaitingForContext" &&
        data.Contexts[0].Response === null
      ) {
        openPopup(data); // открытие поп-ап окна, если состояние WaitingForContext
      }
    })
    .catch((error) => {
      console.error("Error:", error);
    });
}

