// Заглушка для эталонного UnityEditor 2021.1, где ещё нет NamedBuildTarget (появился в 2021.2).
// Вызовы PlayerSettings с NamedBuildTarget дадут здесь CS1503/CS1501 — это ожидаемо, в Unity 6 они корректны.
namespace UnityEditor.Build
{
    public readonly struct NamedBuildTarget
    {
        public static readonly NamedBuildTarget WebGL;
        public static readonly NamedBuildTarget Android;
        public static readonly NamedBuildTarget Standalone;
    }
}
