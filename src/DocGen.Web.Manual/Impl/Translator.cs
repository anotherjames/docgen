namespace DocGen.Web.Manual.Impl
{
    public class Translator : ITranslator
    {
        public string Translate(string language, string content)
        {
            // TODO:
            return $"{language}-{content}";
        }
    }
}