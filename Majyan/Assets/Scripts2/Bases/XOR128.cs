static public class XOR128{

    //seeds
    static private uint x = 123456789, y = 362436069, z = 521288629, w = 88675123;
    
    static public void DecideInitial(int n)
    {
        for(int i = 0; i < n; i++)
        {
            Random();
        }
    }

    static public uint Random()
    {
        uint t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
        return w;
    }

    static public int Next(int maxvalue)
    {
        return (int)(Random() % maxvalue);
    }
}
