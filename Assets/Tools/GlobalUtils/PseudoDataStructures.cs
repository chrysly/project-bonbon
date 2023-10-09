using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// A bunch of handwritten versions of non-serializable types using serializable types;
/// <br></br> Finally, after all these years... The LeetCode grind proved to be worth it *-*;
/// <br></br> PS: Namespace declarations don't support XML documentation, sadge T-T
/// </summary>
namespace PseudoDataStructures {

    /// <summary>
    /// A dystopian map of indeces to a single array of strings to emulate several arrays... Yay ;-;
    /// </summary>
    [Serializable]
    public class ArrayArray<T> {

        public int[] startIndeces;
        public T[] secondaryElements;

        public T[] this[int index] {
            get => ToArray()[index];
            set => ToArray()[index] = value;
        }

        /*
        public static T[] SubArray(this T[] data, int startIndex, int nextIndex) {
            T[] result = new T[nextIndex - startIndex];
            Array.Copy(data, startIndex, result, 0, nextIndex - startIndex);
            return result;
        }*/

        public ArrayArray() {
            startIndeces = new int[0];
            secondaryElements = new T[0];
        }

        public ArrayArray(T[][] array) {
            startIndeces = new int[array.Length];
            secondaryElements = new T[0];
            var i = 0;
            foreach (T[] subArray in array) {
                secondaryElements = secondaryElements.Concat(subArray).ToArray();
                startIndeces[i] = secondaryElements.Length - subArray.Length;
                i++;
            }
        }

        public ArrayArray(List<T>[] array) {
            startIndeces = new int[array.Length];
            secondaryElements = new T[0];
            var i = 0;
            foreach (List<T> subArray in array) {
                if (subArray != null) {
                    secondaryElements = secondaryElements.Concat(subArray.ToArray()).ToArray();
                    startIndeces[i] = secondaryElements.Length - subArray.Count;
                } else startIndeces[i] = secondaryElements.Length;
                i++;
            }
        }

        public T[][] ToArray() {
            var secondaryEList = new List<T>(secondaryElements);
            T[][] array = new T[startIndeces.Length][];
            if (startIndeces.Length > 0) {
                for (int i = 0; i < startIndeces.Length - 1; i++) {
                    array[i] = secondaryEList.GetRange(startIndeces[i], startIndeces[i + 1] - startIndeces[i]).ToArray();
                }
                var lastIndex = startIndeces[startIndeces.Length - 1];
                array[array.Length - 1] = secondaryEList.GetRange(lastIndex, secondaryEList.Count - lastIndex).ToArray();
            }
            return array;
        }

        public List<T>[] ToListArray() {
            var secondaryEList = new List<T>(secondaryElements);
            List<T>[] array = new List<T>[startIndeces.Length];
            if (startIndeces.Length > 0) {
                for (int i = 0; i < startIndeces.Length - 1; i++) {
                    array[i] = secondaryEList.GetRange(startIndeces[i], startIndeces[i + 1] - startIndeces[i]);
                }
                var lastIndex = startIndeces[startIndeces.Length - 1];
                array[array.Length - 1] = secondaryEList.GetRange(lastIndex, secondaryEList.Count - lastIndex);
            }
            return array;
        }
    }

    /// <summary>
    /// A basic map of strings to strings... Yay ;-;
    /// </summary>
    [Serializable]
    public class StringStringDictionary {

        public string[] Keys;
        public string[] Values;

        public StringStringDictionary(Dictionary<string, string> dict) {

            Keys = new string[dict.Count];
            Values = new string[dict.Count];

            var i = 0;
            foreach (KeyValuePair<string, string> kvp in dict) {
                Keys[i] = kvp.Key;
                Values[i] = kvp.Value;
                i++;
            }
        }

        public StringStringDictionary() {
            Keys = new string[0];
            Values = new string[0];
        }

        public Dictionary<string, string> ToDictionary() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < Keys.Length; i++) {
                dict[Keys[i]] = Values[i]; 
            } return dict;;
        }
    }

    /// <summary>
    /// A more elaborate map of strings to lists of strings... Yay ;-;
    /// </summary>
    [Serializable]
    public class StringStringListDictionary {

        public string[] Keys;
        public StringArrayArray Values;

        public StringStringListDictionary(Dictionary<string, List<string>> dict) {

            Keys = new string[dict.Count];
            string[][] tempArr = new string[dict.Count][];

            var i = 0;
            foreach (KeyValuePair<string, List<string>> kvp in dict) {
                Keys[i] = kvp.Key;
                tempArr[i] = kvp.Value.ToArray();
                i++;
            } Values = new StringArrayArray(tempArr);
        }

        public StringStringListDictionary() {
            Keys = new string[0];
            Values = new StringArrayArray();
        }

        public Dictionary<string, List<string>> ToDictionary() {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            string[][] parsedValues = Values.ToArray();
            for (int i = 0; i < Keys.Length; i++) {
                dict[Keys[i]] = new List<string>(parsedValues[i]);
            } return dict;
        }
    }

    /// <summary>
    /// A dystopian map of indeces to a single array of strings to emulate several arrays... Yay ;-;
    /// </summary>
    [Serializable]
    public class StringArrayArray {

        public int[] startIndeces;
        public string[] strings;

        public StringArrayArray(string[][] array) {
            startIndeces = new int[array.Length];
            strings = new string[0];
            var i = 0;
            foreach (string[] subArray in array) {
                strings = strings.Concat(subArray).ToArray();
                startIndeces[i] = strings.Length - subArray.Length;
                i++;
            }
        }

        public StringArrayArray() {
            startIndeces = new int[0];
            strings = new string[0];
        }

        public string[][] ToArray() {
            var stringList = new List<string>(strings);
            string[][] array = new string[startIndeces.Length][];
            if (startIndeces.Length > 0) {
                for (int i = 0; i < startIndeces.Length - 1; i++) {
                    array[i] = stringList.GetRange(startIndeces[i], startIndeces[i + 1] - startIndeces[i]).ToArray();
                }
                var lastIndex = startIndeces[startIndeces.Length - 1];
                array[array.Length - 1] = stringList.GetRange(lastIndex, stringList.Count - lastIndex).ToArray();
            } return array;
        }
    } /// An empty space, with a single bracket protecting this 3 AM script against an infinite, dark void... Yay ;-;
}