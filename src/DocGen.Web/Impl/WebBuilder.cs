namespace DocGen.Web.Impl
{
    public class WebBuilder : IWebBuilder
    {
        public IWeb Build(IWebContext webContext, int port = 8000)
        {
            return new Web(webContext, port);
        }
    }
}