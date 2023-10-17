const express = require('express');
const cors = require('cors');
const app = express();
const PORT = 3000;

app.use(cors());
app.get('/api/translation', (req, res) => {
    const id = req.query.id || "defaultId"; // Если id не предоставлен, используется "defaultId"

    const entity = {
        Id: id,
        BaseText: 'Sample base text',
        Translation: 'Переведено',
        LanguageFrom: 'English',
        LanguageTo: 'Russian',
        TranslationStyle:  'ConversationalStyle',
        Contexts: [{
            Id: 1,
            Question: 'Sample question?',
            Response: 'Sample response'
        }],
        State: 'finished'
    };

    res.send(entity);
});

app.use(express.json());
app.use(cors());

app.post('/api/translation', (req, res) => {
    console.log("Полученные данные:", req.body);
        const entity = {
            Id: req.body.id || defaultId,
            BaseText: req.body.BaseText,
            Translation: req.body.Translation,
            LanguageFrom: req.body.LanguageFrom,
            LanguageTo: req.body.LanguageTo,
            TranslationStyle:  'ConversationalStyle',
            Contexts: [{
                Id: 1,
                Question: null,
                Response: 'Sample response'
            }],
            State: 'finished',
            Context:req.body.Context
        };
    
        res.send(entity);
});

app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});

app.use((req, res, next) => {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    next();
});
