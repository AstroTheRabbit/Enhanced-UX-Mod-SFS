using HarmonyLib;

namespace EnhancedUX
{
    public static class Extensions
    {
        public static ref F FieldRef<F>(this object obj, string field)
        {
            return ref AccessTools.FieldRefAccess<F>(obj.GetType(), field).Invoke(obj);
        }
    }
}