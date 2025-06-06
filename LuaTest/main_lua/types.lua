---------------------------------------
-- types
---------------------------------------
---API test
---@class WinForm
---@field sleep fun(ms: nuber): void
---@field LED fun(on_off: boolean): void
WinForm = {}


---Web
---@class http_request
---@field IsDone fun(): boolean
---@field Ping fun(ip: string): boolean
---@field GetResponseCode fun(): number
---@field GetResponseText fun(): string
---@field GetResponseData fun(): string|table<number, integer>
---@field HasError fun(): boolean
---@field GetError fun(): string

---@class http
---@field SendRequest fun(url: string, method: string, headers?: table<string, string>, body?: string): http_request
---@field Ping fun(ip: string): boolean
http = {}


---MySQL
---@class mysql
---@field Connect fun(connectionString: string): boolean, string?
---@field Disconnect fun(): boolean
---@field IsConnected fun(): boolean
---@field ExecuteNonQuery fun(sql: string): integer, string?
---@field ExecuteScalar fun(sql: string): any, string?
---@field ExecuteQuery fun(sql: string): table<string, any>[], string?
---@field BeginTransaction fun(): boolean
---@field Commit fun(): boolean
---@field Rollback fun(): boolean
mysql = {}


---@class file_api
---@field Exists fun(path: string): boolean
---@field ReadAllText fun(path: string): string
---@field WriteAllText fun(path: string, text: string): boolean
---@field AppendAllText fun(path: string, text: string): boolean
---@field Delete fun(path: string): boolean
---@field Copy fun(source: string, dest: string, overwrite?: boolean): boolean
---@field Move fun(source: string, dest: string): boolean
---@field GetSize fun(path: string): number

---@class dir_api
---@field Exists fun(path: string): boolean
---@field Create fun(path: string): boolean
---@field Delete fun(path: string, recursive?: boolean): boolean
---@field GetFiles fun(path: string): string[]
---@field GetDirectories fun(path: string): string[]

file = {} ---@type file_api
dir = {} ---@type dir_api



---DateTime
---@class DateTimeObject
---@field ToUnixTime fun(self: DateTimeObject): number
---@field ToString fun(self: DateTimeObject, format?: string): string
---@field AddSeconds fun(self: DateTimeObject, sec: number): DateTimeObject
---@field AddDays fun(self: DateTimeObject, days: number): DateTimeObject
---@field Subtract fun(self: DateTimeObject, other: DateTimeObject): TimeSpanObject
---@field CompareTo fun(self: DateTimeObject, other: DateTimeObject): integer
---@field Year number
---@field Month number
---@field Day number
---@field Hour number
---@field Minute number
---@field Second number
---@class TimeSpanObject
---@field TotalSeconds number
---@field TotalMilliseconds number

---@class DateTime
---@field Now fun(): DateTimeObject
---@field UtcNow fun(): DateTimeObject
---@field FromUnixTime fun(seconds: number): DateTimeObject
---@field Parse fun(dateString: string): DateTimeObject
---@field New fun(year: number, month: number, day: number, hour?: number, minute?: number, second?: number): DateTimeObject
DateTime = {}
