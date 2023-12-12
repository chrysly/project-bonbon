using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BattleUI {
    public partial class BonbonSlotAnimator {

        [SerializeField] private GameObject vfxPrefab;
        [SerializeField] private GameObject craftVfxPrefab;
        [SerializeField] private Material defaultMat;
        [SerializeField] private Material bonbonAnimMat;

        /// Enumerator Requirements:
        /// - Must set the animator state to 'Idle' at the end;
        /// - Must update the icon at some point by calling the UpdateIcon() method;
        /// Recommendations & Notes:
        /// - The next state will lerp the scale towards 1 or 1.2 depending on input;
        /// - The animation should last between 0.2 and 2 seconds;
        /// - The Logic won't be affected by this animation, this means:
        ///     - The button may exit the coroutine at any moment;
        ///     - The button may be scaled down before the animation is finished;
        /// 
        /// Any local parameters processed in the Coroutine should be reset *conditionally* in ResolveAnimations();
        /// This applies to both Crafting and Baking Animations;
        /// Animations encompassing several slots are more complex and require a stronger framework;
        /// 
        /// You may use any [SerializeField] at the top of this script to reference VFX assets and such;
        /// Feel free to delete this long-a$$ message at any time;

        /// BonbonCraftInfo contains the following variable:
        /// bonbon: The BonbonObject to be crafted (from which you can access the icon);

        private IEnumerator CraftAnimation(BonbonCraftInfo info) {
            GameObject vfxInstance = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Material[] oldMats = { new Material(defaultMat) };
            Material[] dissolveMat = { new Material(bonbonAnimMat) };
            GetComponent<SpriteRenderer>().materials = dissolveMat;
            transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0f);
            transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
            dissolveMat[0].SetFloat("_Dissolve", 1f);
            dissolveMat[0].DOFloat(0f, "_Dissolve", 1.4f);
            yield return new WaitForSeconds(1.1f);
            GetComponent<SpriteRenderer>().materials = oldMats;
            Destroy(vfxInstance, 3f);
            yield return null;
            state = UIAnimatorState.Idle;
        }


        /// BonbonBakeInfo contains the following variables:
        /// ingredients: An array of BonbonObjects featuring the two ingredients that were baked;
        /// result: The BonbonObject that will be placed in the slot after combining the ingredients;

        private IEnumerator BakeAnimation(BonbonBakeInfo info) {
            GameObject vfxInstance = Instantiate(craftVfxPrefab, transform.position, Quaternion.identity);
            Material[] oldMats = { new Material(defaultMat) };
            Material[] dissolveMat = { new Material(bonbonAnimMat) };
            GetComponent<SpriteRenderer>().materials = dissolveMat;
            transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0f);
            transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBounce);
            dissolveMat[0].SetFloat("_Dissolve", 1f);
            dissolveMat[0].DOFloat(0f, "_Dissolve", 2f);
            yield return new WaitForSeconds(1.5f);
            Destroy(vfxInstance, 3f);
            GetComponent<SpriteRenderer>().materials = oldMats;
            yield return null;
            state = UIAnimatorState.Idle;
        }

        private void ResolveAnimations() {
            if (specialAnimation != null) {
                StopCoroutine(specialAnimation);
                /// Reset local parameters;
            }
        }
    }
}
