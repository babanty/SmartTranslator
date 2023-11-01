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
    LanguageFrom: "EnglishLang",
    LanguageTo: "RussianLang",
    TranslationStyle: "ConversationalStyle",
    Context: "Sample text",
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
        Question: "вопрос про контекст",
        Response: null,
      },
    ],
    State: "WaitingForContext",
    Context: req.body.Context,
  };

  console.log("Полученные данные:", req.body);
  
  setTimeout(() => {
    res.send(waitingContextEntityTestData);
  }, 5000); 
});

app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});
