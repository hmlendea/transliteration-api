using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NuciLog.Core;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class GreekTransliterator : Transliterator, ITransliterator
    {
        readonly Dictionary<string, string> modernGreekTransliterationTable;
        readonly Dictionary<string, string> ancientGreekTransliterationTable;
        readonly Dictionary<string, string> ancientDoricGreekTransliterationTable;

        public GreekTransliterator(ILogger logger) : base(logger)
        {
            // ISO 843
            modernGreekTransliterationTable = new()
            {
                // Uppercase exceptions
                { "Ευ([ήο])", "Ev$1" },
                { "Εύ([ήο])", "Ev́$1" },
                { "Αυ", "Au" },
                { "Αύ", "Aú" },
                { "Ευ", "Eu" },
                { "Εύ", "Eú" },
                { "Ηυ", "Īy" },
                { "Ου", "Ou" },
                { "Ού", "Oú" },

                // Lowercase exceptions
                { "ευ([ήο])", "ev$1" },
                { "εύ([ήο])", "ev́$1" },
                { "αυ", "au" },
                { "αύ", "aú" },
                { "ευ", "eu" },
                { "εύ", "eú" },
                { "ηυ", "īy" },
                { "ου", "ou" },
                { "ού", "oú" },

                // Uppercase letters
                { "Α", "A" },
                { "Ά", "Á" },
                { "Β", "V" },
                { "Γ", "G" },
                { "Δ", "D" },
                { "Ε", "E" },
                { "Έ", "É" },
                { "Η", "Ī" }, // I
                { "Ή", "Ī́" }, // Í
                { "Θ", "Th" },
                { "Ι", "I" },
                { "Ϊ", "Ï" },
                { "Ί", "Í" }, // Ï
                { "Κ", "K" },
                { "Λ", "L" },
                { "Μ", "M" },
                { "Ν", "N" },
                { "Ξ", "X" },
                { "Ο", "O" },
                { "Π", "P" },
                { "Ρ", "R" },
                { "Σ", "S" },
                { "Τ", "T" },
                { "Υ͂", "U" },
                { "Υ", "Y" },
                { "Ύ", "Ý" },
                { "Ϋ", "Ÿ" },
                { "Φ", "F" },
                { "Χ", "Ch" },
                { "Ψ", "Ps" },
                { "Ω", "Ō" },
                { "Ώ", "Ṓ" },
                { "Ζ", "Z" },

                // Lowercase letters
                { "α", "a" },
                { "ά", "á" },
                { "β", "v" },
                { "γ", "g" },
                { "δ", "d" },
                { "ε", "e" },
                { "έ", "é" },
                { "η", "ī" }, // i
                { "ή", "ī́" }, // í
                { "θ", "th" },
                { "ι", "i" },
                { "ϊ", "ï" },
                { "ΐ", "ḯ" },
                { "ί", "í" }, // ï
                { "κ", "k" },
                { "λ", "l" },
                { "μ", "m" },
                { "ν", "n" },
                { "ξ", "x" },
                { "ο", "o" },
                { "ό", "ó" },
                { "π", "p" },
                { "ρ", "r" },
                { "σ", "s" },
                { "ς", "s" },
                { "τ", "t" },
                { "ῦ", "u" },
                { "υ", "y" },
                { "ύ", "ý" },
                { "ϋ", "ÿ" },
                { "ΰ", "ÿ́" },
                { "φ", "f" },
                { "χ", "ch" },
                { "ψ", "ps" },
                { "ω", "ō" },
                { "ώ", "ṓ" },
                { "ζ", "z" }
            };

            ancientGreekTransliterationTable = new()
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
                { "Ἣ", "Ê" },
                { "Ἧ", "Ê" },
                { "Ὴ", "Ḕ" },
                { "Η͂", "Ê" },
                { "Ἤ", "Ḗ" }, // Ḗ
                { "Ἥ", "Ḗ" }, // Ḗ
                { "Ή", "Ḗ" }, // Hē, Ḗ
                { "Ἡ", "Hē" }, // Ē
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
                { "Ἴ", "Í" },
                { "Ί", "Í" },
                { "Ϊ", "Ï" },
                { "Ϊ́", "Ḯ" },
                { "Κ", "K" },
                { "Λ", "L" },
                { "Μ", "M" },
                { "Ν", "N" },
                { "Ξ", "X" },
                { "Ὅ", "Hó" },
                { "Ο", "O" },
                { "Ὀ", "Ō" },
                { "Ὄ", "Ó" },
                { "Ό", "Ó" },
                { "Ὸ", "Ò" },
                { "Π", "P" },
                { "Ρ", "R" },
                { "Ῥ", "Rh" },
                { "Σ", "S" },
                { "Τ", "T" },
                { "Ὑ", "Hy" },
                { "Υ", "U" },
                { "Υ̓", "Ú" },
                { "Ύ", "Ú" },
                { "Υ͂", "Û" },
                { "Ϋ", "Ü" },
                { "Ϋ́", "Ǘ" },
                { "Φ", "Ph" },
                { "Χ", "Ch" },
                { "Ψ", "Ps" },
                { "Ω", "Ō" },
                { "Ώ", "Ṓ" }, // Ṓ
                { "Ω͂", "Ô" },

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
                { "ή", "ḗ" }, // ē, ḗ
                { "ᾔ", "ēí" },
                { "ᾒ", "êi" },
                { "ᾖ", "êi" },
                { "ᾐ", "ēi" },
                { "ᾕ", "ēí" },
                { "ᾓ", "êi" },
                { "ᾗ", "êi" },
                { "ᾑ", "ēi" },
                { "ὑ", "hy" },
                { "ί", "í" },
                { "ῖ", "ī" },
                { "ϊ", "ï" },
                { "ΐ", "ḯ" },
                { "ἴ", "í" },
                { "ό", "ó" },
                { "ὸ", "ò" },
                { "ύ", "ú" },
                { "ῦ", "û" },
                { "ϋ", "ü" },
                { "ΰ", "ǘ" },
                { "ὐ", "u" }, // u is the most common transliteration, not ú
                { "ώ", "ṓ" }, // ṓ
                { "ῶ", "ô" },

                // Additional characters
                { "ϵ", "e" }, // lunate epsilon
                { "ϑ", "th" }, // script theta
                { "ϱ", "r" }, // script rho
                { "ϖ", "p" }, // lunate pi
                { "ϰ", "k" }, // script kappa
                { "ϗ", "kai" }, // kai symbol
            };

            ancientDoricGreekTransliterationTable = new()
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

        protected override string PerformTransliteration(string text, Language language)
        {
            string transliteratedText = text;


            if (language.Equals(Language.AncientGreek) ||
                language.Equals(Language.AncientGreekDoric))
            {
                if (language.Equals(Language.AncientGreekDoric))
                {
                    foreach (string character in ancientDoricGreekTransliterationTable.Keys)
                    {
                        transliteratedText = Regex.Replace(transliteratedText, character, ancientDoricGreekTransliterationTable[character]);
                    }
                }

                foreach (string character in ancientGreekTransliterationTable.Keys)
                {
                    transliteratedText = Regex.Replace(transliteratedText, character, ancientGreekTransliterationTable[character]);
                }
            }
            else if (language.Equals(Language.Greek))
            {
                foreach (string character in modernGreekTransliterationTable.Keys)
                {
                    transliteratedText = Regex.Replace(transliteratedText, character, modernGreekTransliterationTable[character]);
                }
            }
            else
            {
                throw new ArgumentException($"The {language.Name} language is not supported by the {nameof(GreekTransliterator)}");
            }

            transliteratedText = ApplyFixes(transliteratedText, language);

            return transliteratedText;
        }

        string ApplyFixes(string text, Language language)
        {
            string fixedText = text;

            if (language.Equals(Language.AncientGreek) ||
                language.Equals(Language.AncientGreekDoric))
            {
                fixedText = ApplyAncientFixes(fixedText);
            }

            return fixedText;
        }


        static string ApplyAncientFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "Ach", "Akh");
            fixedText = Regex.Replace(fixedText, "Ch([eé])", "Kh$1");
            fixedText = Regex.Replace(fixedText, "Cha", "Kha");
            fixedText = Regex.Replace(fixedText, "Ḗl", "Hēl");
            fixedText = Regex.Replace(fixedText, "Kú", "Ký");

            fixedText = Regex.Replace(fixedText, "([aeio])y", "$1u");
            fixedText = Regex.Replace(fixedText, "([hk])ê([^n])", "$1ē$2");
            fixedText = Regex.Replace(fixedText, "([lr])[úý]", "$1ý");
            fixedText = Regex.Replace(fixedText, "[bv]([úý])", "bý");
            fixedText = Regex.Replace(fixedText, "b(ats|ion)", "v$1");
            fixedText = Regex.Replace(fixedText, "ch([eé])([ií])", "kh$1$2");
            fixedText = Regex.Replace(fixedText, "gú", "gý");
            fixedText = Regex.Replace(fixedText, "óvo", "óbo");
            fixedText = Regex.Replace(fixedText, "v(ai|os|y)", "b$1");

            return fixedText;
        }
    }
}
