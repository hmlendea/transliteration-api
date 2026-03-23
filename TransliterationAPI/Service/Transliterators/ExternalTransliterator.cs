using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuciLog.Core;
using TransliterationAPI.Logging;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public abstract class ExternalTransliterator(
        ILogger logger)
        : IExternalTransliterator
    {
        public async Task<string> Transliterate(string text, Language language)
        {
            IEnumerable<LogInfo> logInfos =
            [
                new LogInfo(MyLogInfoKey.Text, text),
                new LogInfo(MyLogInfoKey.LanguageCode, language.Code),
                new LogInfo(MyLogInfoKey.LanguageName, language.Name),
                new LogInfo(MyLogInfoKey.Transliterator, GetType().Name)
            ];

            logger.Info(
                MyOperation.TransliteratorExecution,
                OperationStatus.Started,
                logInfos);

            try
            {
                string transliteratedText = await PerformTransliteration(text, language);

                logger.Info(
                    MyOperation.TransliteratorExecution,
                    OperationStatus.Success,
                    logInfos,
                    new LogInfo(MyLogInfoKey.TransliteratedText, transliteratedText));

                return transliteratedText;
            }
            catch (Exception ex)
            {
                logger.Error(
                    MyOperation.TransliteratorExecution,
                    OperationStatus.Failure,
                    ex,
                    logInfos);

                throw;
            }
        }

        protected abstract Task<string> PerformTransliteration(string text, Language language);
    }
}
