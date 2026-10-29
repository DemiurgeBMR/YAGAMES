using BaggageRush.Core;
using UnityEngine;

namespace BaggageRush.Monetization
{
    /// <summary>Что получает игрок за каждый товар. Вызывается и при покупке, и при восстановлении незавершённой.</summary>
    public static class PurchaseRewards
    {
        public static void Grant(string productId)
        {
            var data = Services.Save.Data;
            switch (productId)
            {
                case ProductIds.NoAds:
                    data.noAds = true;
                    Services.Ads.RefreshBanner();
                    break;
                case ProductIds.StarterPack:
                    data.coins += 2000;
                    data.AddBooster(BoosterIds.Magnet, 3);
                    data.AddBooster(BoosterIds.Freeze, 3);
                    break;
                case ProductIds.CoinsSmall:
                    data.coins += 2500;
                    break;
                case ProductIds.CoinsBig:
                    data.coins += 8000;
                    break;
                case ProductIds.BoostersPack:
                    foreach (var id in BoosterIds.All)
                        data.AddBooster(id, 5);
                    break;
                default:
                    Debug.LogWarning($"Неизвестный товар: {productId}");
                    return;
            }
            Services.Save.MarkDirty();
            // Покупку нельзя потерять: пишем сразу, не дожидаясь интервала.
            Services.Save.Flush();
        }
    }
}
