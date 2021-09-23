//  Project : UNITY FOLDOUT
// Contacts : Pix - ask@pixeye.games

using UnityEngine;

namespace Runtime {

public class ReferenceAttribute : PropertyAttribute {

    public string PropName;

    public ReferenceAttribute(string propName)
    {
        PropName = propName;
    }

}

}