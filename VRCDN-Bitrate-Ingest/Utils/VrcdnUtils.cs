namespace LucHeart.VRCDN.BI.Utils;

public static class VrcdnUtils
{
    public static string ConvertKeyToVrcdnAuthThing(int key, string input)
    {
        Span<byte> xorBytes = stackalloc byte[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            xorBytes[i] = (byte)(input[i] ^ key);
        }
        return Convert.ToBase64String(xorBytes);
    }
}