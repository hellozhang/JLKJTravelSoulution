using JLKJTravelSoulution.IServices.BASE;
using JLKJTravelSoulution.Model.Models;
using System.Threading.Tasks;

namespace JLKJTravelSoulution.IServices
{	
	/// <summary>
	/// RoleServices
	/// </summary>	
    public interface IRoleServices :IBaseServices<Role>
	{
        Task<Role> SaveRole(string roleName);


    }
}
