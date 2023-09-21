using UnityEngine;

namespace ModelAssetDatabase {

    /// <summary>
    /// An 'ext' (external, extension, extended, etc.) data pack to complement a Model loaded by the Asset Library;
    /// </summary>
    public class ExtData : ScriptableObject {
        /// <summary> Version number of the Model Data asset, to check for deprecated files; </summary>
        public int version;
        /// <summary> GUID of the model associated with this data file; </summary>
        public string guid;
        /// <summary> Whether the data underwent manual reimport at least once; </summary>
        public bool isReimported;
        /// <summary> Whether the file was imported as a Model or as an Animation-only file; </summary>
        public bool isModel;
        /// <summary> Personalized user notes on the file; </summary>
        public string notes;
    }
}