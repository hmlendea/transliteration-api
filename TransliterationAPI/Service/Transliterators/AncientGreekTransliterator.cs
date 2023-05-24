using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TransliterationAPI.Service.Transliterators
{
    public class AncientGreekTransliterator : IAncientGreekTransliterator
    {
        Dictionary<char, string> transliterationMap;
        Dictionary<char, string> transliterationMap2;

        public AncientGreekTransliterator()
        {
            transliterationMap = new Dictionary<char, string>
            {
                { 'α', "a" },
                { 'ἀ', "a" },
                { 'β', "v" }, //b
                { 'γ', "g" },
                { 'δ', "d" },
                { 'ε', "e" },
                { 'ζ', "z" },
                { 'η', "ē" },
                { 'θ', "th" },
                { 'ι', "i" },
                { 'κ', "k" },
                { 'λ', "l" },
                { 'μ', "m" },
                { 'ν', "n" },
                { 'ξ', "x" },
                { 'ο', "o" },
                { 'π', "p" },
                { 'ρ', "r" },
                { 'ς', "s" },
                { 'σ', "s" },
                { 'τ', "t" },
                { 'υ', "u" },
                { 'φ', "ph" },
                { 'χ', "ch" },
                { 'ψ', "ps" },
                { 'ω', "ō" },

                // Capital letters
                { 'Α', "A" },
                { 'Β', "B" },
                { 'Γ', "G" },
                { 'Δ', "D" },
                { 'Ε', "E" },
                { 'Ζ', "Z" },
                { 'Η', "Ē" },
                { 'Θ', "Th" },
                { 'Ι', "I" },
                { 'Κ', "K" },
                { 'Λ', "L" },
                { 'Μ', "M" },
                { 'Ν', "N" },
                { 'Ξ', "X" },
                { 'Ο', "O" },
                { 'Π', "P" },
                { 'Ρ', "R" },
                { 'Σ', "S" },
                { 'Τ', "T" },
                { 'Υ', "U" },
                { 'Φ', "Ph" },
                { 'Χ', "Ch" },
                { 'Ψ', "Ps" },
                { 'Ω', "Ō" },

                // Common diacritical marks
                { 'ά', "á" },
                { 'έ', "é" },
                { 'ῆ', "ê" },
                { 'ή', "ḗ" }, // ḗ
                { 'ί', "í" },
                { 'ϊ', "ï" },
                { 'ΐ', "ḯ" },
                { 'ό', "ó" },
                { 'ὐ', "ú" },
                { 'ύ', "ú" },
                { 'ϋ', "ü" },
                { 'ΰ', "ǘ" },
                { 'ώ', "ṓ" }, // ṓ

                // Capital letters with diacritical marks
                { 'Ἀ', "A" },
                { 'Ά', "Á" },
                { 'Ἐ', "E" },
                { 'Έ', "É" },
                { 'Ή', "Ḗ" }, // Ḗ
                { 'Ἡ', "Hē" },
                { 'Ἰ', "I" },
                { 'Ί', "Í" },
                { 'Ὀ', "Ó" },
                { 'Ὄ', "Ó" },
                { 'Ό', "Ó" },
                { 'Ῥ', "Rh" },
                { 'Ύ', "Ú" },
                { 'Ώ', "Ṓ" }, // Ṓ


                // Additional characters
                { 'ϵ', "e" }, // lunate epsilon
                { 'ϑ', "th" }, // script theta
                { 'ϱ', "r" }, // script rho
                { 'ϖ', "p" }, // lunate pi
                { 'ϰ', "k" }, // script kappa
                { 'ϗ', "kai" }, // kai symbol
            };

            transliterationMap2 = new Dictionary<char, string>
            {
                {'ἀ', "a"},
                {'ἄ', "á"},
                {'ἁ', "a"},
                {'ἅ', "á"},
                {'ά', "á"},
                {'ᾴ', "á̄"},
                {'ὰ', "à"},
                {'ᾲ', "ā̀"},
                {'ᾰ', "a"},
                {'ᾱ', "ā"},
                {'ᾳ', "ā"},
                {'ᾄ', "ái"},
                {'ᾀ', "āi"},
                {'ᾅ', "ái"},
                {'ᾁ', "āi"},
                {'ἂ', "â"},
                {'ἆ', "â"},
                {'ἃ', "â"},
                {'ἇ', "â"},
                {'ᾶ', "â"},
                {'ᾷ', "â̄"},
                {'ᾂ', "âi"},
                {'ᾆ', "âi"},
                {'ᾃ', "âi"},
                {'ᾇ', "âi"},
                {'β', "b"},
                {'γ', "g"},
                {'δ', "d"},
                {'ε', "e"},
                {'ἐ', "e"},
                {'ἔ', "é"},
                {'ἒ', "ê"},
                {'ἑ', "e"},
                {'ἕ', "é"},
                {'ἓ', "ê"},
                {'έ', "é"},
                {'ὲ', "è"},
                {'ζ', "z"},
                {'η', "ē"},
                {'ἠ', "ē"},
                {'ἤ', "ḗ"}, // ḗ
                {'ἢ', "ê"},
                {'ἦ', "ê"},
                {'ἡ', "ē"},
                {'ἥ', "ḗ"}, // ḗ
                {'ἣ', "ê"},
                {'ἧ', "ê"},
                {'ή', "ḗ"}, // ḗ
                {'ῄ', "ē̄́"},
                {'ὴ', "ḕ"},
                {'ῂ', "ē̄̀"},
                {'ῆ', "ê"},
                {'ῇ', "ê̄"},
                {'ῃ', "ē̂"},
                {'ᾔ', "ēí"},
                {'ᾒ', "êi"},
                {'ᾖ', "êi"},
                {'ᾐ', "ēi"},
                {'ᾕ', "ēí"},
                {'ᾓ', "êi"},
                {'ᾗ', "êi"},
                {'ᾑ', "ēi"},
            };
        }

        public string Transliterate(string text)
        {
            string transliteratedText = string.Empty;

            foreach (char character in text)
            {
                if (transliterationMap.ContainsKey(character))
                {
                    transliteratedText += transliterationMap[character];
                }
                else
                {
                    transliteratedText += character;
                }
            }

            transliteratedText = ApplyFixes(transliteratedText);

            return transliteratedText;
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "Ach", "Akh");
            fixedText = Regex.Replace(fixedText, "Buz", "Byz");
            fixedText = Regex.Replace(fixedText, "Ku", "Ky");
            fixedText = Regex.Replace(fixedText, "Kú", "Ký");
            fixedText = Regex.Replace(fixedText, "Mu([gk])", "My$1");

            fixedText = Regex.Replace(fixedText, "([hk])ê([^n])", "$1ē$2");
            fixedText = Regex.Replace(fixedText, "che([ií])", "khe$1");
            fixedText = Regex.Replace(fixedText, "gup", "gyp");
            fixedText = Regex.Replace(fixedText, "lu([mn])", "ly$1");
            fixedText = Regex.Replace(fixedText, "óvo", "óbo");
            fixedText = Regex.Replace(fixedText, "rut", "ryt");
            fixedText = Regex.Replace(fixedText, "v([úý])", "bý");
            fixedText = Regex.Replace(fixedText, "vai", "bai");
            fixedText = Regex.Replace(fixedText, "vos", "bos");
            fixedText = Regex.Replace(fixedText, "vul", "byl");

            return fixedText;
        }
    }
}
