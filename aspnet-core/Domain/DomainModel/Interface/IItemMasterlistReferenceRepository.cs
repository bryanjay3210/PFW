using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IItemMasterlistReferenceRepository : IRepository<ItemMasterlistReference>
    {
        #region Get Data
        Task<List<ItemMasterlistReference>> GetItemMasterlistReferences();
        Task<List<ItemMasterlistReference>> GetItemMasterlistReferencesByProductId(int productId);
        Task<ItemMasterlistReference?> GetItemMasterlistReference(int itemMasterlistReferenceId);
        #endregion

        #region Save Data
        Task<List<ItemMasterlistReference>> Create(ItemMasterlistReference itemMasterlistReference);
        Task<List<ItemMasterlistReference>> Update(ItemMasterlistReference itemMasterlistReference);
        Task<List<ItemMasterlistReference>> Delete(List<int> itemMasterlistReferenceIds);
        Task<List<ItemMasterlistReference>> SoftDelete(List<int> itemMasterlistReferenceIds);

        Task<List<ItemMasterlistReference>> CreateByProduct(ItemMasterlistReference itemMasterlistReference);
        Task<List<ItemMasterlistReference>> UpdateByProduct(ItemMasterlistReference itemMasterlistReference);

        Task<bool> RemoveExistingIsMainPartsLink(int itemMasterlistReferenceId, int productId);
        Task<bool> RemoveExistingIsMainOEM(int itemMasterlistReferenceId, int productId);
        #endregion
    }
}
