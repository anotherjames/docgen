namespace DocGen.Web.Manual.Internal
{
    public class ManualSection
    {
        public ManualSection(int order, string markdown, string file)
        {
            Order = order;
            Markdown = markdown;
            File = file;
        }
        
        public int Order { get; }
        
        public string Markdown { get; }
        
        public string File { get; }
    }
}