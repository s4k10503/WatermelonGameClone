using System.Linq;
using UnityEditor;

public class CI
{
    [MenuItem("CI/Build")]
    public static void Build()
    {
        BuildPipeline.BuildPlayer(ScenePaths, "Build/Application.exe", BuildTarget.StandaloneWindows64, BuildOptions.CompressWithLz4);
    }

    static string[] ScenePaths => EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
}
