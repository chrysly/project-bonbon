using UnityEngine;
using UnityEngine.UI;

namespace BattleUI {
    public class BonbonCraftHandler : BonbonDerivativeHandler {

        [SerializeField] private Transform ingredientView;
        [SerializeField] private GameObject craftButtonPrefab;

        public event System.Action OnButtonArrange;
        public event System.Action<BonbonFXInfo> OnBonbonModification;

        public override UIInputPack InputArrangement() {
            int craftCount = CurrActor.BonbonList.Count;
            BonbonCraftButton[] buttonArr = new BonbonCraftButton[craftCount];
            for (int i = 0; i < craftCount; i++) {
                BonbonBlueprint recipe = CurrActor.BonbonList[i];
                GameObject go = Instantiate(craftButtonPrefab,
                    transform.GetComponentInChildren<HorizontalLayoutGroup>().transform);
                go.name = $"Craftable: {recipe}";
                buttonArr[i] = go.GetComponent<BonbonCraftButton>();
                buttonArr[i].Init(this, recipe);
            } OnButtonArrange?.Invoke(); 
            return new UIInputPack(new[] { buttonArr }, TraversalMode.Vertical);
        }

        public void CraftBonbon(BonbonBlueprint bonbon) {
            BonbonObject freshBonbon = BonbonHandler.CreateBonbon(bonbon, CurrActor, new bool[4]);
            CurrActor.AcceptBonbon(Slot, freshBonbon);
            OnBonbonModification?.Invoke(new BonbonCraftInfo(Slot, freshBonbon));
            Brain.ReturnTo<BonbonMainHandler>();
        }
    }
}