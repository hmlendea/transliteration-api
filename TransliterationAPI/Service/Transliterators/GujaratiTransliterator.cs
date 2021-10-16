using System.Text.RegularExpressions;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class GujaratiTransliterator : IGujaratiTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public GujaratiTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public string Transliterate(string text)
        {
            string[] guc = new string[] { "ક", "ખ", "ગ", "ઘ", "ચ", "છ", "જ", "ઝ", "ટ", "ઠ", "ડ", "ઢ", "ણ", "ત", "થ", "દ", "ધ", "ન", "પ", "ફ", "બ", "ભ", "મ", "ય", "ર", "લ", "વ", "શ", "ષ", "સ", "હ", "ળ", "ઞ" };
            string[] enc = new string[] { "ka", "kha", "ga", "gha", "cha", "chha", "ja", "za", "ṭa", "ṭha", "ḍa", "ḍha", "ṇa", "ta", "tha", "da", "dha", "na", "pa", "fa", "ba", "bha", "ma", "ya", "ra", "la", "va", "sha", "ṣha", "sa", "ha", "ḷa", "nya" };
            string[] guh = new string[] { "ક્", "ખ્", "ગ્", "ઘ્", "ચ્", "છ્", "જ્", "ઝ્", "ટ્", "ઠ્", "ડ્", "ઢ્", "ણ્", "ત્", "થ્", "દ્", "ધ્", "ન્", "પ્", "ફ્", "બ્", "ભ્", "મ્", "ય્", "ર્", "લ્", "વ્", "શ્", "ષ્", "સ્", "હ્", "ળ્", "ઞ્" };
            string[] enh = new string[] { "k", "kh", "g", "gh", "ch", "chh", "j", "z", "ṭ", "ṭh", "ḍ", "ḍh", "ṇ", "t", "th", "d", "dh", "n", "p", "f", "b", "bh", "m", "y", "r", "l", "v", "sh", "ṣh", "s", "h", "ḷ", "ny" };
            string[] guv = new string[] { "ઓ", "ઔ", "આ", "ઇ", "ઈ", "ઉ", "ઊ", "એ", "ઐ", "ઍ", "ઑ", "ૠ", "અ" };
            string[] env = new string[] { "o", "au", "ā", "i", "ī", "u", "ū", "e", "ai", "ĕ", "ŏ", "ṛu", "a" };
            string[] guvs = new string[] { "િ", "ી", "ુ", "ૂ", "ે", "ૈ", "ો", "ૌ", "ૅ", "ૉ", "ં", "ૃ", "્", "ઃ", "ા" };
            string[] envs = new string[] { "i", "ī", "u", "ū", "e", "ai", "o", "au", "ĕ", "ŏ", "an", "ṛu", "", "ah", "ā" };
            string[] gun = new string[] { "૦", "૧", "૨", "૩", "૪", "૫", "૬", "૭", "૮", "૯" };
            string[] enn = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] encomb = new string[] { "kh", "gh", "ch", "chh", "ṭh", "ḍh", "th", "dh", "bh", "sh", "ṣh", "ny" };

            string s;
            string r;
            string transliteratedText = string.Empty;

            for (int i = 0; i < guc.Length; i++) {
                for (int j = 0; j < guvs.Length; j++) {
                    s = guc[i] + guvs[j] + "ઃ ";
                    r = enh[i] + envs[j] + "h" + envs[j] + " ";
                    transliteratedText = text.Replace(s, r);
                    s = guc[i] + guvs[j] + "ઃ";
                    r = enh[i] + envs[j] + "h";
                    transliteratedText = transliteratedText.Replace(s, r);
                }
            }

            for (int i = 0; i < guc.Length; i++) {
                for (int j = 0; j < guvs.Length; j++) {
                    s = guc[i] + guvs[j] + "ં";
                    r = enh[i] + envs[j] + "n";
                    transliteratedText = transliteratedText.Replace(s, r);
                }
            }

            for (int i = 0; i < guv.Length; i++) {
                s = guv[i] + "ં";
                r = env[i] + "n";
                transliteratedText = transliteratedText.Replace(s, r);
            }

            for (int i = 0; i < guc.Length; i++) {
                for (int j = 0; j < guvs.Length; j++) {
                    s = guc[i] + guvs[j];
                    r = enh[i] + envs[j];
                    transliteratedText = transliteratedText.Replace(s, r);
                }
            }

            transliteratedText = ReplaceArray(guh, enh, transliteratedText);
            transliteratedText = ReplaceArray(guvs, envs, transliteratedText);
            transliteratedText = ReplaceArray(guv, env, transliteratedText);
            transliteratedText = ReplaceArray(guc, enc, transliteratedText);
            transliteratedText = ReplaceArray(gun, enn, transliteratedText);

            for (int i = 0; i < enc.Length; i++) {
                var pat = enc[i] + " ";
                var rep = enh[i] + " ";
                transliteratedText = transliteratedText.Replace(pat, rep);
            }

            for (int i = 0; i < enh.Length; i++) {
                for (int j = 0; j < enc.Length; j++) {
                    var pat = enh[i] + enh[j] + " ";
                    var rep = enh[i] + enc[j] + " ";
                    transliteratedText = transliteratedText.Replace(pat, rep);
                }
            }

            for (int i = 0; i < env.Length; i++) {
                for (int j = 0; j < encomb.Length; j++) {
                    var pat = env[i] + encomb[j] + "a ";
                    var rep = env[i] + encomb[j] + " ";
                    transliteratedText = transliteratedText.Replace(pat, rep);
                }
            }

            return ApplyFixes(transliteratedText);
        }

        string ReplaceArray(string[] a, string[] b, string text) {
            if (a.Length != b.Length) {
                return null;
            }

            for (int i = 0; i < a.Length; i++) {
                text = text.Replace(a[i], b[i]);
            }

            return text;
        }

        string ApplyFixes(string text)
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
