namespace DocGen.Web.Impl
{
    public class WebBuilder : IWebBuilder
    {
        public IWeb Build(int port = 8000)
        {
            return new Web();
        }
    }
}