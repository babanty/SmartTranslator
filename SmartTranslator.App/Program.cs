using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.DataAccess;
using SmartTranslator.Infrastructure.Extensions;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.Infrastructure.UserLanguages;
using SmartTranslator.TelegramBot.Management.GptTelegramBots;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TelegramBot.View;
using SmartTranslator.TelegramBot.View.Filters;
using SmartTranslator.TranslationCore;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.DI;

var builder = WebApplication.CreateBuilder(args);
var assemblies = AppDomain.CurrentDomain.GetAssemblies();

var translationCoreOptions = builder.Services.AddConfig<TranslationCoreOptions>(builder.Configuration, "TranslationCoreOptions");
builder.Services.AddConfig<GptTelegramBotOptions>(builder.Configuration, "GptTelegramBotOptions");

builder.Services.AddAutoMapper((config) => { config.AllowNullCollections = true; }, assemblies);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IGptTelegramBotBuilder, GptTelegramBotBuilder>();
builder.Services.AddScoped<IGptTranslator, GptTranslator>();
builder.Services.AddScoped<ILanguageManager, LanguageManager>();
builder.Services.AddScoped<ITextMistakeManager, TextMistakeManager>();
builder.Services.AddScoped<ITelegramBotMessageSender, TelegramBotMessageSender>();
builder.Services.AddScoped<ITelegramBotClientProvider, TelegramBotClientProvider>();
builder.Services.AddScoped<ILoadingAnimation, LoadingAnimation>();
builder.Services.AddScoped<ITranslationManager, TranslationManager>();
builder.Services.AddScoped<ITemplateStringServiceWithUserLanguage, TemplateStringServiceWithUserLanguageService>();
builder.Services.AddScoped<CoupleLanguageTranslatorController>();
builder.Services.AddScoped<TelegramBotRoutingResolver>();
builder.Services.AddScoped<TelegramIncomingMessageHandler>();
builder.Services.AddScoped<TelegramViewProvider>();
builder.Services.AddScoped<TranslationViewProvider>();
builder.Services.AddTemplateStringService();
builder.Services.AddUserLanguageProvider();

builder.Services.AddTranslationCore(translationCoreOptions);
builder.Services.AddTelegramTranslatorBotView(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<EntityNotFoundExceptionFilter>();

// databases
builder.Services.AddDbContext<StatisticsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("StatisticsDbConnection")
                        ?? throw new ArgumentNullException("Database config not found")));
builder.Services.AddDbContext<TelegramTranslationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TelegramTranslationDbConnection")
                        ?? throw new ArgumentNullException("Database config not found")));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
