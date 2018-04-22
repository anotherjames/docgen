using System;
using System.Threading.Tasks;

namespace DocGen.Tests
{
    public static class AssertExtensions
    {
        public static void Exception(Action action, Action<Exception> assert)
        {
            try
            {
                action();
                throw new Exception("No exception was thrown, but was expected");
            }catch(Exception ex)
            {
                assert(ex);
            }
        }

        public static async Task ExceptionAsync(Func<Task> action, Action<Exception> assert)
        {
            try
            {
                await action();
                throw new Exception("No exception was thrown, but was expected");
            }
            catch (Exception ex)
            {
                assert(ex);
            }
        }
    }
}