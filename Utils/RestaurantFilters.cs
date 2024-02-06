using System.Text.RegularExpressions;

namespace restaurant_api.Utils
{
    public static class RestaurantFilters
    {
        public static bool HasSameCategory(string categoryId, string? compId) => String.IsNullOrEmpty(compId) ? true : categoryId == compId;
        public static bool HasSearchString(string name, string? searchString)
        {
            if (searchString == null) return true;
            var regex = new Regex($"{searchString}", RegexOptions.IgnoreCase);
            return regex.IsMatch(name);
        }
    }
}
