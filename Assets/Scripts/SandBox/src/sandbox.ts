import { UnityEngine } from 'csharp';
import Vector3 = UnityEngine.Vector3;

global.abc = function() {
    
}

export const FindRun = function(fn: string) {
    return global[ fn ];
}