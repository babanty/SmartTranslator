const express = require("express");
const cors = require("cors");
const app = express();
const PORT = 3000;

app.use(cors());
app.use(express.json());

app.get("/api/translation", (req, res) => {
  const id = req.query.id || "defaultId";

  const entity = {
    Id: id,
    BaseText: "Sample base text",
    Translation: "Переведено",
    LanguageFrom: "English",
    LanguageTo: "Russian",
    TranslationStyle: "ConversationalStyle",
    Contexts: [
      {
        Id: 1,
        Question: "Sample question?",
        Response: "Sample response",
      },
    ],
    State: "finished",
  };

  res.send(entity);
});

app.post("/api/translation", (req, res) => {
  const translatedEntityTestData = {
    Id: req.body.id || "defaultId",
    BaseText: req.body.BaseText,
    Translation: req.body.Translation,
    LanguageFrom: req.body.LanguageFrom,
    LanguageTo: req.body.LanguageTo,
    TranslationStyle: "ConversationalStyle",
    Contexts: [
      {
        Id: 1,
        Question: null,
        Response: "Sample response",
      },
    ],
    State: "finished",
    Context: req.body.Context,
  };

  const waitingContextEntityTestData = {
    Id: req.body.id || "defaultId",
    BaseText: req.body.BaseText,
    Translation: req.body.Translation,
    LanguageFrom: req.body.LanguageFrom,
    LanguageTo: req.body.LanguageTo,
    TranslationStyle: req.body.TranslationStyle,
    Contexts: [
      {
        Id: 1,
        Question:
          "вопрос про контекст ты - кошачий ветеринарный диетолог. Твоя задача проанализировать данный состав корма и выписать от 2 до 5 минусов и плюсов, а также выдели его категорию: холистик (это беззерновой корм), премиум, супер премиум, эконом. Пиши кратко. Выделив что-то в плюс или минус, объясни, для чего конкретно это вредно или полезно. Ты не используешь размытые и общие формулировки.",
        Response: null,
      },
    ],
    State: "WaitingForContext",
    Context: req.body.Context,
  };

  console.log("Полученные данные:", req.body);
  res.send(waitingContextEntityTestData); // отправьте нужный объект в зависимости от логики вашего приложения
});

app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});
