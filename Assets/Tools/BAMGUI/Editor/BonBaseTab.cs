using UnityEngine;

/// <summary>
/// Because the structure I devised for the MADGUI has been quite comfortable to work with, I'm building 
/// this tool using a similar structure. This tool has different needs and less complexity than the MAD,
/// thus I'm creating a new tool altogether rather than inheriting from the former's abstract base;
/// </summary>
namespace BonbonAssetManager {
    public abstract class BonBaseTab : ScriptableObject {

        protected BonBaseTool Tool;

        protected const string INVALID_MANAGER = "You attempted to create a new tab without a proper manager to handle it! The tab was not instantiated;";

        public static T CreateTab<T>(BonBaseTool tool) where T : BonBaseTab {
            var tab = CreateInstance<T>();
            tab.Tool = tool;
            tab.InitializeTab();
            return tab;
        }

        protected abstract void InitializeTab();

        public abstract void ShowGUI();
    }

    public abstract class ActorTab : BonBaseTab {

        protected ActorManager ActorManager;

        protected override void InitializeTab() {
            if (Tool is BonbonManager) {
                ActorManager = Tool as ActorManager;
            } else Debug.LogError(INVALID_MANAGER);
        }
    }
}