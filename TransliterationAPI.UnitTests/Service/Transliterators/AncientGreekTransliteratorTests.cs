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
        [TestCase("Αττική", "Attikḗ")]
        [TestCase("Ἀχαία", "Akhaía")]
        [TestCase("Βαβυλών", "Babylṓn")]
        [TestCase("Βυζάντιον", "Byzántion")]
        [TestCase("Δαμασκός", "Damaskós")]
        [TestCase("Δελφοί", "Delphoí")]
        [TestCase("Δωδώνη", "Dōdṓnē")]
        [TestCase("Ελευσίσ", "Eleusís")]
        [TestCase("Ἐλευσίς", "Eleusís")]
        [TestCase("Ἐπίδαυρος", "Epídauros")]
        [TestCase("Ἐρέτρια", "Erétria")]
        [TestCase("Ἐτεόκρητη", "Eteókrētē")]
        [TestCase("Ήπειρος", "Ḗpeiros")]
        [TestCase("Ἡράκλεια Σιντική", "Hērákleia Sintikḗ")]
        [TestCase("Ἡράκλεια", "Hērákleia")]
        [TestCase("Ἡράκλειον", "Hērákleion")]
        [TestCase("Ἡφαιστία", "Hēphaistía")]
        [TestCase("Θεσσαλονίκη", "Thessaloníkē")]
        [TestCase("Θῆβαι", "Thēbai")]
        [TestCase("Ἰουερνία", "Iouernía")]
        [TestCase("Ἰουερνίς", "Iouernís")]
        [TestCase("Ἰταβύριον", "Itabýrion")]
        [TestCase("Κασθαναία", "Kasthanaía")]
        [TestCase("Κασσώπη", "Kassṓpē")]
        [TestCase("Κασταναία", "Kastanaía")]
        [TestCase("Κερύνεια", "Kerýneia")]
        [TestCase("Κεφαλληνία", "Kephallēnía")]
        [TestCase("Κνωσός", "Knōsós")]
        [TestCase("Κόρινθος", "Kórinthos")]
        [TestCase("Κορόπη", "Korópē")]
        [TestCase("Κορυτσά", "Korytsá")]
        [TestCase("Κορωνέα", "Korōnéa")]
        [TestCase("Κορώνη", "Korṓnē")]
        [TestCase("Κουλουκιά", "Kouloukiá")]
        [TestCase("Κρήτη", "Krḗtē")]
        [TestCase("Κρούσεβατς", "Kroúsevats")]
        [TestCase("Κύθηρα", "Kýthēra")]
        [TestCase("Κυλλήνη", "Kyllḗnē")]
        [TestCase("Κύπρος", "Kýpros")]
        [TestCase("Κυρήνη", "Kyrḗnē")]
        [TestCase("Κωνσταντινούπολις", "Kōnstantinoúpolis")]
        [TestCase("Λέσβος", "Lésbos")]
        [TestCase("Μακεδονία", "Makedonía")]
        [TestCase("Μαντίνεια", "Mantíneia")]
        [TestCase("Μεσσήνη", "Messḗnē")]
        [TestCase("Μίλητος", "Mílētos")]
        [TestCase("Μυγδονία", "Mygdonía")]
        [TestCase("Νάξος", "Náxos")]
        [TestCase("Νεάπολις", "Neápolis")]
        [TestCase("Ολυμπία", "Olympía")]
        [TestCase("Ὄλυνθος", "Ólynthos")]
        [TestCase("Οὐινδόβονα", "Oúindóbona")]
        [TestCase("Παλαιστίνη", "Palaistínē")]
        [TestCase("Παλλήνη", "Pallḗnē")]
        [TestCase("Παραλία", "Paralía")]
        [TestCase("Πελοπόννησος", "Pelopónnēsos")]
        [TestCase("Ῥόδος", "Rhódos")]
        [TestCase("Ῥώμη", "Rhṓmē")]
        [TestCase("Σπάρτη", "Spártē")]
        [TestCase("Φιλαδέλφεια", "Philadélpheia")]
        [TestCase("Χαλάστρα", "Khalástra")]
        [TestCase("Χαλέστρη", "Khaléstrē")]
        [TestCase("Χαλκιδική", "Khalkidikḗ")]
        public void GivenATextInAncientGreekScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string ancientGreekText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(ancientGreekText), Is.EqualTo(expectedTransliteratedText));

        [Test]
        [TestCase("Ᾰθῆναι", "Athênai")]
        [TestCase("Αττική", "Attiká")]
        [TestCase("Δᾶλος", "Dâlos")]
        [TestCase("Ἐτεόκρητη", "Eteókrēta")]
        [TestCase("Ήπειρος", "Ápeiros")]
        [TestCase("Ἡράκλεια Σιντική", "Hērákleia Sintiká")]
        [TestCase("Ἡφαιστία", "Haphaistía")]
        [TestCase("Κασσώπη", "Kassṓpa")]
        [TestCase("Κερύνεια", "Karýneia")]
        [TestCase("Κεφαλληνία", "Kephallanía")]
        [TestCase("Κορόπη", "Korópa")]
        [TestCase("Κορώνη", "Korṓna")]
        [TestCase("Κρήτη", "Krḗta")]
        [TestCase("Κυλλήνη", "Kyllána")]
        [TestCase("Μαντίνεια", "Mantinéa")]
        [TestCase("Παλλήνη", "Pallána")]
        [TestCase("Χαλκιδική", "Khalkidiká")]
        public void GivenATextInAncientGreekDoricScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string ancientGreekDoricText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(ancientGreekDoricText, "doric"), Is.EqualTo(expectedTransliteratedText));
    }
}
