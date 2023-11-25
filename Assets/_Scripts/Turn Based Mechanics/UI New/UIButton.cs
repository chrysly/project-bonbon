namespace BattleUI {
    public class UIButton : UIElement {

        protected UIBrain brain;
        
        public void Init(UIBrain brain) {
            this.brain = brain;
        }

        public bool Available => IsAvailable();

        public virtual void Activate() {
            /// Broadcast Use Event;
        }

        public virtual bool IsAvailable() => true;
    }

    public class MainStateButton : UIButton { }

    public class Main2SkillButton : MainStateButton {
        
        public override void Activate() {
            base.Activate();
            brain.Transition(UIStateType.Skill); 
        }
    }

    public class Main2BonbonButton : MainStateButton {
        public override void Activate() {
            base.Activate();
            brain.Transition(UIStateType.Bonbon);
        }
    }

    public class BonbonButton : UIButton {

        public BonbonObject Bonbon { get; private set; }

        public override void Activate() {
            if (Bonbon != null) {

            } else {

            }
        }
    }
}