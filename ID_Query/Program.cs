using RestSharp;
using System.Diagnostics;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ID_Query
{
    internal class Program
    {
        static string SessionId { get; set; } = "";
        #region main
        static async Task Main(string[] args)
        {            
            await Prompt();
        }        

        static async Task Prompt()
        {
            Console.WriteLine("Fetch ID through PID - By TNA\n\n");

            if (IsBf1Run() is false)
            {
                Console.WriteLine("BF1 has not been found. Closing ....");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Fetching SessionID ...");
            if (GetSessionID() is false)
            {
                Console.WriteLine("Failed to get SessionID, make sure your game is open and online!");
                Console.ReadLine();
                return;
            }

            Init();

            while (true)
            {
                Console.WriteLine("\nPID: ");
                string? input = Console.ReadLine();

                if (input is null) continue;

                var ids = await GetID(Convert.ToInt64(input));

                Console.WriteLine($"\nID: {ids.Value.Item1}\nID2: {ids.Value.Item2}");
            }
        }

        public static bool IsBf1Run()
        {
            var pArray = Process.GetProcessesByName("bf1");
            if (pArray.Length > 0)
            {
                foreach (var item in pArray)
                {
                    if (item.MainWindowTitle.Equals("Battlefield™ 1"))
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region SessionID

        public const string SessionIDMask = "58 2D 47 61 74 65 77 61 79 53 65 73 73 69 6F 6E";

        private static bool GetSessionID()
        {
            Debug.WriteLine("1");
            var str = SearchMemory(SessionIDMask);
            if (str != string.Empty)
            {
                SessionId = str;
                Console.WriteLine($"Got SessionID successfully {str}");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to Get SessionID");
                return false;
            }
        }

        private struct CanReadAddress
        {
            public long Address;
            public int Size;
        }

        private static List<CanReadAddress> canReads = new();

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION64
        {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public int __alignment1;
            public ulong RegionSize;
            public int State;
            public int Protect;
            public int Type;
            public int __alignment2;
        }

        [DllImport("kernel32.dll")]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, int dwLength);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        private static int processId;

        public static IntPtr GetProcessHandle()
        {
            var pArray = Process.GetProcessesByName("bf1");
            processId = pArray[0].Id;
            return OpenProcess(ProcessAccessFlags.All, false, processId);
        }

        [Flags]
        private enum AllocationType : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        private enum AllocationProtect : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] lpBuffer, int nsize, out IntPtr lpNumberOfBytesRead);

        private static long FindPattern(string pattern, long baseAddress, int moduleSize)
        {            
            List<byte> tempArray = new();
            foreach (var each in pattern.Split(' '))
            {
                tempArray.Add(Convert.ToByte(each, 16));
            }
            byte[] patternByteArray = tempArray.ToArray();

            byte[] localModulebytes = new byte[moduleSize];
            ReadProcessMemory(GetProcessHandle(), baseAddress, localModulebytes, moduleSize, out _);

            for (int indexAfterBase = 0; indexAfterBase < localModulebytes.Length; indexAfterBase++)
            {
                bool noMatch = false;

                if (localModulebytes[indexAfterBase] != patternByteArray[0])
                    continue;

                for (var MatchedIndex = 0; MatchedIndex < patternByteArray.Length && indexAfterBase + MatchedIndex < localModulebytes.Length; MatchedIndex++)
                {
                    if (patternByteArray[MatchedIndex] == 0x0)
                        continue;
                    if (patternByteArray[MatchedIndex] != localModulebytes[indexAfterBase + MatchedIndex])
                    {
                        noMatch = true;
                        break;
                    }
                }

                if (!noMatch)
                    return baseAddress + indexAfterBase;
            }

            return 0;
        }

        public static string ReadString(long address, int size)
        {
            byte[] buffer = new byte[size];
            ReadProcessMemory(OpenProcess(ProcessAccessFlags.All, false, processId), address, buffer, size, out _);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0)
                {
                    byte[] _buffer = new byte[i];
                    Buffer.BlockCopy(buffer, 0, _buffer, 0, i);
                    return Encoding.ASCII.GetString(_buffer);
                }
            }

            return Encoding.ASCII.GetString(buffer);
        }

        private static bool IsGuidByReg(string strSrc)
        {
            Debug.WriteLine("5");

            strSrc = strSrc.ToLower();
            Regex reg = new("^[a-f0-9]{8}(-[a-f0-9]{4}){3}-[a-f0-9]{12}$", RegexOptions.Compiled);
            return reg.IsMatch(strSrc);
        }

        public static string SearchMemory(string mask)
        {
            Debug.WriteLine("2");
            canReads.Clear();

            var mbi = default(MEMORY_BASIC_INFORMATION64);
            var size = Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64));

            long baseAddress = 0;
            long maxAddress = long.MaxValue;

            Debug.WriteLine("3");

            while (baseAddress >= 0 && baseAddress <= maxAddress && mbi.RegionSize >= 0)
            {
                // 扫描内存信息
                if (VirtualQueryEx(GetProcessHandle(), new IntPtr(baseAddress), out mbi, size) == 0)
                    break;

                // 如果是已物理分配 并且是 可读内存
                if (mbi.State == (int)AllocationType.Commit && mbi.Protect == (int)AllocationProtect.PAGE_READWRITE)
                {
                    canReads.Add(new CanReadAddress()
                    {
                        Address = baseAddress,
                        Size = (int)mbi.RegionSize
                    });
                }

                // 设置基地址偏移
                baseAddress += (long)mbi.RegionSize;
            }

            Debug.WriteLine("4");

            foreach (var item in canReads)
            {
                var addr = FindPattern(mask, item.Address, item.Size);
                if (addr != 0)
                {
                    var str = ReadString(addr, 54);
                    str = str.Replace("X-GatewaySession:", "").Trim();
                    if (IsGuidByReg(str))
                    {
                        return str;
                    }
                }
            }

            Debug.WriteLine("6");

            return string.Empty;
        }
        #endregion

        #region GetID
        static async Task<(string, string)?> GetID(long personaId)
        {            
            string id = "";
            string id2 = "";

            var result = await GetPersonasByIds(personaId);
            Debug.WriteLine(result.IsSuccess + " " + result.Message);

            if (result.IsSuccess)
            {
                try //tna
                {
                    JsonNode jNode = JsonNode.Parse(result.Message)!;
                    id = jNode["result"]![$"{personaId}"]![$"nucleusId"]!.GetValue<string>();
                    id2 = jNode["result"]![$"{personaId}"]!["platformId"]!.GetValue<string>();
                }
                catch //tna
                {
                    Console.WriteLine($"Failed to Parse Information: {result.Message}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("EA API did not respond with content.");
                return null;
            }

            return (id, id2);
        }
        #endregion

        #region BF1API
        private const string Host = "https://sparta-gw.battlelog.com/jsonrpc/pc/api";

        private static RestClient client;
        private static Dictionary<string, string> headers;

        public static void Init()
        {
            if (client == null)
            {
                var options = new RestClientOptions(Host)
                {
                    MaxTimeout = 5000,
                    ThrowOnAnyError = true
                };

                client = new RestClient(options);
                headers = new Dictionary<string, string>
                {
                    ["User-Agent"] = "ProtoHttp 1.3/DS 15.1.2.1.0 (Windows)",
                    ["X-GatewaySession"] = SessionId,
                    ["X-ClientVersion"] = "release-bf1-lsu35_26385_ad7bf56a_tunguska_all_prod",
                    ["X-DbId"] = "Tunguska.Shipping2PC.Win32",
                    ["X-CodeCL"] = "3779779",
                    ["X-DataCL"] = "3779779",
                    ["X-SaveGameVersion"] = "26",
                    ["X-HostingGameId"] = "tunguska",
                    ["X-Sparta-Info"] = "tenancyRootEnv=unknown; tenancyBlazeEnv=unknown"
                };
            }
        }

        public static async Task<RespContent> GetPersonasByIds(long personaId)
        {
            var respContent = new RespContent();

            try
            {
                headers["X-GatewaySession"] = SessionId;
                respContent.IsSuccess = false;

                var reqBody = new
                {
                    jsonrpc = "2.0",
                    method = "RSP.getPersonasByIds",
                    @params = new
                    {
                        game = "tunguska",
                        personaIds = new[] { personaId }
                    },
                    id = Guid.NewGuid()
                };

                var request = new RestRequest()
                    .AddHeaders(headers)
                    .AddJsonBody(reqBody);

                var response = await client.ExecutePostAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    respContent.IsSuccess = true;
                    respContent.Message = response.Content;
                }
                else
                {
                    var respError = JsonDese<RespError>(response.Content);
                    respContent.Message = $"{respError.error.code} {respError.error.message}";
                }
            }
            catch (Exception ex)
            {
                respContent.Message = ex.Message;                
            }

            return respContent;
        }


        private static JsonSerializerOptions Options1 = new()
        {
            IncludeFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        public static T JsonDese<T>(string result)
        {
            return JsonSerializer.Deserialize<T>(result, Options1);
        }


        public class RespContent
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public double ExecTime { get; set; }
        }
        public class RespError
        {
            public string jsonrpc { get; set; }
            public string id { get; set; }
            public Error error { get; set; }
        }
        public class Error
        {
            public string message { get; set; }
            public int code { get; set; }
        }
        #endregion
    }
}