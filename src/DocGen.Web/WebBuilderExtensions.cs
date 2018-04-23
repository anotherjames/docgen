using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DocGen.Web
{
    public static class WebBuilderExtensions
    {
        public static void RegisterMvc(this IWebBuilder webBuilder, string path, object routeData)
        {
            webBuilder.Register(path, async context => {
                var actionSelector = context.RequestServices.GetRequiredService<IActionSelector>();
                var actionInvokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();

                var routeContext = new RouteContext(context);
                if (routeData != null) {
                    foreach(var value in new RouteValueDictionary(routeData)) {
                        routeContext.RouteData.Values[value.Key] = value.Value;
                    }
                }
                var candidates = actionSelector.SelectCandidates(routeContext);
                if (candidates == null || candidates.Count == 0) {
                    throw new Exception("No actions matched");
                }

                var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);
                if (actionDescriptor == null) {
                    throw new Exception("No actions matched");
                }

                var actionContext = new ActionContext(context, routeContext.RouteData, actionDescriptor);

                var invoker = actionInvokerFactory.CreateInvoker(actionContext);
                if (invoker == null) {
                    throw new InvalidOperationException("Couldn't create invoker");
                }

                await invoker.InvokeAsync();
            });
        }

        public class TestMvcRouteHandler : IRouter
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IActionInvokerFactory _actionInvokerFactory;
        private readonly IActionSelector _actionSelector;
        private readonly ILogger _logger;

        public TestMvcRouteHandler(
            IActionInvokerFactory actionInvokerFactory,
            IActionSelector actionSelector,
            ILoggerFactory loggerFactory)
            : this(actionInvokerFactory, actionSelector, loggerFactory, actionContextAccessor: null)
        {
        }

        public TestMvcRouteHandler(
            IActionInvokerFactory actionInvokerFactory,
            IActionSelector actionSelector,
            ILoggerFactory loggerFactory,
            IActionContextAccessor actionContextAccessor)
        {
            // The IActionContextAccessor is optional. We want to avoid the overhead of using CallContext
            // if possible.
            _actionContextAccessor = actionContextAccessor;

            _actionInvokerFactory = actionInvokerFactory;
            _actionSelector = actionSelector;
            _logger = loggerFactory.CreateLogger<MvcRouteHandler>();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // We return null here because we're not responsible for generating the url, the route is.
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var candidates = _actionSelector.SelectCandidates(context);
            if (candidates == null || candidates.Count == 0)
            {
                //_logger.NoActionsMatched(context.RouteData.Values);
                return Task.CompletedTask;
            }

            var actionDescriptor = _actionSelector.SelectBestCandidate(context, candidates);
            if (actionDescriptor == null)
            {
                //_logger.NoActionsMatched(context.RouteData.Values);
                return Task.CompletedTask;
            }

            context.Handler = (c) =>
            {
                var routeData = c.GetRouteData();
                routeData = new RouteData();
                var actionContext = new ActionContext(context.HttpContext, routeData, actionDescriptor);
                if (_actionContextAccessor != null)
                {
                    _actionContextAccessor.ActionContext = actionContext;
                }

                var invoker = _actionInvokerFactory.CreateInvoker(actionContext);
                if (invoker == null)
                {
                    throw new InvalidOperationException("Couldn't create invoker.");
                }

                return invoker.InvokeAsync();
            };

            return Task.CompletedTask;
        }
    }
    }
}