using BaggageRush.Core;
using BaggageRush.Monetization;
using UnityEngine;

namespace BaggageRush.DebugTools
{
    /// <summary>
    /// Временная панель этапа 0 на IMGUI: проверяет сервисы, сохранения и рекламу без игровой сцены.
    /// Уберём, когда появится настоящий интерфейс.
    /// </summary>
    public sealed class DebugPanel : MonoBehaviour
    {
        const float RefHeight = 960f;
        const float Width = 500f;

        [SerializeField] Font font;

        GUIStyle label, title, button;
        string lastEvent = "—";

        void OnGUI()
        {
            if (!Services.IsReady) return;
            EnsureStyles();

            var scale = Screen.height / RefHeight;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            var virtualWidth = Screen.width / scale;
            var data = Services.Save.Data;

            GUILayout.BeginArea(new Rect((virtualWidth - Width) / 2f, 40f, Width, RefHeight - 80f));
            GUILayout.Label("Багаж-мастер · этап 0", title);
            GUILayout.Label($"Площадка: {Services.Platform.Name} · язык: {Services.Platform.Language} · {Services.Platform.Device}", label);
            GUILayout.Label($"Монеты: {data.coins}   Уровень: {data.level}   Пассажиры: {data.passengers}", label);
            GUILayout.Label($"Магнитов: {data.GetBooster(BoosterIds.Magnet)}   Без рекламы: {(data.noAds ? "да" : "нет")}", label);
            GUILayout.Label($"Пауза: {(GamePause.IsPaused ? "да" : "нет")}   Межстраничная доступна: {(Services.Ads.CanShowInterstitial ? "да" : "нет")}", label);
            GUILayout.Space(16f);

            if (GUILayout.Button("Пройти уровень (+50, межстраничная)", button))
            {
                data.level++;
                data.coins += 50;
                data.passengers += 200;
                Services.Save.MarkDirty();
                Services.Leaderboards.SubmitScore("passengers", data.passengers);
                var shown = Services.Ads.TryShowInterstitial(null);
                lastEvent = shown ? "Межстраничная показана" : "Межстраничная пропущена правилами";
            }
            if (GUILayout.Button("Реклама за награду: +100 монет", button))
            {
                Services.Ads.ShowRewarded(() =>
                {
                    Services.Save.Data.coins += 100;
                    Services.Save.MarkDirty();
                    lastEvent = "Награда выдана";
                });
            }
            if (GUILayout.Button("Купить «Отключить рекламу»", button))
            {
                Services.Purchases.Purchase(ProductIds.NoAds, ok =>
                {
                    if (ok) PurchaseRewards.Grant(ProductIds.NoAds);
                    lastEvent = ok ? "Покупка выдана" : "Покупка отменена";
                });
            }
            if (GUILayout.Button("Сбросить прогресс", button))
            {
                Services.Save.ResetProgress();
                Services.Ads.RefreshBanner();
                lastEvent = "Прогресс сброшен";
            }

            GUILayout.Space(16f);
            GUILayout.Label($"Последнее событие: {lastEvent}", label);
            GUILayout.Label("Перезапустите Play Mode — прогресс должен сохраниться.", label);
            GUILayout.EndArea();
        }

        void EnsureStyles()
        {
            if (label != null) return;
            label = new GUIStyle(GUI.skin.label) { font = font, fontSize = 18, wordWrap = true };
            label.normal.textColor = new Color32(0x23, 0x32, 0x4a, 0xff);
            title = new GUIStyle(label) { fontSize = 28, fontStyle = FontStyle.Bold };
            button = new GUIStyle(GUI.skin.button) { font = font, fontSize = 20, fixedHeight = 56f };
        }
    }
}
