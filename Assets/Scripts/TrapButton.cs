using UnityEngine;

public class TrapButton : MonoBehaviour
{
    public TrapMachine connectedMachine;
    public void PressButton()
    {
        if(connectedMachine != null)
        {
            connectedMachine.ToggleMachine();
        }
    }
}