using UnityEngine;

public class LevelResources : MonoBehaviour
{
    public GameResources RedResources;
    public GameResources BlueResources;
    public GameResources YellowResources;
    public GameResources BlackResources;
    
    public GameResources GetResource(string id)
    {
        switch (id)
        {
            case "red":
                return RedResources;
            case "blue":
                return BlueResources;
            case "yellow":
                return YellowResources;
            case "black":
                return BlackResources;
            default: return null;
        }
    }
}
