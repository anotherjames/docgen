using System;
using Newtonsoft.Json;
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

        [Fact]
        public void Current_page_returns_all_empty_leafs_to_render_children()
        {
            _menuStore.AddPage("/", "Home", 0);
            _menuStore.AddPage("/test1", "Test 1", 1);
            _menuStore.AddPage("/nonexistant/test2", "Test 2", 2);
            _menuStore.AddPage("/nonexistant/test3", "Test 3", 2);
            _menuStore.AddPage("/nonexistant/another/test4", "Test 4", 2);

            var menu = _menuStore.BuildMenu("/");
            
            Console.WriteLine(JsonConvert.SerializeObject(menu));

            Assert.NotNull(menu);
            Assert.Equal(2, menu.Children.Count);
            var test1 = menu.Children[0];
            var nonexistant = menu.Children[1];
            Assert.Equal(3, nonexistant.Children.Count);
            var test2 = nonexistant.Children[0];
            var test3 = nonexistant.Children[1];
            var another = nonexistant.Children[2];
            Assert.Equal(1, another.Children.Count);
            var test4 = another.Children[0];
            
            Assert.Equal("/", menu.Path);
            Assert.Equal("/test1", test1.Path);
            Assert.Equal("/nonexistant", nonexistant.Path);
            Assert.Equal("/nonexistant/test2", test2.Path);
            Assert.Equal("/nonexistant/test3", test3.Path);
            Assert.Equal("/nonexistant/another", another.Path);
            Assert.Equal("/nonexistant/another/test4", test4.Path);
            
            Assert.False(menu.IsEmptyParent);
            Assert.False(test1.IsEmptyParent);
            Assert.True(nonexistant.IsEmptyParent);
            Assert.False(test2.IsEmptyParent);
            Assert.False(test3.IsEmptyParent);
            Assert.True(another.IsEmptyParent);
            Assert.False(test4.IsEmptyParent);
        }
        
    }
}
