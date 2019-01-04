    

using JLKJTravelSoulution.IServices.BASE;
using JLKJTravelSoulution.Model.Models;
using System.Threading.Tasks;

namespace JLKJTravelSoulution.IServices
{	
	/// <summary>
	/// sysUserInfoServices
	/// </summary>	
    public interface IsysUserInfoServices :IBaseServices<sysUserInfo>
	{
        Task<sysUserInfo> SaveUserInfo(string loginName, string loginPWD);
        Task<string> GetUserRoleNameStr(string loginName, string loginPWD);
    }
}
