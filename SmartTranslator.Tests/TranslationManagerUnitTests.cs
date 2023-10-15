using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;
using Xunit;

namespace SmartTranslator.Tests
{
    public class TranslationManagerUnitTests : IDisposable
    {
        private readonly TelegramTranslationDbContext _dbContext;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IGptTranslator> _translatorMock;
        private readonly Mock<ILanguageManager> _languageManagerMock;
        private readonly Mock<ITextMistakeManager> _textMistakeManagerMock;
        private readonly Mock<IPublisher> _publisherMock;
        private readonly RateLimitOptions _rateLimitOptions;

        public TranslationManagerUnitTests()
        {
            var options = new DbContextOptionsBuilder<TelegramTranslationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Each test has it's unique db name
                .Options;

            _dbContext = new TelegramTranslationDbContext(options);
            _mapperMock = new Mock<IMapper>();
            _translatorMock = new Mock<IGptTranslator>();
            _languageManagerMock = new Mock<ILanguageManager>();
            _textMistakeManagerMock = new Mock<ITextMistakeManager>();
            _publisherMock = new Mock<IPublisher>();
            _rateLimitOptions = new RateLimitOptions
            {
                RateLimits = new RateLimit[]
                {
                    new RateLimit
                    {
                        AllowedTranslations = 3,
                        TimeSpanInSeconds = 60
                    },
                    new RateLimit
                    {
                        AllowedTranslations = 30,
                        TimeSpanInSeconds = 86400
                    }
                }
            };
        }


        [Fact]
        public async Task ExecuteEntityProcessingPipeline_LanguageNotDetermined_WaitingForLanguageState()
        {
            // Arrange
            _languageManagerMock.Setup(l => l.DetermineLanguage(It.IsAny<string>())).Returns(Task.FromResult((Language?)null));

            var expectedLanguage = (Language?)null;
            var expectedState = TelegramTranslationState.WaitingForLanguage;

            var manager = CreateManager();
            var entity = new TelegramTranslationEntity() { BaseText = "Some text" };

            // Act
            var result = await manager.ExecuteEntityProcessingPipeline(entity);

            // Assert
            Assert.Equal(expectedLanguage, result.LanguageFrom);
            Assert.Equal(expectedState, result.State);
        }


        [Fact]
        public async Task ExecuteEntityProcessingPipeline_LanguageDetermined_CorrectStateAndLanguage()
        {
            var determinedBaseLanguage = Language.English;
            var question = "What is the meaning of life?";
            var evaluationResult = new EvaluationResponse
            {
                Percent = 0.5f,
                Request = new ClarificationRequest
                {
                    ClarifyingQuestion = question,
                }
            };

            _languageManagerMock.Setup(l => l.DetermineLanguage(It.IsAny<string>())).Returns(Task.FromResult((Language?)determinedBaseLanguage));
            _languageManagerMock.Setup(l => l.GetLanguagePair()).Returns((Language.Russian, Language.English));
            _translatorMock.Setup(l => l.EvaluateContext(It.IsAny<string>(), It.IsAny<Language>(), It.IsAny<string>())).Returns(Task.FromResult(evaluationResult));

            var expectedTargetLanguage = Language.Russian;
            var expectedState = TelegramTranslationState.WaitingForContext;

            var manager = CreateManager();
            var entity = new TelegramTranslationEntity() { BaseText = "Some text" };

            // Act
            var result = await manager.ExecuteEntityProcessingPipeline(entity);

            // Assert
            Assert.Equal(question, result.Contexts.Single().Question);
            Assert.Equal(determinedBaseLanguage, result.LanguageFrom);
            Assert.Equal(expectedTargetLanguage, result.LanguageTo);
            Assert.Equal(expectedState, result.State);
        }


        [Fact]
        public async Task ExecuteEntityProcessingPipeline_StyleDetermined_AddsStyleToEntity()
        {
            // Arrange
            var expectedStyle = TranslationStyle.OfficialStyle;
            var styleEvaluationResult = new StyleDefinitionResult
            {
                ProbabilityOfSuccess = new List<StyleProbability>
                {
                    new StyleProbability { Style = expectedStyle, Probability = 0.9f },
                    new StyleProbability { Style = TranslationStyle.ConversationalStyle, Probability = 0.6f }
                }
            };
            var determinedBaseLanguage = Language.English;
            var evaluationResult = new EvaluationResponse
            {
                Percent = 1
            };

            _languageManagerMock.Setup(l => l.DetermineLanguage(It.IsAny<string>())).Returns(Task.FromResult((Language?)determinedBaseLanguage));
            _languageManagerMock.Setup(l => l.GetLanguagePair()).Returns((Language.Russian, Language.English));
            _translatorMock.Setup(t => t.DefineStyle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Language>(), It.IsAny<Language>())).Returns(Task.FromResult(styleEvaluationResult));
            _translatorMock.Setup(l => l.EvaluateContext(It.IsAny<string>(), It.IsAny<Language>(), It.IsAny<string>())).Returns(Task.FromResult(evaluationResult));

