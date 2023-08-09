namespace SmartTranslator.Infrastructure.TemplateStrings;

/// <summary> A context representing a single main physical database where data will be written by default </summary>
internal class TemplateStringDbContext
{
    /// <summary> A collection of string templates stored in the database </summary>
    private List<TemplateString> TemplateStringCollection { get; } = new()
    {
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "Текст слишком длинный, максимальное количество символов: MaxSymbols, ваш текст: TextLength.", "", TemplateLanguage.Rus, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "The text is too long, maximum number of characters: MaxSymbols, your text: TextLength.", "", TemplateLanguage.Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "Metin çok uzun, maksimum karakter sayısı: MaxSymbols, metniniz: TextLength.", "", TemplateLanguage.Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "Der Text ist zu lang, maximale Anzahl an Zeichen: MaxSymbole, Ihre Textlänge: TextLänge.", "", TemplateLanguage.Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "El texto es demasiado largo, número máximo de caracteres: MaxSímbolos, longitud de su texto: LongitudTexto.", "", TemplateLanguage.Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "Le texte est trop long, nombre maximal de caractères: MaxSymboles, longueur de votre texte: LongueurTexte.", "", TemplateLanguage.Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "Το κείμενο είναι πολύ μεγάλο, μέγιστος αριθμός χαρακτήρων: MaxΣύμβολα, το μήκος του κειμένου σας: ΜήκοςΚειμένου.", "", TemplateLanguage.Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "TextIsTooLongException", "Matn juda uzun, belgilarning maksimal soni: MaxBelgilar, matningizning uzunligi: MatnUzunligi.", "", TemplateLanguage.Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Неизвестная ошибка, мы постараемся исправить ее как можно скорее.", "", TemplateLanguage.Rus, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Unknown error, we will try to fix it as soon as possible.", "", TemplateLanguage.Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Bilinmeyen bir hata oluştu, en kısa sürede düzeltmeye çalışacağız.", "", TemplateLanguage.Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Unbekannter Fehler, wir werden versuchen, ihn so schnell wie möglich zu beheben.", "", TemplateLanguage.Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Error desconocido, intentaremos solucionarlo lo más pronto posible.", "", TemplateLanguage.Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Erreur inconnue, nous essaierons de la corriger dès que possible.", "", TemplateLanguage.Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Άγνωστο σφάλμα, θα προσπαθήσουμε να το διορθώσουμε το συντομότερο δυνατό.", "", TemplateLanguage.Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownErrorWeWillTryToFixItAsSoonAsPossible", "Noma'lum xato, biz uni imkoni boricha tezroq tuzatishga harakat qilamiz.", "", TemplateLanguage.Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Простите, получен неизвестный тип сообщения.", "", TemplateLanguage.Rus, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Sorry, unknown message type received.", "", TemplateLanguage.Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Üzgünüm, bilinmeyen mesaj türü alındı.", "", TemplateLanguage.Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Entschuldigung, unbekannter Nachrichtentyp empfangen.", "", TemplateLanguage.Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Lo siento, se recibió un tipo de mensaje desconocido.", "", TemplateLanguage.Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Désolé, type de message inconnu reçu.", "", TemplateLanguage.Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Λυπάμαι, έλαβα άγνωστο τύπο μηνύματος.", "", TemplateLanguage.Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "UnknownMessageTypeException", "Uzr, noma'lum xabar turi qabul qilindi.", "", TemplateLanguage.Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Простите, голосовые сообщения пока не поддерживаются.", "", TemplateLanguage.Rus, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Sorry, voice messages are not supported yet.", "", TemplateLanguage.Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Üzgünüz, sesli mesajlar henüz desteklenmiyor.", "", TemplateLanguage.Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Entschuldigung, Sprachnachrichten werden noch nicht unterstützt.", "", TemplateLanguage.Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Lo siento, los mensajes de voz aún no están soportados.", "", TemplateLanguage.Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Désolé, les messages vocaux ne sont pas encore pris en charge.", "", TemplateLanguage.Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Λυπάμαι, τα φωνητικά μηνύματα δεν υποστηρίζονται ακόμα.", "", TemplateLanguage.Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "VoiceMessageTypeNotImplementedException", "Kechirasiz, ovozli xabarlar hali qo'llab-quvvatlanmaydi.", "", TemplateLanguage.Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "RateLimitException", "Простите, переводчик сейчас пользуется таким спросом, что нам заблокировали ресурсы. Через минуту все придет в норму, приносим извинения!", "", TemplateLanguage.Rus, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Sorry, the translator is in such high demand right now that we've been blocked from resources. In a minute everything will be back to normal, we apologize!", "", TemplateLanguage.Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Üzgünüz, çevirmen şu anda o kadar talep görüyor ki, kaynaklarımız engellendi. Bir dakika içinde her şey normale dönecek, özür dileriz!", "", TemplateLanguage.Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Entschuldigung, der Übersetzer ist gerade so stark nachgefragt, dass wir von Ressourcen blockiert wurden. In einer Minute wird alles wieder normal sein, wir bitten um Entschuldigung!", "", TemplateLanguage.Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Lo sentimos, el traductor está en tal alta demanda en este momento que hemos sido bloqueados de los recursos. En un minuto todo volverá a la normalidad, ¡nos disculpamos!", "", TemplateLanguage.Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Désolé, le traducteur est tellement demandé en ce moment que nous avons été bloqués des ressources. Dans une minute, tout reviendra à la normale, nous nous excusons!", "", TemplateLanguage.Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Λυπούμαστε, ο μεταφραστής είναι τόσο ζητημένος αυτή τη στιγμή που μας έχουν αποκλείσει από τους πόρους. Σε ένα λεπτό όλα θα επιστρέψουν στην κανονικότητα, ζητάμε συγνώμη!", "", TemplateLanguage.Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "RateLimitException", "Kechirasiz, tarjimon hozirgi vaqtda shunchalik talab qilinmoqda ki, biz resurslardan bloklanib qoldik. Bir daqiqa ichida hamma narsa normaga qaytariladi, biz uzr so'raymiz!", "", TemplateLanguage.Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Упс.. Несмотря на то что у нас повышенный приоритет на взаимодействие с chatGPT, он все равно не смог выполнить запрос из-за того что перегружен. Приносим свои извинения! Попробуйте через несколько минут, пожалуйста.", "", TemplateLanguage.Rus, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Oops.. Despite our high priority on interaction with chatGPT, it still couldn't fulfill the request due to being overloaded. We apologize! Please try again in a few minutes.", "", TemplateLanguage.Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Hata... ChatGPT ile etkileşim önceliğimize rağmen, yoğunluktan dolayı isteği yerine getiremedi. Özür dileriz! Lütfen birkaç dakika sonra tekrar deneyin.", "", TemplateLanguage.Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Hoppla... Trotz unserer hohen Priorität für die Interaktion mit chatGPT, konnte die Anfrage aufgrund von Überlastung nicht erfüllt werden. Wir entschuldigen uns! Bitte versuchen Sie es in ein paar Minuten erneut.", "", TemplateLanguage.Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Vaya... A pesar de nuestra alta prioridad en la interacción con chatGPT, no pudo cumplir con la solicitud debido a estar sobrecargado. ¡Pedimos disculpas! Por favor, inténtelo de nuevo en unos minutos.", "", TemplateLanguage.Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Oups... Malgré notre priorité élevée pour l'interaction avec chatGPT, il n'a pas pu répondre à la demande en raison de la surcharge. Nous nous excusons! Veuillez réessayer dans quelques minutes.", "", TemplateLanguage.Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Ουπς ... Παρά την υψηλή προτεραιότητά μας στην επαφή με το chatGPT, δεν κατάφερε να εκπληρώσει το αίτημα λόγω υπερφόρτωσης. Ζητούμε συγγνώμη! Δοκιμάστε ξανά σε λίγα λεπτά.", "", TemplateLanguage.Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "ModelOverloadedException", "Kechirasiz... ChatGPT bilan aloqaga o'tishga yuqori darajada e'tibor qaratganmizga qaramasdan, uni yuklash tufayli so'rovni bajarib bo'lmadi. Kechirasiz! Iltimos, bir necha daqiqa ichida yana bir bor urinib ko'ring.", "", TemplateLanguage.Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} The artificial intelligence failed to correctly translate your message. We will check what went wrong and fix it in the next update. We apologize for the inconvenience!", "", TemplateLanguage.Rus_Eng, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} Yapay zeka mesajınızı doğru bir şekilde çeviremedi. Ne olduğunu kontrol edeceğiz ve bir sonraki güncellemede bunu düzelteceğiz. Özür dileriz!", "", TemplateLanguage.Rus_Tur, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} Die künstliche Intelligenz hat Ihre Nachricht nicht korrekt übersetzt. Wir werden prüfen, was schief gelaufen ist und es im nächsten Update beheben. Wir entschuldigen uns für die Unannehmlichkeiten!", "", TemplateLanguage.Rus_Deu, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} La inteligencia artificial falló al traducir correctamente tu mensaje. Verificaremos qué salió mal y lo solucionaremos en la próxima actualización. ¡Lamentamos las molestias!", "", TemplateLanguage.Rus_Esp, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} L'intelligence artificielle a échoué à traduire correctement votre message. Nous vérifierons ce qui s'est mal passé et le corrigerons lors de la prochaine mise à jour. Nous nous excusons pour le désagrément!", "", TemplateLanguage.Rus_Fra, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} Η τεχνητή νοημοσύνη απέτυχε να μεταφράσει σωστά το μήνυμά σας. Θα ελέγξουμε τι πήγε στραβά και θα το διορθώσουμε στην επόμενη ενημέρωση. Ζητούμε συγγνώμη για την ταλαιπωρία!", "", TemplateLanguage.Rus_Gre, EnvironmentType.Any),
        new TemplateString(Guid.NewGuid(), "FailedToTranslateException", $"Искусственному интеллекту не удалось корректно перевести Ваше сообщение. Мы проверим что пошло не так и поправим это при следующем обновлении. Приносим свои извинения! {Environment.NewLine} Sun'iy intellekt sizning xabaringizni to'g'ri tarjima qila olmadi. Nima noto'g'ri bo'lganini tekshirib, uni keyingi yangilanishda to'g'rilaymiz. Noqulaylik uchun uzr so'raymiz!", "", TemplateLanguage.Rus_Uzb, EnvironmentType.Any),

        new TemplateString(Guid.NewGuid(), "GreetingLetter", @"Привет!
Это еще не все функции переводчика! Еще он может понять транслит, например если написать ему ""аполоджайс"".
Кроме того существуют и другие языковые пары.
Hey there! 
That's not all the translator's functions yet! It can also understand transliteration, for example, if you write ""Privet"" to it. 
Moreover, there are other language pairs as well.
🇷🇺 - 🇹🇷 @gtp_translator_rus_tur_bot
🇷🇺 - 🇪🇸 @gtp_translator_rus_esp_bot
🇷🇺 - 🇩🇪 @gtp_translator_rus_deu_bot
🇷🇺 - 🇫🇷 @gtp_translator_rus_fra_bot
🇷🇺 - 🇬🇷 @gtp_translator_rus_gre_bot
🇷🇺 - 🇺🇿 @gpt_translator_rus_uzb_bot", "", TemplateLanguage.Rus_Eng, EnvironmentType.Any),
    };

    private readonly object _templateStringCollectionLock = new();

    public IQueryable<TemplateString> GetQuery()
    {
        return TemplateStringCollection.AsQueryable();
    }

    public Task<IReadOnlyCollection<TemplateString>> ExecuteQuery(IQueryable<TemplateString> query)
    {
        lock (_templateStringCollectionLock)
            return Task.FromResult((IReadOnlyCollection<TemplateString>)query.ToList());
    }
}
