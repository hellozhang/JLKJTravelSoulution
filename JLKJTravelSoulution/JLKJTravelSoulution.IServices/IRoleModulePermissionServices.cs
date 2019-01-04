using JLKJTravelSoulution.IServices.BASE;
using JLKJTravelSoulution.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JLKJTravelSoulution.IServices
{	
	/// <summary>
	/// RoleModulePermissionServices
	/// </summary>	
    public interface IRoleModulePermissionServices :IBaseServices<RoleModulePermission>
	{

        Task<List<RoleModulePermission>> GetRoleModule();
    }
}
