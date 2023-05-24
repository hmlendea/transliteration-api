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
        [TestCase("ᾈγίοι Ανάργυροι", "Agíoi Anárgyroi")]
        [TestCase("Ἀθῆναι", "Athênai")]
        [TestCase("Ᾰθῆναι", "Athênai")]
        [TestCase("Ἀίγινα", "Aígina")]
        [TestCase("Ἀίγυπτος", "Aígyptos")]
        [TestCase("Ᾰκρόπολις", "Akrópolis")]
        [TestCase("Ἀλεξάνδρεια", "Alexándreia")]
        [TestCase("Ἄνδρος", "Ándros")]
        [TestCase("Ἀντιόχεια τῆς Μυγδονίας", "Antiókheia tês Mygdonías")]
        [TestCase("Ᾰπολλωνία", "Apollōnía")]
        [TestCase("Ἄργος", "Árgos")]
        [TestCase("Ἀρκαδία", "Arkadía")]
        [TestCase("Ἄρτα", "Árta")]
        [TestCase("Ασκαλων", "Askalōn")]
        [TestCase("Ἄστυπαλαια", "Ástypalaia")]
        [TestCase("Ἀχαία", "Akhaía")]
        [TestCase("Βαβυλών", "Babylṓn")]
        [TestCase("Βυζάντιον", "Byzántion")]
        [TestCase("Δαμασκός", "Damaskós")]
        [TestCase("Δελφοί", "Delphoí")]
        [TestCase("Ελευσίσ", "Eleusís")]
        [TestCase("Ἐλευσίς", "Eleusís")]
        [TestCase("Ἐπίδαυρος", "Epídauros")]
        [TestCase("Ἐρέτρια", "Erétria")]
        [TestCase("Ήπειρος", "Ḗpeiros")]
        [TestCase("Ἡράκλεια", "Hērákleia")]
        [TestCase("Ἡράκλειον", "Hērákleion")]
        [TestCase("Θεσσαλονίκη", "Thessaloníkē")]
        [TestCase("Θῆβαι", "Thēbai")]
        [TestCase("Ἰουερνία", "Iouernía")]
        [TestCase("Ἰουερνίς", "Iouernís")]
        [TestCase("Ἰταβύριον", "Itabýrion")]
        [TestCase("Κασθαναία", "Kasthanaía")]
        [TestCase("Κασταναία", "Kastanaía")]
        [TestCase("Κνωσός", "Knōsós")]
        [TestCase("Κόρινθος", "Kórinthos")]
        [TestCase("Κορυτσά", "Korytsá")]
        [TestCase("Κορωνέα", "Korōnéa")]
        [TestCase("Κρούσεβατς", "Kroúsevats")]
        [TestCase("Κύθηρα", "Kýthēra")]
        [TestCase("Κύπρος", "Kýpros")]
        [TestCase("Κυρήνη", "Kyrḗnē")]
        [TestCase("Κωνσταντινούπολις", "Kōnstantinoúpolis")]
        [TestCase("Λέσβος", "Lésbos")]
        [TestCase("Μακεδονία", "Makedonía")]
        [TestCase("Μεσσήνη", "Messḗnē")]
        [TestCase("Μίλητος", "Mílētos")]
        [TestCase("Μυγδονία", "Mygdonía")]
        [TestCase("Νάξος", "Náxos")]
        [TestCase("Νεάπολις", "Neápolis")]
        [TestCase("Ολυμπία", "Olympía")]
        [TestCase("Ὄλυνθος", "Ólynthos")]
        [TestCase("Οὐινδόβονα", "Oúindóbona")]
        [TestCase("Παλαιστίνη", "Palaistínē")]
        [TestCase("Παραλία", "Paralía")]
        [TestCase("Πελοπόννησος", "Pelopónnēsos")]
        [TestCase("Ῥόδος", "Rhódos")]
        [TestCase("Ῥώμη", "Rhṓmē")]
        [TestCase("Σπάρτη", "Spártē")]
        [TestCase("Φιλαδέλφεια", "Philadélpheia")]
        public void GivenATextInAncientGreekScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string ancientGreekText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(ancientGreekText), Is.EqualTo(expectedTransliteratedText));
    }
}
