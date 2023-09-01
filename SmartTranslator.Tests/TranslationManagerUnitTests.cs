using Xunit;
using Moq;
using SmartTranslator.DataAccess;
using SmartTranslator.TranslationCore.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Abstractions;
using Microsoft.EntityFrameworkCore.InMemory;
using AutoMapper;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.TranslationCore.Enums;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;

namespace SmartTranslator.Tests
{
    public class TranslationManagerUnitTests : IDisposable
    {
        private readonly TelegramTranslationDbContext _dbContext;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IGptTranslator> _translatorMock;
        private readonly Mock<ILanguageManager> _languageManagerMock;
        private readonly Mock<ITextMistakeManager> _textMistakeManagerMock;

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
            _translatorMock.Setup(l => l.EvaluateContext(It.IsAny<string>(), It.IsAny<Language>())).Returns(Task.FromResult(evaluationResult));

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
            _translatorMock.Setup(l => l.EvaluateContext(It.IsAny<string>(), It.IsAny<Language>())).Returns(Task.FromResult(evaluationResult));

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
            _translatorMock.Setup(l => l.EvaluateContext(It.IsAny<string>(), It.IsAny<Language>())).Returns(Task.FromResult(evaluationResult));
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


        private TranslationManager CreateManager()
        {
            return new TranslationManager(_dbContext, _mapperMock.Object, _translatorMock.Object, _languageManagerMock.Object, _textMistakeManagerMock.Object);
        }


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
