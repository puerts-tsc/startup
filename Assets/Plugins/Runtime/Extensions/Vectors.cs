using UnityEngine;

namespace Runtime {

public static class Vectors {

    public static Vector3 SetX(this Vector3 target, float value) => new Vector3(value, target.y, target.z);

    public static Vector3 SetY(this Vector3 target, float value) => new Vector3(target.x, value, target.z);

}

}