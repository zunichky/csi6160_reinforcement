using UnityEditor;
class Builder {
    static void linuxBuild() {

        // Place all your scenes here
        string[] scenes = {"Assets/scenes/AI.unity"};

        string pathToDeploy = "builds/linux_AI/build.X86_64";       

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneLinux64, BuildOptions.None);      
    }
}