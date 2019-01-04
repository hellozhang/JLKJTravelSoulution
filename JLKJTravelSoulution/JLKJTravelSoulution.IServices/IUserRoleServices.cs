using JLKJTravelSoulution.IServices.BASE;
using JLKJTravelSoulution.Model.Models;
using System.Threading.Tasks;

namespace JLKJTravelSoulution.IServices
{	
	/// <summary>
	/// UserRoleServices
	/// </summary>	
    public interface IUserRoleServices :IBaseServices<UserRole>
	{

        Task<UserRole> SaveUserRole(int uid, int rid);
    }
}

