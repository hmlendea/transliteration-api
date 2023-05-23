using NUnit.Framework;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class AncientGreekTransliteratorTests
    {
        private IAncientGreekTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new AncientGreekTransliterator();
        }

        [Test]
        [TestCase("Ἀθῆναι", "Athênai")]
        [TestCase("Ἀντιόχεια τῆς Μυγδονίας", "Antiókheia tês Mygdonías")]
        [TestCase("Ασκαλων", "Askalōn")]
        [TestCase("Βαβυλών", "Babylṓn")]
        [TestCase("Βυζάντιον", "Byzántion")]
        [TestCase("Δαμασκός", "Damaskós")]
        [TestCase("Ελευσίσ", "Eleusís")]
        [TestCase("Ήπειρος", "Ḗpeiros")]
        [TestCase("Βαβυλών", "Babylṓn")]
        [TestCase("Θεσσαλονίκη", "Thessaloníkē")]
        [TestCase("Ἰουερνία", "Ἰouernía")]
        [TestCase("Ἰουερνίς", "Ἰouernís")]
        [TestCase("Κόρινθος", "Kórinthos")]
        [TestCase("Κρούσεβατς", "Kroúsevats")]
        [TestCase("Κωνσταντινούπολις", "Kōnstantinoúpolis")]
        [TestCase("Μακεδονία", "Makedonía")]
        [TestCase("Μυγδονία", "Mygdonía")]
        [TestCase("Ῥώμη", "Rhṓmē")]
        [TestCase("Σπάρτη", "Spártē")]
        [TestCase("Φιλαδέλφεια", "Philadélpheia")]
        public void GivenATextInAncientGreekScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string ancientGreekText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(ancientGreekText), Is.EqualTo(expectedTransliteratedText));
    }
}
