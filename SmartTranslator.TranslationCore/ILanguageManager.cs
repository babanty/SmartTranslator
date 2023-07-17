﻿using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public interface ILanguageManager
{
    (Language, Language) GetLanguagePair();

    Task<Language> DetermineLanguage(string text);
}