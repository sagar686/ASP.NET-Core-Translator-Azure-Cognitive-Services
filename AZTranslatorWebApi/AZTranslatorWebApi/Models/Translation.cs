using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AZTranslatorWebApi.Models
{
    public class Translation
    {
        public string Text { get; set; }
        public TextResult Transliteration { get; set; }
        public string To { get; set; }
        public Alignment Alignment { get; set; }
        public SentenceLength SentLen { get; set; }
    }
    public class Alignment
    {
        public string Proj { get; set; }
    }

    public class SentenceLength
    {
        public int[] SrcSentLen { get; set; }
        public int[] TransSentLen { get; set; }
    }

    public class TranslationInput
    {
        public string Text { get; set; }
        public string TargetLanguage { get; set; }
    }
}
