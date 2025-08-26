
public static class UnityObjectImplementedInterfaceExtensions
{
    public static bool IsValid<T>(this T Interface) where T: class
    {
        if(Interface is UnityEngine.Object obj)
        {
            return obj != null;
        }
        else
        {
            return Interface != null;
        }
    } 
}