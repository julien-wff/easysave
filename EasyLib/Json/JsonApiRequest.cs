using EasyLib.Enums;

namespace EasyLib.Json;

public struct JsonApiRequest
{
    public ApiAction Action { get; set; }
    public JsonJob Job { get; set; }
}
