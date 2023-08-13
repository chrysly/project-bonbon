public abstract class State<M, S, I> 
    where M : StateMachine<M, S, I> 
    where S : State<M, S, I> 
    where I : StateInput
{
    public M MySM;
    public I Input => MySM.CurrInput;

    public virtual void Enter(I i) { }
    public virtual void Exit(I i) { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}