//declare var g: NodeJS.Global & typeof globalThis;

declare global {
    // import { GlobalConfig } from 'Table/DemoData';
    // export var config: GlobalConfig;
    interface String {
        prependHello(): string;
        
        toRed(...args:any[]): string;
        
        toYellow(...args:any[]): string;
        
        toBlue(...args:any[]): string;
        
        toGreen(...args:any[]): string;
    }
}

export {}