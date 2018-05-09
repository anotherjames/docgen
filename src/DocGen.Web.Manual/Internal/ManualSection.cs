namespace DocGen.Web.Manual.Internal
{
    public class ManualSection
    {
        public ManualSection(string title, int order, string markdown, string file)
        {
            Title = title;
            Order = order;
            Markdown = markdown;
            File = file;
        }
        
        public string Title { get; }
        
        public int Order { get; }
        
        public string Markdown { get; }
        
        public string File { get; }
    }
}