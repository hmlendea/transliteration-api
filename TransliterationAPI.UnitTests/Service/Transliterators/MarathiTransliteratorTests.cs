using NUnit.Framework;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class MarathiTransliteratorTests
    {
        private IMarathiTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new MarathiTransliterator();
        }

        [Test]
        [TestCase("अमरावती", "Amrāvatī")]
        [TestCase("अहमदनगर", "Ahmadnagar")]
        [TestCase("इंदापूर", "Indāpūr")]
        [TestCase("उस्मानाबाद", "Usmānābād")]
        [TestCase("ओस्मानाबाद", "Osamānābād")]
        [TestCase("औरंगाबाद", "Aurangābād")]
        [TestCase("कोल्हापूर", "Kōlhāpūr")]
        [TestCase("गढचिरौळी", "Gadachiraulī")]
        [TestCase("गोंदिया", "Gōndiyā")]
        [TestCase("चंद्रपूर", "Chandrāpur")]
        [TestCase("जळगाव", "Jaḷagāv")]
        [TestCase("जालना", "Jālnā")]
        [TestCase("ठाणे", "Ṭhāṇē")]
        [TestCase("तलासरी", "Talāsarī")]
        [TestCase("धर्माबाद", "Dharmābād")]
        [TestCase("धुळे", "Dhuḷe")]
        [TestCase("नागपूर", "Nāgāpūr")]
        [TestCase("नांदेड", "Nāndēd")]
        [TestCase("नासिक", "Nāsik")]
        [TestCase("पणवेल", "Paṇvēl")]
        [TestCase("परभणी", "Parabhaṇī")]
        [TestCase("पालघर", "Pālaghar")]
        [TestCase("बुलडाणा", "Buḷḍāṇā")]
        [TestCase("बोरिवली", "Borivlī")]
        [TestCase("भंडारा", "Bhāṇḍārā")]
        [TestCase("माल्कापूर", "Mālkāpūr")]
        [TestCase("माळेगाव", "Māḷēgāv")]
        [TestCase("मीराज", "Mīrāj")]
        [TestCase("यवतमाळ", "Yavatamāḷ")]
        [TestCase("येवला", "Yēvalā")]
        [TestCase("रत्नागिरी", "Ratnāgirī")]
        [TestCase("लातूर", "Lātūr")]
        [TestCase("वाशिम", "Vāśim")]
        [TestCase("वासई", "Vāsai")]
        [TestCase("शहादोल", "Śahādōl")]
        [TestCase("शिरडी", "Śirḍī")]
        [TestCase("शिरुर", "Śirur")]
        [TestCase("सांगली", "Sāṅglī")]
        [TestCase("सांजागाव", "Sāṅjāgāv")]
        [TestCase("सातारा", "Sātārā")]
        [TestCase("सोलापूर", "Solāpūr")]
        [TestCase("हिंगोली", "Hiṅgōlī")]
        public void GivenATextInMarathiScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string marathiText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(marathiText), Is.EqualTo(expectedTransliteratedText));
    }
}
