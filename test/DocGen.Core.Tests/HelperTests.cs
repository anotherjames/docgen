using System;
using Xunit;

namespace DocGen.Core.Tests
{
    public class HelperTests
    {
        [Theory]
        [InlineData("/test", "test2", "/test/test2")]
        [InlineData("/test", "/test2", "/test2")]
        [InlineData("/test", "../test2", "/test2")]
        [InlineData("/test", "../test2/../test3", "/test3")]
        [InlineData("/test?query=ignored", "test2", "/test/test2")]
        [InlineData("/test?query=ignored", "test2?query=preserved", "/test/test2?query=preserved")]
        [InlineData("/test?#ignored", "test2", "/test/test2")]
        [InlineData("/test?#ignored", "test2#preserved", "/test/test2#preserved")]
        public void Can_resolve_relative_path(string input, string relative, string expected)
        {
            var result = Helpers.ResolvePathPart(input, relative);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Can_not_resolve_url_when_path_walks_out_of_root()
        {
            try
            {
                Helpers.ResolvePathPart("/test", "../../test");
                Assert.True(false, "Succeeded, when it should have failed.");
            }
            catch (Exception ex)
            {
                Assert.Equal("Invalid path '../../test'", ex.Message);
            }
        }
    }
}