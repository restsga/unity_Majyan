static public class Random{

    //seeds
    static private uint x = 123456789, y = 362436069, z = 521288629, w = 88675123;

    static public bool SetSeed(int seed)
    {
        if (seed <= 0)
        {
            return false;
        }
        w = (uint)seed;
        return true;
    }

    static public uint Xor128()
    {
        uint t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
        return w;
    }

    static public int Xor128_next(int maxvalue)
    {
        return (int)(Xor128() % maxvalue);
    }

}