            var manager = CreateManager();
            var entity = new TelegramTranslationEntity
            {
                BaseText = "Some text"
            };

            // Act
            var result = await manager.ExecuteEntityProcessingPipeline(entity);

            // Assert
            Assert.Equal(expectedStyle, result.TranslationStyle);
        }


        [Fact]
        public async Task ExecuteEntityProcessingPipeline_ValidTranslation_ProcessesTranslation()
        {
            // Arrange
            var expectedTranslation = "Переведенный текст";
            var contextString = "Q1 - A1";
            var style = TranslationStyle.OfficialStyle;
            var expectedStyle = TranslationStyle.OfficialStyle;
            var styleEvaluationResult = new StyleDefinitionResult
            {
                ProbabilityOfSuccess = new List<StyleProbability>
                {
                    new StyleProbability { Style = expectedStyle, Probability = 0.9f },
                    new StyleProbability { Style = TranslationStyle.ConversationalStyle, Probability = 0.6f }
                }
            };
            var determinedBaseLanguage = Language.English;
            var evaluationResult = new EvaluationResponse
            {
                Percent = 1
            };

            // Old ones
            _languageManagerMock.Setup(l => l.DetermineLanguage(It.IsAny<string>())).Returns(Task.FromResult((Language?)determinedBaseLanguage));
            _languageManagerMock.Setup(l => l.GetLanguagePair()).Returns((Language.Russian, Language.English));
            _translatorMock.Setup(t => t.DefineStyle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Language>(), It.IsAny<Language>())).Returns(Task.FromResult(styleEvaluationResult));
            _translatorMock.Setup(l => l.EvaluateContext(It.IsAny<string>(), It.IsAny<Language>(), It.IsAny<string>())).Returns(Task.FromResult(evaluationResult));
            // New ones
            _translatorMock.Setup(t => t.Translate(It.IsAny<string>(), contextString, It.IsAny<Language>(), It.IsAny<Language>(), style)).Returns(Task.FromResult(expectedTranslation));
            _textMistakeManagerMock.Setup(t => t.Correct(It.IsAny<string>())).Returns(Task.FromResult(expectedTranslation)); // Assuming the corrected text is same as the initial translation

            var manager = CreateManager();
            var entity = new TelegramTranslationEntity
            {
                BaseText = "Some text"
            };

            // Act
            var result = await manager.ExecuteEntityProcessingPipeline(entity);

            // Assert
            Assert.Equal(expectedTranslation, result.Translation);
        }


        [Fact]
        public async Task FinishTranslation_EntityExists_UpdatesStateAndUpdatedAt()
        {
            // Arrange
            var translationId = "testId";
            var translation = new TelegramTranslationEntity
            {
                Id = translationId,
                BaseText = "Some text",
                UserName = "Test",
                State = TelegramTranslationState.WaitingForContext,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
            _dbContext.TelegramTranslations.Add(translation);
            await _dbContext.SaveChangesAsync();

            var manager = CreateManager();

            // Act
            await manager.FinishTranslation(translationId);

            // Assert
            var updatedEntity = _dbContext.TelegramTranslations.Find(translationId);
            Assert.NotNull(updatedEntity);
            Assert.Equal(TelegramTranslationState.Finished, updatedEntity.State);
            Assert.True(updatedEntity.UpdatedAt > DateTime.UtcNow.AddMinutes(-1)); // Ensure the UpdatedAt is recent within the last minute.
        }

        [Fact]
        public async Task FinishTranslation_EntityDoesNotExist_DoesNotUpdate()
        {
            // Arrange
            var translationId = "nonExistingId";

            var manager = CreateManager();

            // Act
            await manager.FinishTranslation(translationId);

            // Assert
            var nonExistingEntity = _dbContext.TelegramTranslations.Find(translationId);
            Assert.Null(nonExistingEntity);
        }


        private TranslationManager CreateManager()
        {
            return new TranslationManager(_dbContext, _mapperMock.Object, _translatorMock.Object, _languageManagerMock.Object, _textMistakeManagerMock.Object, _publisherMock.Object, _rateLimitOptions);
        }


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
