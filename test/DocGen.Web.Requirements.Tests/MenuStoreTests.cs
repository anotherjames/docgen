using System;
using DocGen.Requirements;
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

        [Fact]
        public void Can_return_selected_page()
        {
            _menuStore.AddPage("/", "Home", 0);
            _menuStore.AddPage("/test1", "Test 1", 1);
            _menuStore.AddPage("/test1/test2", "Test 2", 2);
            _menuStore.AddPage("/test1/test3", "Test 3", 3);
            _menuStore.AddPage("/test1/test4", "Test 4", 4);
            _menuStore.AddPage("/test1/test4/test5", "Test 5", 5);

            var menu = _menuStore.BuildMenu("/test1");
            
            Assert.Equal(1, menu.Children.Count);
            var test1 = menu.Children[0];
            Assert.Equal("/test1", test1.Path);
            Assert.Equal(3, test1.Children.Count);
            Assert.Equal("/test1/test2", test1.Children[0].Path);
            Assert.Equal("/test1/test3", test1.Children[1].Path);
            Assert.Equal("/test1/test4", test1.Children[2].Path);
            var test4 = test1.Children[2];
            Assert.Equal(0, test4.Children.Count);

            menu = _menuStore.BuildMenu("/test1/test4");
            Assert.Equal(1, menu.Children.Count);
            test1 = menu.Children[0];
            Assert.Equal("/test1", test1.Path);
            Assert.Equal(1, test1.Children.Count);
            test4 = test1.Children[0];
            Assert.Equal("/test1/test4", test4.Path);
            Assert.Equal(1, test4.Children.Count);
            var test5 = test4.Children[0];
            Assert.Equal("/test1/test4/test5", test5.Path);
        }

        [Fact]
        public void Can_order_pages()
        {
            _menuStore.AddPage("/", "Home", 0);
            _menuStore.AddPage("/test1", "Test 1", 1);
            _menuStore.AddPage("/test1/test2", "Test 2", 2);
            _menuStore.AddPage("/test1/test3", "Test 3", 0);
            _menuStore.AddPage("/test1/test4", "Test 4", 1);
            _menuStore.AddPage("/test1/test4/test5", "Test 5", 5);
            _menuStore.AddPage("/test6", "Test 6", 0);

            var menu = _menuStore.BuildMenu("/");
            
            Assert.Equal(2, menu.Children.Count);
            var test1 = menu.Children[1];
            var test6 = menu.Children[0];
            Assert.Equal("/test1", test1.Path);
            Assert.Equal("/test6", test6.Path);

            menu = _menuStore.BuildMenu("/test1");
            
            Assert.Equal(1, menu.Children.Count);
            test1 = menu.Children[0];
            var test2 = test1.Children[2];
            var test3 = test1.Children[0];
            var test4 = test1.Children[1];
            Assert.Equal("/test1/test2", test2.Path);
            Assert.Equal("/test1/test3", test3.Path);
            Assert.Equal("/test1/test4", test4.Path);
        }

        [Fact]
        public void Non_existant_path_uses_first_existing_as_context()
        {
            _menuStore.AddPage("/", "Home", 0);
            _menuStore.AddPage("/test1", "Test 1", 1);
            _menuStore.AddPage("/test1/test2", "Test 2", 2);
            _menuStore.AddPage("/test1/test3", "Test 3", 3);
            _menuStore.AddPage("/test1/test4", "Test 4", 4);
            _menuStore.AddPage("/test1/test4/test5", "Test 5", 5);
            _menuStore.AddPage("/test6", "Test 6", 6);

            var menu = _menuStore.BuildMenu("/sdfa/we/sdfg/sdfg/");
            
            Assert.Equal(2, menu.Children.Count);
            var test1 = menu.Children[0];
            var test6 = menu.Children[1];
            Assert.Equal("/test1", test1.Path);
            Assert.Equal("/test6", test6.Path);
            Assert.Equal(0, test1.Children.Count);
            Assert.Equal(0, test6.Children.Count);
            
            menu = _menuStore.BuildMenu("/test1/sdf/weasd/sdfg/wef");
            
            Assert.Equal(1, menu.Children.Count);
            test1 = menu.Children[0];
            Assert.Equal("/test1", test1.Path);
            Assert.Equal(3, test1.Children.Count);
        }
    }
}
