using MediatR;
using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.Extensions;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.Management.GptTelegramBots;
using SmartTranslator.TelegramBot.View;
using SmartTranslator.TranslationCore;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.DI;

var builder = WebApplication.CreateBuilder(args);


var translationCoreOptions = builder.Services.AddConfig<TranslationCoreOptions>(builder.Configuration, "TranslationCoreOptions");
builder.Services.AddConfig<GptTelegramBotOptions>(builder.Configuration, "GptTelegramBotOptions");

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IGptTelegramBotBuilder, GptTelegramBotBuilder>();
builder.Services.AddScoped<IGptTranslator, GptTranslator>();
builder.Services.AddScoped<ITelegramBotMessageSender, TelegramBotMessageSender>();
builder.Services.AddScoped<ITelegramBotClientProvider, TelegramBotClientProvider>();
builder.Services.AddScoped<ILoadingAnimation, LoadingAnimation>();
builder.Services.AddScoped<CoupleLanguageTranslatorController>();
builder.Services.AddScoped<TelegramBotRoutingResolver>();
builder.Services.AddScoped<TelegramIncomingMessageHandler>();
builder.Services.AddTemplateStringService();

builder.Services.AddTranslationCore(translationCoreOptions);
builder.Services.AddTelegramTranslatorBotView(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
