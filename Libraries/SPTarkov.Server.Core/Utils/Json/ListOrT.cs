namespace SPTarkov.Server.Core.Utils.Json;

public class ListOrT<T>(List<T>? list, T? item)
{
  // Do not remove, its used by the cloner
  // ReSharper disable once UnusedMember.Local
  ListOrT() : this(null, default)
  {
  }

  public List<T>? List
  {
    get;
    // Do not remove, its used by the cloner
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    set;
  } = list;

  public T? Item
  {
    get;
    // do not remove, its used by the cloner
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    set;
  } = item;

  public bool IsItem
  {
    get
    {
      return Item != null;
    }
  }

  public bool IsList
  {
    get
    {
      return List != null;
    }
  }
}
