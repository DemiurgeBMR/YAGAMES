# Багаж-мастер (Baggage Rush) — заметки для Claude

Unity 6 2D-игра про сортировку багажа. Первая площадка — Яндекс Игры (WebGL), потом RuStore и Google Play. Дизайн, монетизация и план этапов — `docs/PLAN.md` (отмечайте выполненные этапы).

## Общение
- С пользователем — по-русски. Комментарии в коде и сообщения в логах — тоже по-русски.
- Пользователь и Claude — единственные разработчики. Пользователь тестирует в Unity и присылает ошибки/скриншоты.

## Ограничения облачной сессии
- Unity в контейнере нет. После изменений C# запускайте `tools/compile-check/check.sh`
  (нужен .NET SDK: `apt-get install -y dotnet-sdk-8.0`). Скрипт компилирует `Assets/_Game` против
  эталонных сборок Unity 2021.x из NuGet. Ожидаемые ложные ошибки — только про API новее 2021.1
  (`NamedBuildTarget`-перегрузки `PlayerSettings`, `SetIl2CppCodeGeneration`, `Get/SetScriptingDefineSymbols`).
- Пакеты uGUI/TextMeshPro с packages.unity.com из контейнера недоступны — код на них проверяется только у пользователя.
- После добавления файлов в `Assets/` запускайте `python3 tools/gen_meta.py`, чтобы .meta с GUID попали в git.

## Соглашения
- Пространства имён `BaggageRush.*` (`Core`, `Platform`, `Monetization`, `DebugTools`, `EditorTools`).
  Не называйте пространства `Debug`/`Editor` — конфликт с `UnityEngine.Debug`/`UnityEditor.Editor`.
- Код площадок — только в `Scripts/Platform/` за интерфейсами; игровой код обращается к `Services.*`.
- Сцены и префабы собираются editor-скриптами (меню **YAGAMES**), а не ручным YAML.
- Статика сбрасывается через `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]` (Play Mode без перезагрузки домена).
- Сохранения — `SaveData` + `JsonUtility`: только поля и списки, при смене формата повышать `CurrentVersion`.
- Арт: 1080×1920, 100 px/юнит, камера ortho size 9.6. Исходники графики — SVG в `docs/mockups/src/mockups.html`,
  рендер в PNG через Playwright (`node docs/mockups/src/render.js`).
