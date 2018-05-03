using DocGen.Requirements;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal
{
    public static class UrlHelperExtensions
    {
        public static string UserNeed(this IUrlHelper urlHelper, UserNeed userNeed)
        {
            var url = $"/requirements/{userNeed.NumberFullyQualified}";
            if (urlHelper == null) return url;
            return urlHelper.Content($"~{url}");
        }
        
        public static string ProductRequirement(this IUrlHelper urlHelper, ProductRequirement productRequirement)
        {
            var url = $"/requirements/{productRequirement.NumberFullyQualified}";
            if (urlHelper == null) return url;
            return urlHelper.Content($"~{url}");
        }
        
        public static string SoftwareSpecification(this IUrlHelper urlHelper, SoftwareSpecification softwareSpecification)
        {
            var url = $"/requirements/{softwareSpecification.NumberFullyQualified}";
            if (urlHelper == null) return url;
            return urlHelper.Content($"~{url}");
        }
        
        public static string TestCase(this IUrlHelper urlHelper, TestCase testCase)
        {
            var url = $"/requirements/{testCase.NumberFullyQualified}";
            if (urlHelper == null) return url;
            return urlHelper.Content($"~{url}");
        }
    }
}