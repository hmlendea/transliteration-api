using System.Text.RegularExpressions;

using NuciExtensions;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class GujaratiTransliterator(ILogger logger) : Transliterator(logger), ITransliterator
    {
        protected override string PerformTransliteration(string text, Language language)
        {
            string[] gujaratiConsonants = ["ક", "ખ", "ગ", "ઘ", "ચ", "છ", "જ", "ઝ", "ટ", "ઠ", "ડ", "ઢ", "ણ", "ત", "થ", "દ", "ધ", "ન", "પ", "ફ", "બ", "ભ", "મ", "ય", "ર", "લ", "વ", "શ", "ષ", "સ", "હ", "ળ", "ઞ"];
            string[] encodedConsonants = ["ka", "kha", "ga", "gha", "cha", "chha", "ja", "za", "ṭa", "ṭha", "ḍa", "ḍha", "ṇa", "ta", "tha", "da", "dha", "na", "pa", "fa", "ba", "bha", "ma", "ya", "ra", "la", "va", "sha", "ṣha", "sa", "ha", "ḷa", "nya"];
            string[] gujaratiHalantConsonants = ["ક્", "ખ્", "ગ્", "ઘ્", "ચ્", "છ્", "જ્", "ઝ્", "ટ્", "ઠ્", "ડ્", "ઢ્", "ણ્", "ત્", "થ્", "દ્", "ધ્", "ન્", "પ્", "ફ્", "બ્", "ભ્", "મ્", "ય્", "ર્", "લ્", "વ્", "શ્", "ષ્", "સ્", "હ્", "ળ્", "ઞ્"];
            string[] encodedHalantConsonants = ["k", "kh", "g", "gh", "ch", "chh", "j", "z", "ṭ", "ṭh", "ḍ", "ḍh", "ṇ", "t", "th", "d", "dh", "n", "p", "f", "b", "bh", "m", "y", "r", "l", "v", "sh", "ṣh", "s", "h", "ḷ", "ny"];
            string[] gujaratiVowels = ["ઓ", "ઔ", "આ", "ઇ", "ઈ", "ઉ", "ઊ", "એ", "ઐ", "ઍ", "ઑ", "ૠ", "અ"];
            string[] encodedVowels = ["o", "au", "ā", "i", "ī", "u", "ū", "e", "ai", "ĕ", "ŏ", "ṛu", "a"];
            string[] gujaratiVowelSigns = ["િ", "ી", "ુ", "ૂ", "ે", "ૈ", "ો", "ૌ", "ૅ", "ૉ", "ં", "ૃ", "્", "ઃ", "ા"];
            string[] encodedVowelSigns = ["i", "ī", "u", "ū", "e", "ai", "o", "au", "ĕ", "ŏ", "an", "ṛu", "", "ah", "ā"];
            string[] gujaratiNumerals = ["૦", "૧", "૨", "૩", "૪", "૫", "૬", "૭", "૮", "૯"];
            string[] encodedNumerals = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
            string[] encodedConsonantCombinations = ["kh", "gh", "ch", "chh", "ṭh", "ḍh", "th", "dh", "bh", "sh", "ṣh", "ny"];

            string sourcePattern;
            string replacementPattern;
            string transliteratedText = string.Empty;

            for (int consonantIndex = 0; consonantIndex < gujaratiConsonants.Length; consonantIndex++)
            {
                for (int vowelSignIndex = 0; vowelSignIndex < gujaratiVowelSigns.Length; vowelSignIndex++)
                {
                    sourcePattern = gujaratiConsonants[consonantIndex] + gujaratiVowelSigns[vowelSignIndex] + "ઃ ";
                    replacementPattern = encodedHalantConsonants[consonantIndex] + encodedVowelSigns[vowelSignIndex] + "h" + encodedVowelSigns[vowelSignIndex] + " ";
                    transliteratedText = text.Replace(sourcePattern, replacementPattern);
                    sourcePattern = gujaratiConsonants[consonantIndex] + gujaratiVowelSigns[vowelSignIndex] + "ઃ";
                    replacementPattern = encodedHalantConsonants[consonantIndex] + encodedVowelSigns[vowelSignIndex] + "h";
                    transliteratedText = transliteratedText.Replace(sourcePattern, replacementPattern);
                }
            }

            for (int consonantIndex = 0; consonantIndex < gujaratiConsonants.Length; consonantIndex++)
            {
                for (int vowelSignIndex = 0; vowelSignIndex < gujaratiVowelSigns.Length; vowelSignIndex++)
                {
                    sourcePattern = gujaratiConsonants[consonantIndex] + gujaratiVowelSigns[vowelSignIndex] + "ં";
                    replacementPattern = encodedHalantConsonants[consonantIndex] + encodedVowelSigns[vowelSignIndex] + "n";
                    transliteratedText = transliteratedText.Replace(sourcePattern, replacementPattern);
                }
            }

            for (int vowelIndex = 0; vowelIndex < gujaratiVowels.Length; vowelIndex++)
            {
                sourcePattern = gujaratiVowels[vowelIndex] + "ં";
                replacementPattern = encodedVowels[vowelIndex] + "n";
                transliteratedText = transliteratedText.Replace(sourcePattern, replacementPattern);
            }

            for (int consonantIndex = 0; consonantIndex < gujaratiConsonants.Length; consonantIndex++)
            {
                for (int vowelSignIndex = 0; vowelSignIndex < gujaratiVowelSigns.Length; vowelSignIndex++)
                {
                    sourcePattern = gujaratiConsonants[consonantIndex] + gujaratiVowelSigns[vowelSignIndex];
                    replacementPattern = encodedHalantConsonants[consonantIndex] + encodedVowelSigns[vowelSignIndex];
                    transliteratedText = transliteratedText.Replace(sourcePattern, replacementPattern);
                }
            }

            transliteratedText = ReplaceArray(gujaratiHalantConsonants, encodedHalantConsonants, transliteratedText);
            transliteratedText = ReplaceArray(gujaratiVowelSigns, encodedVowelSigns, transliteratedText);
            transliteratedText = ReplaceArray(gujaratiVowels, encodedVowels, transliteratedText);
            transliteratedText = ReplaceArray(gujaratiConsonants, encodedConsonants, transliteratedText);
            transliteratedText = ReplaceArray(gujaratiNumerals, encodedNumerals, transliteratedText);

            for (int encodedIndex = 0; encodedIndex < encodedConsonants.Length; encodedIndex++)
            {
                string pattern = encodedConsonants[encodedIndex] + " ";
                string replacement = encodedHalantConsonants[encodedIndex] + " ";
                transliteratedText = transliteratedText.Replace(pattern, replacement);
            }

            for (int halantIndex = 0; halantIndex < encodedHalantConsonants.Length; halantIndex++)
            {
                for (int encodedIndex = 0; encodedIndex < encodedConsonants.Length; encodedIndex++)
                {
                    string pattern = encodedHalantConsonants[halantIndex] + encodedHalantConsonants[encodedIndex] + " ";
                    string replacement = encodedHalantConsonants[halantIndex] + encodedConsonants[encodedIndex] + " ";
                    transliteratedText = transliteratedText.Replace(pattern, replacement);
                }
            }

            for (int vowelIndex = 0; vowelIndex < encodedVowels.Length; vowelIndex++)
            {
                for (int combinationIndex = 0; combinationIndex < encodedConsonantCombinations.Length; combinationIndex++)
                {
                    string pattern = encodedVowels[vowelIndex] + encodedConsonantCombinations[combinationIndex] + "a ";
                    string replacement = encodedVowels[vowelIndex] + encodedConsonantCombinations[combinationIndex] + " ";
                    transliteratedText = transliteratedText.Replace(pattern, replacement);
                }
            }

            return ApplyFixes(transliteratedText);
        }

        private static string ReplaceArray(string[] sourceChars, string[] replacementChars, string text)
        {
            if (sourceChars.Length != replacementChars.Length)
            {
                return null;
            }

            for (int index = 0; index < sourceChars.Length; index++)
            {
                text = text.Replace(sourceChars[index], replacementChars[index]);
            }

            return text;
        }

        private static string ApplyFixes(string text)
        {
            string fixedText = text.ToTitleCase();

            fixedText = Regex.Replace(fixedText, "yvar\\s", "ya");
            fixedText = Regex.Replace(fixedText, "y\\s", "ya ");
            fixedText = Regex.Replace(fixedText, "jnyaa", "gnaa");
            fixedText = Regex.Replace(fixedText, "jnya", "gna");
            fixedText = Regex.Replace(fixedText, "jny\\s", "gna");
            fixedText = Regex.Replace(fixedText, "svaa", "swaa");
            fixedText = Regex.Replace(fixedText, "svā", "swā");

            return fixedText;
        }
    }
}
