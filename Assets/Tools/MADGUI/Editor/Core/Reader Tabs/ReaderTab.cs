using UnityEngine;

namespace ModelAssetDatabase {

    /// <summary>
    /// Base class for all tabs managed by the Reader Tool;
    /// </summary>
    public abstract class ReaderTab : BaseTab {

        /// <summary> The Reader parent tool of this tab; </summary>
        protected Reader Reader;

        protected override void InitializeData() {
            if (Tool is Reader) {
                Reader = Tool as Reader;
            } else Debug.LogError(INVALID_MANAGER);
        }
    }
}