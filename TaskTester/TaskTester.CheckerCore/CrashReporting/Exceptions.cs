using System.Collections.Generic;

namespace TaskTester.CheckerCore.CrashReporting
{
    public static class Exceptions
    {
        public static readonly IReadOnlyDictionary<string, string> ByCode = new Dictionary<string, string>() {
            {"0", "STATUS_WAIT_0"},
            {"0x80", "STATUS_ABANDONED_WAIT_0"},
            {"0xc0", "STATUS_USER_APC"},
            {"0x102", "STATUS_TIMEOUT"},
            {"0x103", "STATUS_PENDING"},
            {"0x40000005", "STATUS_SEGMENT_NOTIFICATION"},
            {"0x80000001", "STATUS_GUARD_PAGE_VIOLATION"},
            {"0x80000002", "STATUS_DATATYPE_MISALIGNMENT"},
            {"0x80000003", "STATUS_BREAKPOINT"},
            {"0x80000004", "STATUS_SINGLE_STEP"},
            {"0xc0000005", "STATUS_ACCESS_VIOLATION"},
            {"0xc0000006", "STATUS_IN_PAGE_ERROR"},
            {"0xc0000008l", "STATUS_INVALID_HANDLE"},
            {"0xc0000017", "STATUS_NO_MEMORY"},
            {"0xc000001d", "STATUS_ILLEGAL_INSTRUCTION"},
            {"0xc0000025", "STATUS_NONCONTINUABLE_EXCEPTION"},
            {"0xc0000026", "STATUS_INVALID_DISPOSITION"},
            {"0xc000008c", "STATUS_ARRAY_BOUNDS_EXCEEDED"},
            {"0xc000008d", "STATUS_FLOAT_DENORMAL_OPERAND"},
            {"0xc000008e", "STATUS_FLOAT_DIVIDE_BY_ZERO"},
            {"0xc000008f", "STATUS_FLOAT_INEXACT_RESULT"},
            {"0xc0000090", "STATUS_FLOAT_INVALID_OPERATION"},
            {"0xc0000091", "STATUS_FLOAT_OVERFLOW"},
            {"0xc0000092", "STATUS_FLOAT_STACK_CHECK"},
            {"0xc0000093", "STATUS_FLOAT_UNDERFLOW"},
            {"0xc0000094", "STATUS_INTEGER_DIVIDE_BY_ZERO"},
            {"0xc0000095", "STATUS_INTEGER_OVERFLOW"},
            {"0xc0000096", "STATUS_PRIVILEGED_INSTRUCTION"},
            {"0xc00000fd", "STATUS_STACK_OVERFLOW"},
            {"0xc000013a", "STATUS_CONTROL_C_EXIT"},
            {"0xc0000142", "STATUS_DLL_INIT_FAILED"},
            {"0xc000026b", "STATUS_DLL_INIT_FAILED_LOGOFF"},
            {"0x40000015", "std::bad_alloc" }
        };
    }
}
