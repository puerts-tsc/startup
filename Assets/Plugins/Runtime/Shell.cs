﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Runtime
{
    public class Shell
    {
        // 当找不到文件或者拒绝访问时出现的Win32错误码
        const int ERROR_FILE_NOT_FOUND = 2;
        const int ERROR_ACCESS_DENIED = 5;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string Wsl( string cmd, bool noWindow = true ) 
            => Run( "wsl.exe", $"{cmd} 2>&1", noWindow );

        public static string Run( string command, string argument, bool noWindow = true )
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.CreateNoWindow = noWindow;
            start.FileName = command;
            start.Arguments = argument;
            start.ErrorDialog = false;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true; // 由调用程序获取输出信息
            start.RedirectStandardError = true; //重定向标准错误输出
            Process p = Process.Start( start );
            p.WaitForExit();
            StreamReader reader = p.StandardOutput; //获取exe处理之后的输出信息
            var log = reader.ReadToEnd().TrimEnd();
            Debug.Log( log );
            p.Close();
            return log;
        }

        // 通过命令行获取help显示信息
        static List<string> Execute( string args = "" )
        {
            var result = new List<string>();
            Process process = new Process();
            try {
                process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动 
                process.StartInfo.CreateNoWindow = true; //是否在新窗口中启动该进程的值 (不显示程序窗口)
                process.StartInfo.RedirectStandardInput = true; // 接受来自调用程序的输入信息 
                process.StartInfo.RedirectStandardOutput = true; // 由调用程序获取输出信息
                process.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                process.StartInfo.WorkingDirectory = Path.GetFullPath( Application.dataPath + "/.." );
                process.StartInfo.FileName = "wsl.exe";
                process.StartInfo.Arguments = $"{args} 2>&1";
                process.Start(); // 启动程序
                process.WaitForExit(); //等待程序执行完退出进程
                //process.StandardInput.WriteLine( "help" ); //向cmd窗口发送输入信息
                //process.StandardInput.AutoFlush = true;
                // 前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //process.StandardInput.WriteLine( "exit" );
                StreamReader reader = process.StandardOutput; //获取exe处理之后的输出信息
                string curLine = reader.ReadLine(); //获取错误信息到error
                while ( !reader.EndOfStream ) {
                    if ( !string.IsNullOrEmpty( curLine ) ) {
                        UnityEngine.Debug.Log( curLine );
                        if ( process.ExitCode == 0 ) {
                            result.Add( curLine );
                        }
                    }

                    curLine = reader.ReadLine();
                }

                reader.Close(); //close进程
                process.Close();
            }
            catch ( Exception e ) {
                Debug.LogError( e.Message );
//                    if ( e.NativeErrorCode == ERROR_FILE_NOT_FOUND ) {
//                        Console.WriteLine( e.Message + ". 检查文件路径." );
//                    }
//                    else if ( e.NativeErrorCode == ERROR_ACCESS_DENIED ) {
//                        Console.WriteLine( e.Message + ". 你没有权限操作文件." );
//                    }
            }

            return result;
        }

        static void Main()
        {
            //MyProcess myProcess = new MyProcess();
            //myProcess.Execute();
            Shell.Execute( "ls" );
            Console.ReadKey();
        }
    }
}