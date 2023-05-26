using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TransliterationAPI.Service.Transliterators
{
    public class AncientGreekTransliterator : IAncientGreekTransliterator
    {
        Dictionary<string, string> transliterationTable;
        Dictionary<string, string> doricTransliterationTable;

        public AncientGreekTransliterator()
        {
            transliterationTable = new Dictionary<string, string>
            {
                { "α", "a" },
                { "ἀ", "a" },
                { "β", "b" }, // v
                { "γ", "g" },
                { "δ", "d" },
                { "ε", "e" },
                { "ζ", "z" },
                { "η", "ē" },
                { "θ", "th" },
                { "ι", "i" },
                { "κ", "k" },
                { "λ", "l" },
                { "μ", "m" },
                { "ν", "n" },
                { "ξ", "x" },
                { "ο", "o" },
                { "π", "p" },
                { "ρ", "r" },
                { "ς", "s" },
                { "σ", "s" },
                { "τ", "t" },
                { "υ", "y" }, // u
                { "φ", "ph" },
                { "χ", "ch" },
                { "ψ", "ps" },
                { "ω", "ō" },

                // Capital letters
                { "Α", "A" },
                { "Ἀ", "A" },
                { "Ἄ", "Á" },
                { "ᾈ", "A" },
                { "Ἁ", "A" },
                { "Ἅ", "Á" },
                { "Ά", "Á" },
                { "Ὰ", "À" },
                { "Ᾰ", "A" },
                { "Ᾱ", "Ā" },
                { "Ἂ", "Â" },
                { "Ἆ", "Â" },
                { "Ἃ", "Â" },
                { "Ἇ", "Â" },
                { "Α͂", "Â" },
                { "Αι", "Ā" },
                { "Άι", "Á̄" },
                { "Ὰι", "Ā̀" },
                { "Ἀι", "Āi" },
                { "Ἄι", "Ái" },
                { "Ἁι", "Āi" },
                { "Ἅι", "Ái" },
                { "Α͂ι", "Â̄" },
                { "Ἂι", "Âi" },
                { "Ἆι", "Âi" },
                { "Ἃι", "Âi" },
                { "Ἇι", "Âi" },
                { "Β", "B" },
                { "Γ", "G" },
                { "Δ", "D" },
                { "Ε", "E" },
                { "Ἐ", "E" },
                { "Ἔ", "É" },
                { "Ἒ", "Ê" },
                { "Ἑ", "E" },
                { "Ἕ", "É" },
                { "Ἓ", "Ê" },
                { "Έ", "É" },
                { "Ὲ", "È" },
                { "Ζ", "Z" },
                { "Η", "Ē" },
                { "Ἠ", "Ē" },
                { "Ἢ", "Ê" },
                { "Ἦ", "Ê" },
                { "Ἡ", "Hē" }, // Ē
                { "Ἣ", "Ê" },
                { "Ἧ", "Ê" },
                { "Ὴ", "Ḕ" },
                { "Η͂", "Ê" },
                { "Ἤ", "Ḗ" }, // Ḗ
                { "Ἥ", "Ḗ" }, // Ḗ
                { "Ή", "Ḗ" }, // Ḗ
                { "Ηι", "Ē̂" },
                { "Ήι", "Ē̄́" },
                { "Ὴι", "Ē̄̀" },
                { "Η͂ι", "Ê̄" },
                { "Ἠι", "Ēi" },
                { "Ἤι", "Ēí" },
                { "Ἢι", "Êi" },
                { "Ἦι", "Êi" },
                { "Ἡι", "Ēi" },
                { "Ἥι", "Ēí" },
                { "Ἣι", "Êi" },
                { "Ἧι", "Êi" },
                { "Θ", "Th" },
                { "Ἱ", "Hi" },
                { "Ι", "I" },
                { "Ἰ", "I" },
                { "Ί", "Í" },
                { "Ϊ", "Ï" },
                { "Ϊ́", "Ḯ" },
                { "Κ", "K" },
                { "Λ", "L" },
                { "Μ", "M" },
                { "Ν", "N" },
                { "Ξ", "X" },
                { "Ο", "O" },
                { "Ὀ", "Ō" },
                { "Ὄ", "Ó" },
                { "Ό", "Ó" },
                { "Π", "P" },
                { "Ρ", "R" },
                { "Ῥ", "Rh" },
                { "Σ", "S" },
                { "Τ", "T" },
                { "Υ", "U" },
                { "Υ̓", "Ú" },
                { "Ύ", "Ú" },
                { "Ϋ", "Ü" },
                { "Ϋ́", "Ǘ" },
                { "Φ", "Ph" },
                { "Χ", "Ch" },
                { "Ψ", "Ps" },
                { "Ω", "Ō" },
                { "Ώ", "Ṓ" }, // Ṓ

                // Lowercase letters
                { "ἄ", "á" },
                { "ἁ", "a" },
                { "ἅ", "á" },
                { "ά", "á" },
                { "ᾴ", "á̄" },
                { "ὰ", "à" },
                { "ᾲ", "ā̀" },
                { "ᾰ", "a" },
                { "ᾱ", "ā" },
                { "ᾳ", "ā" },
                { "ᾄ", "ái" },
                { "ᾀ", "āi" },
                { "ᾅ", "ái" },
                { "ᾁ", "āi" },
                { "ἂ", "â" },
                { "ἆ", "â" },
                { "ἃ", "â" },
                { "ἇ", "â" },
                { "ᾶ", "â" },
                { "ᾷ", "â̄" },
                { "ᾂ", "âi" },
                { "ᾆ", "âi" },
                { "ᾃ", "âi" },
                { "ᾇ", "âi" },
                { "ἐ", "e" },
                { "ἔ", "é" },
                { "ἒ", "ê" },
                { "ἑ", "e" },
                { "ἕ", "é" },
                { "ἓ", "ê" },
                { "έ", "é" },
                { "ὲ", "è" },
                { "ἠ", "ē" },
                { "ἢ", "ê" },
                { "ἦ", "ê" },
                { "ἡ", "ē" },
                { "ἣ", "ê" },
                { "ἧ", "ê" },
                { "ῄ", "ē̄́" },
                { "ὴ", "ḕ" },
                { "ῂ", "ē̄̀" },
                { "ῆ", "ê" },
                { "ῇ", "ê̄" },
                { "ῃ", "ē̂" },
                { "ἤ", "ḗ" }, // ḗ
                { "ἥ", "ḗ" }, // ḗ
                { "ή", "ḗ" }, // ḗ
                { "ᾔ", "ēí" },
                { "ᾒ", "êi" },
                { "ᾖ", "êi" },
                { "ᾐ", "ēi" },
                { "ᾕ", "ēí" },
                { "ᾓ", "êi" },
                { "ᾗ", "êi" },
                { "ᾑ", "ēi" },
                { "ί", "í" },
                { "ῖ", "ī" },
                { "ϊ", "ï" },
                { "ΐ", "ḯ" },
                { "ό", "ó" },
                { "ύ", "ú" },
                { "ϋ", "ü" },
                { "ΰ", "ǘ" },
                { "ὐ", "u" }, // u is the most common transliteration, not ú
                { "ώ", "ṓ" }, // ṓ

                // Additional characters
                { "ϵ", "e" }, // lunate epsilon
                { "ϑ", "th" }, // script theta
                { "ϱ", "r" }, // script rho
                { "ϖ", "p" }, // lunate pi
                { "ϰ", "k" }, // script kappa
                { "ϗ", "kai" }, // kai symbol
            };

            doricTransliterationTable = new Dictionary<string, string>
            {
                { "Ή", "Á" },
                { "Ἡ([^ρ])", "Ha$1" },

                { "([κλνπτ])η", "$1a" },
                { "([κλνπτ])ἤ", "$1á" },
                { "([κλνπτ])ἥ", "$1á" },
                { "([κλνπτ])ή", "$1á" },
                { "ε([ρ])", "a$1" },
                { "ίνεια", "inéa" },
            };
        }

        public string Transliterate(string text)
            => Transliterate(text, null);

        public string Transliterate(string text, string variant)
        {
            string transliteratedText = text;

            if (!string.IsNullOrWhiteSpace(variant))
            {
                if(variant.Equals("doric", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (string character in doricTransliterationTable.Keys)
                    {
                        transliteratedText = Regex.Replace(transliteratedText, character, doricTransliterationTable[character]);
                    }
                }
            }

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            transliteratedText = ApplyFixes(transliteratedText);

            return transliteratedText;
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "Ach", "Akh");
            fixedText = Regex.Replace(fixedText, "Ch([eé])", "Kh$1");
            fixedText = Regex.Replace(fixedText, "Cha", "Kha");
            fixedText = Regex.Replace(fixedText, "Kú", "Ký");

            fixedText = Regex.Replace(fixedText, "([aeio])y", "$1u");
            fixedText = Regex.Replace(fixedText, "([hk])ê([^n])", "$1ē$2");
            fixedText = Regex.Replace(fixedText, "([lr])[úý]", "$1ý");
            fixedText = Regex.Replace(fixedText, "[bv]([úý])", "bý");
            fixedText = Regex.Replace(fixedText, "b(ats|ion)", "v$1");
            fixedText = Regex.Replace(fixedText, "ch([eé])([ií])", "kh$1$2");
            fixedText = Regex.Replace(fixedText, "óvo", "óbo");
            fixedText = Regex.Replace(fixedText, "v(ai|os|y)", "b$1");

            return fixedText;
        }
    }
}
