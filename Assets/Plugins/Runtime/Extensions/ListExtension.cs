using System.Collections.Generic;

namespace Runtime.Extensions {

public static class ListExtension {

    public static void Add<T>(this List<string> list)
    {
        list.Add(typeof(T).Name);
    }

}

}