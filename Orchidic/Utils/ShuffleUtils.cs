namespace Orchidic.Utils;

public static class ShuffleUtils
{
    public static void GenerateRandomCycle(int n, out int[] next, out int[] prev)
    {
        var rnd = new Random();
        var p = Enumerable.Range(0, n).ToArray();

        // 原地洗牌
        for (int i = n - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (p[i], p[j]) = (p[j], p[i]);
        }

        next = new int[n];
        prev = new int[n];

        // 构造单环 + 逆向映射
        for (int i = 0; i < n; i++)
        {
            int u = p[i];
            int v = p[(i + 1) % n];

            next[u] = v; // 前向
            prev[v] = u; // 逆向
        }
    }
}