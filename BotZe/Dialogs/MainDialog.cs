// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.14.0

//using CoreBot.CognitiveModels;
using EmptyBotZe.CognitiveModels;
using EmptyBotZe.Recognizers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyBotZe.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly MainRecognizer _luisRecognizer;


        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(MainRecognizer luisRecognizer)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "Em que posso ajudar você?\nFale algo \"Quero abrir um chamado\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            /*if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(BookingDialog), new BookingDetails(), cancellationToken);
            }*/

            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<ZeCognitiveModel>(stepContext.Context, cancellationToken);
            var responseMessageText = $"Olá.";
            var  responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
            switch (luisResult.TopIntent().intent)
            {
                case ZeCognitiveModel.Intent.saudacao:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;
                //Identificar as inteções e fazer as respostas
                case ZeCognitiveModel.Intent.chamado:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.incidente:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.problema:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.mopp:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.ocorrencia:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.refeicao:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.tempo:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.abertura:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.prazo:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                case ZeCognitiveModel.Intent.saber:
                    responseMessageText = $"Olá.";
                    responseMessage = MessageFactory.Text(responseMessageText, responseMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                    break;

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Desculpe, não entendi o que falou, tente ({luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //// If the child dialog ("BookingDialog") was cancelled, the user failed to confirm or if the intent wasn't BookFlight
            //// the Result here will be null.
            /*if (stepContext.Result is BookingDetails result)
            {
            //    // Now we have all the booking details call the booking service.

            //    // If the call to the booking service was successful tell the user.

                var timeProperty = new TimexProperty(result.TravelDate);
                var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
                var messageText = $"Eu abri seu chamado";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }*/

            // Restart the main dialog with a different message the second time around
            var promptMessage = "Posso te ajudar em mais alguma coisa?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
