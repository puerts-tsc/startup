// type Constructor<T, Args extends any[] = any[]> = new (...args: Args) => T;
//
//
//
// interface ColumnConstructor<T> {
//    
//     readonly _name: string;
//     readonly comment: string;
//     readonly dataType: Constructor<T>;
//     test: SearchFunc;
//    
//     new(value: T): Column<T>;
// }
//
// interface Column<T> {
//     readonly value: T;
// }
//
// export function Column<T>(dataType: Constructor<T>): ColumnConstructor<T> {
//    
//     return class Column {
//         static readonly _name: string;
//         static readonly comment: string;
//         static readonly dataType = dataType;
//        
//         test: SearchFunc = function(source: T) {
//            
//         }
//        
//         constructor(readonly value: T) {
//         }
//     };
// }
//
// class Big {
//     constructor(value: number) {
//     }
// }
//
// class Bigint {
//     constructor(readonly value: Big = new Big(0)) {
//     }
// }
//
// export class Amount extends Column(Bigint) {
//     static readonly _name: string = '"amt"';
//    
//     constructor() {
//         super(new Bigint(0));
//     }
// }
//
// export class Quantity extends Column(Bigint) {
//     static readonly _name: string = '"qty"';
//    
//     constructor() {
//         super(new Bigint(0));
//         super.test(this);
//     }
// }
//
// // function Resource<T>() {
// //     abstract class Resource {
// //         /* static methods */
// //         public static list: T[] = [];
// //        
// //         public static async fetch(): Promise<T[]> {
// //             return null!;
// //         }
// //        
// //         /*  instance methods */
// //         public save(): Promise<T> {
// //             return null!
// //         }
// //     }
// //    
// //     return Resource;
// // }
//
//# sourceMappingURL=Resource.js.map