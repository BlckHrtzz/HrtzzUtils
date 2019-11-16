using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hrtzz.BuildTools
{
    public class BuildProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild (BuildReport buildReport)
        {
            Debug.Log ("<color=green><b>Build Post Processing Started</b></color>");
            Debug.Log ("<color=green><b>Build Report : " + buildReport.summary.result + "</b></color>");

            BuildSummary buildSummary = buildReport.summary;

#if UNITY_IOS
            if (buildSummary.platform == BuildTarget.iOS)
            {
                HrtzzBuildPipeline.CopyAllContentFromSource (buildReport.summary.outputPath);
                //SetupXCodeProject(buildReport.summary.outputPath);
                //InstallCocoaPods(buildReport.summary.outputPath);
            }
#endif

            if (buildSummary.platform == BuildTarget.Android)
            {
                FileBrowserUtility.Open (buildSummary.outputPath);
            }

            //  Updates the build version after successful build.
            BuildPipeline.WriteBuildSettingToJson (BuildPipeline.UpdateBuildVersion ());
        }
    }
}
