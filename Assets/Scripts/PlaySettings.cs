
public static class PlaySettings
{
    public static float clickCritRate = 1f;
    public static float clickDamageMulti = 1f;
    public static int passiveIncomingMulti = 1;
    public static float[] _EntitySpawnDelay = new float[5] {10,60,360,2160,12960};

    public static void PowerUP(int index)
    {
        switch (index)
        {
            case 2:
                clickCritRate += .5f;
                break;
            case 1:
                clickDamageMulti += 1f;
                break;
            case 0:
                passiveIncomingMulti += 1;
                break;
        }
    }
}
