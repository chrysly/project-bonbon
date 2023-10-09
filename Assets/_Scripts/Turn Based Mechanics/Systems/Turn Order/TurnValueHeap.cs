using System.Collections.Generic;

public class TurnValueHeap : MinHeap<TurnValueHandler> {
    public TurnValueHeap() : base() {}
    public TurnValueHeap(List<Actor> actors) : this() {
        CreateTurnValueList(actors);
        BuildHeap();
    }
    public TurnValueHeap(TurnValueHeap turnValueHeap) {
        _backingArray = (TurnValueHandler[]) turnValueHeap._backingArray.Clone();
        _size = turnValueHeap._size;
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
    public void FlatModifyTurnValue(Actor actor, int modifyTurnValue) {
        int i;
        for(i = 1; i <= _size; i++) {
            TurnValueHandler tvh = _backingArray[i];
            if (tvh.Actor.Equals(actor)) {
                tvh.ActionMeter += modifyTurnValue * tvh.Speed;
                break;
            }
        }
        UpHeap(i);
        DownHeap(i);
    }
    public void FlatModifyTurnValueAll(int modifyTurnValue) {
        for(int i = 1; i <= _size; i++) {
            TurnValueHandler tvh = _backingArray[i];
            tvh.ActionMeter += modifyTurnValue * tvh.Speed;
        }
        BuildHeap();
    }
    public void RemoveActor(Actor actor) {
        for(int i = 1; i <= _size; i++) {
            TurnValueHandler tvh = _backingArray[i];
            if (tvh.Actor.Equals(actor)) {
                _backingArray[i] = _backingArray[_size];
                _backingArray[_size] = null;
                DownHeap(i);
                return;
            }
        }

        throw new KeyNotFoundException("Provided actor for removal from heap does not exist!");
    }
    public void BuildTurnValueHeap() {
        BuildHeap();
    }

    public void ResetTop()
    {
        Peek().ResetActionMeter();
        BuildHeap();
    }
}