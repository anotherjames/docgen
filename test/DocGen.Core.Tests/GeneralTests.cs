using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace DocGen.Core.Tests
{
    public class GeneralTests
    {
        [Fact]
        public void Can_normalize_line_endings()
        {
            Assert.Equal($"test1{Environment.NewLine}test2{Environment.NewLine}test3", "test1\ntest2\r\ntest3".NormalizeLineEndings());
        }
    }
}