#!/usr/bin/env bash
# Проверка компиляции скриптов без Unity. Нужен .NET SDK 8 (apt-get install dotnet-sdk-8.0).
set -euo pipefail
cd "$(dirname "$0")"

if [ ! -d refs/engine ]; then
  mkdir -p refs/tmp refs/engine refs/editor
  curl -sSfL -o refs/tmp/engine.nupkg https://api.nuget.org/v3-flatcontainer/unityengine.modules/2021.3.33/unityengine.modules.2021.3.33.nupkg
  curl -sSfL -o refs/tmp/editor.nupkg https://api.nuget.org/v3-flatcontainer/unity3d.sdk/2021.1.14.1/unity3d.sdk.2021.1.14.1.nupkg
  (cd refs/tmp && unzip -qo engine.nupkg -d engine && unzip -qo editor.nupkg -d editor)
  cp refs/tmp/engine/lib/netstandard2.0/*.dll refs/engine/
  cp refs/tmp/editor/lib/UnityEditor.dll refs/editor/
  rm -rf refs/tmp
fi

for mode in Player Editor; do
  echo "=== $mode ==="
  dotnet build CompileCheck.csproj -nologo -v q -clp:NoSummary -p:Mode=$mode 2>&1 | grep -E "error|warning CS" | sed 's|.*/Assets/|Assets/|' | sort -u || true
done
echo "Готово."
