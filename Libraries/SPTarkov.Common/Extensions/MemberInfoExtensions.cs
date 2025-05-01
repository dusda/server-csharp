using System.Reflection;
using System.Text.Json.Serialization;

namespace SPTarkov.Common.Extensions;

public static class MemberInfoExtensions
{
  public static string GetJsonName(this MemberInfo memberInfo)
  {
    var jsonPropertyAttribute = memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
    if (jsonPropertyAttribute != null)
      return jsonPropertyAttribute.Name;

    var jsonIgnoreAttribute = memberInfo.GetCustomAttribute<JsonIgnoreAttribute>();
    if (jsonIgnoreAttribute != null)
      return string.Empty;

    return memberInfo.Name;
  }
}
