using SPTarkov.Common.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace SPTarkov.Server.Core.Generators.WeaponGen;

[Injectable]
public class InventoryMagGen()
{
  readonly TemplateItem _ammoTemplate;
  readonly TemplateItem _magazineTemplate;
  readonly GenerationData _magCounts;
  readonly BotBaseInventory _pmcInventory;
  readonly TemplateItem _weaponTemplate;

  public InventoryMagGen
  (
      GenerationData magCounts,
      TemplateItem magazineTemplate,
      TemplateItem weaponTemplate,
      TemplateItem ammoTemplate,
      BotBaseInventory pmcInventory
  ) : this()
  {
    _magCounts = magCounts;
    _magazineTemplate = magazineTemplate;
    _weaponTemplate = weaponTemplate;
    _ammoTemplate = ammoTemplate;
    _pmcInventory = pmcInventory;
  }

  public GenerationData GetMagCount()
  {
    return _magCounts;
  }

  public TemplateItem GetMagazineTemplate()
  {
    return _magazineTemplate;
  }

  public TemplateItem GetWeaponTemplate()
  {
    return _weaponTemplate;
  }

  public TemplateItem GetAmmoTemplate()
  {
    return _ammoTemplate;
  }

  public BotBaseInventory GetPmcInventory()
  {
    return _pmcInventory;
  }
}
