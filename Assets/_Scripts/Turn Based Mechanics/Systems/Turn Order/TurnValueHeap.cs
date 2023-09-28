using System.Collections.Generic;

public class TurnValueHeap : MinHeap<TurnValueHandler> {
    public TurnValueHeap() : base() {}
    public TurnValueHeap(List<Actor> actors) : this() {
        CreateTurnValueList(actors);
        BuildHeap();
    }

    private void CreateTurnValueList(List<Actor> actors) {
        foreach(Actor actor in actors) {
            if (_size + 1 == _backingArray.Length) {
                TurnValueHandler[] resizedArray = new TurnValueHandler[_backingArray.Length * 2];
                for(int i = 1; i <= _size; i++) {
                    resizedArray[i] = _backingArray[i];
                }
                _backingArray = resizedArray;
            }
            _backingArray[_size + 1] = new TurnValueHandler(actor);
            _size++;
        }
    }
    public void FlatModify(Actor actor, int modifyTurnValue) {
        for(int i = 1; i <= _size; i++) {
            TurnValueHandler tvh = _backingArray[i];
            if (tvh.Actor.Equals(actor)) {
                tvh.ActionMeter -= modifyTurnValue * tvh.Speed;
                break;
            }
        }
    }
    public void FlatModifyAll(int modifyTurnValue) {
        for(int i = 1; i <= _size; i++) {
            TurnValueHandler tvh = _backingArray[i];
            tvh.ActionMeter -= modifyTurnValue * tvh.Speed;
        }
    }
}