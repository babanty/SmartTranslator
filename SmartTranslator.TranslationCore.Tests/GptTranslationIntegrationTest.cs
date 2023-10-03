using SmartTranslator.TranslationCore.Enums;
using Xunit;

namespace SmartTranslator.TranslationCore.Tests;

public class GptTranslationIntegrationTest
{
    private readonly IntegrationTestOptions _testOptions;

    public GptTranslationIntegrationTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
    }


    [Fact]
    public async Task Translate_ValidInputWithoutContext_TranslatesCorrectly()
    {
        // Arrange
        var translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };
        var httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };
        var httpClient = new GptHttpClient(httpClientOptions);
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "Hello world!";
        var context = "";
        var from = Language.English;
        var to = Language.Russian;
        var translationStyle = TranslationStyle.ConversationalStyle;

        // Act
        var result = await translator.Translate(text, context, from, to, translationStyle);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("\"Привет, мир!\"", result);
    }

    
    [Fact]
    public async Task EvaluateContext_UnequivocalInput_AnswerOK()
    {
        // Arrange
        var translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };
        var httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };
        var httpClient = new GptHttpClient(httpClientOptions);
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "Hello world!";
        var to = Language.Russian;
        var contexts = "";

        // Act
        var result = await translator.EvaluateContext(text, to, contexts);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1f, result.Percent);
    }


    [Fact]
    public async Task EvaluateContext_EquivocalInput_AnswerOK()
    {
        // Arrange
        var translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };
        var httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };
        var httpClient = new GptHttpClient(httpClientOptions);
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "She was struck by the book.";
        var to = Language.Russian;
        var contexts = "";

        // Act
        var result = await translator.EvaluateContext(text, to, contexts);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0f, result.Percent);
        Assert.NotNull(result.Request);
    }
    

    [Fact]
    public async Task DefineStyle_ValidInput_AnswerOK()
    {
        // Arrange
        var translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };
        var httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };
        var httpClient = new GptHttpClient(httpClientOptions);
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "She was struck by the book.";
        var context = "";
        var from = Language.English;
        var to = Language.Russian;


        //Act
        var result = await translator.DefineStyle(text, context, from, to);


        // Assert
        float GetProbability(TranslationStyle style) =>
            result.ProbabilityOfSuccess.Where(prob => prob.Style == style).First().Probability;

        var officialProb = GetProbability(TranslationStyle.OfficialStyle);
        var conversationalProb = GetProbability(TranslationStyle.ConversationalStyle);
        var teenageProb = GetProbability(TranslationStyle.TeenageStyle);

        Assert.NotNull(result);

        Assert.InRange(officialProb, 0.6f, 0.8f); // TODO: change to more appropriate values once GPT-4 is accessible
        Assert.InRange(conversationalProb, 0.5f, 0.7f); // TODO: change to more appropriate values once GPT-4 is accessible
        Assert.InRange(teenageProb, 0.2f, 0.4f); // TODO: change to more appropriate values once GPT-4 is accessible
    }

    /*
     * Don't run unless really needed - one test costs a lot
    [Fact] 
    public async Task Translate_DdosAttack_AnswerOK()
    {
        // Arrange
        var translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };
        var httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };
        var httpClient = new GptHttpClient(httpClientOptions);
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "\r\nЧто такое Lorem Ipsum?\r\nLorem Ipsum - это текст-\"рыба\", часто используемый в печати и вэб-дизайне. Lorem Ipsum является стандартной \"рыбой\" для текстов на латинице с начала XVI века. В то время некий безымянный печатник создал большую коллекцию размеров и форм шрифтов, используя Lorem Ipsum для распечатки образцов. Lorem Ipsum не только успешно пережил без заметных изменений пять веков, но и перешагнул в электронный дизайн. Его популяризации в новое время послужили публикация листов Letraset с образцами Lorem Ipsum в 60-х годах и, в более недавнее время, программы электронной вёрстки типа Aldus PageMaker, в шаблонах которых используется Lorem Ipsum.\r\n\r\nПочему он используется?\r\nДавно выяснено, что при оценке дизайна и композиции читаемый текст мешает сосредоточиться. Lorem Ipsum используют потому, что тот обеспечивает более или менее стандартное заполнение шаблона, а также реальное распределение букв и пробелов в абзацах, которое не получается при простой дубликации \"Здесь ваш текст.. Здесь ваш текст.. Здесь ваш текст..\" Многие программы электронной вёрстки и редакторы HTML используют Lorem Ipsum в качестве текста по умолчанию, так что поиск по ключевым словам \"lorem ipsum\" сразу показывает, как много веб-страниц всё ещё дожидаются своего настоящего рождения. За прошедшие годы текст Lorem Ipsum получил много версий. Некоторые версии появились по ошибке, некоторые - намеренно (например, юмористические варианты).\r\n\r\n";
        var context = "";
        var from = Language.English;
        var to = Language.Russian;
        var translationStyle = TranslationStyle.ConversationalStyle;

        // Act
        var tasks = new List<Task>();
        int successfulCount = 0;

        for (int i = 0; i < 2; i++)
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    await translator.Translate(text, context, from, to, translationStyle);
                    Interlocked.Increment(ref successfulCount);
                }
                catch
                {
                    // Обработка ошибки, если это необходимо
                    // Иначе просто продолжаем
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"Успешных переводов: {successfulCount}");

        // Assert
        Assert.Equal(1, 1);
    }*/
}
