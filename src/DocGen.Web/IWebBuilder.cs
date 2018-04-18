namespace DocGen.Web
{
    public interface IWebBuilder
    {
        IWeb Build(int port = 8000);
    }
}