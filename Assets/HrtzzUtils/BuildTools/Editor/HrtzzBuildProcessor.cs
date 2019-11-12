using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hrtzz.BuildTools
{
    // Custom build processor.
    public class BuildProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild (BuildReport buildReport)
        {
            Debug.Log ("<color=green><b>Build Post Processing Started</b></color>");
            Debug.Log ("<color=green><b>Build Report : " + buildReport.summary.result + "</b></color>");

            //if (buildReport.summary.result != BuildResult.Succeeded)
            //{
            //    return;
            //}

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
                HrtzzFileBrowser.Open (buildSummary.outputPath);
            }

            //  Updates the build version after successful build.
            HrtzzBuildPipeline.WriteBuildSettingToJson (HrtzzBuildPipeline.UpdateBuildVersion ());
        }
    }
}
