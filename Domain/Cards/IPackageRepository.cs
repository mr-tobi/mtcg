using Domain.Repositories;

namespace Domain.Cards;

public interface IPackageRepository : IRepository<Package>
{
    /// <summary>
    /// Save package.
    /// </summary>
    /// <param name="package">Package to save</param>
    /// <returns>Saved package</returns>
    Package Save(Package package);

    /// <summary>
    /// Remove package
    /// </summary>
    /// <param name="package">Package to remove</param>
    void Remove(Package package);

    /// <summary>
    /// Get next available package.
    /// </summary>
    /// <returns>Next available package</returns>
    Package GetNextPackage();
}
