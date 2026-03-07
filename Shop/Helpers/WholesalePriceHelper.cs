using Shop.Models;

namespace Shop.Helpers
{
    public static class WholesalePriceHelper
    {
        public static decimal GetDiscountedPrice(decimal originalPrice, int quantity, ICollection<WholesaleTier> tiers)
        {
            if (tiers == null || !tiers.Any())
                return originalPrice;

            var applicableTier = tiers
                .Where(t => quantity >= t.MinQuantity)
                .OrderByDescending(t => t.MinQuantity)
                .FirstOrDefault();

            if (applicableTier == null)
                return originalPrice;

            return Math.Round(originalPrice * (1 - applicableTier.DiscountPercent / 100m), 2);
        }

        public static int GetDiscountPercent(int quantity, ICollection<WholesaleTier> tiers)
        {
            if (tiers == null || !tiers.Any()) return 0;

            return tiers
                .Where(t => quantity >= t.MinQuantity)
                .OrderByDescending(t => t.MinQuantity)
                .FirstOrDefault()?.DiscountPercent ?? 0;
        }
    }
}