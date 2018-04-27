using System;
using Xunit;

namespace DocGen.Web.Requirements.Tests
{
    public class MenuStoreTests
    {
        IMenuStore _menuStore;
        
        public MenuStoreTests()
        {
            _menuStore = new Impl.MenuStore();
        }
        
        [Fact]
        public void Can_get_menu()
        {
            _menuStore.AddPage("/", "Home", 0);
            _menuStore.AddPage("/test1", "Test 1", 1);
            _menuStore.AddPage("/test2", "Test 2", 2);

            var menu = _menuStore.BuildMenu("/");
            
            Assert.NotNull(menu);
            Assert.Equal("/", menu.Path);
            Assert.Equal(2, menu.Children.Count);
            Assert.Equal("/test1", menu.Children[0].Path);
            Assert.Equal("/test2", menu.Children[1].Path);
        }
        
        [Fact]
        public void Current_page_returns_director_child_pages_only()
        {
            _menuStore.AddPage("/", "Home", 0);
            _menuStore.AddPage("/test1", "Test 1", 1);
            _menuStore.AddPage("/test2", "Test 2", 2);
            _menuStore.AddPage("/test2/test3", "Test 3", 3);

            var menu = _menuStore.BuildMenu("/");
            
            // The root shoudl be return with only direct
            // child nodes expanded
            Assert.NotNull(menu);
            Assert.Equal("/", menu.Path);
            Assert.Equal(2, menu.Children.Count);
            Assert.Equal("/test1", menu.Children[0].Path);
            Assert.Equal("/test2", menu.Children[1].Path);
            Assert.Equal(0, menu.Children[1].Children.Count);
        }
    }
}
