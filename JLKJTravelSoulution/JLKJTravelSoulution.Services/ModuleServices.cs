using JLKJTravelSoulution.Services.BASE;
using JLKJTravelSoulution.Model.Models;
using JLKJTravelSoulution.IRepository;
using JLKJTravelSoulution.IServices;

namespace JLKJTravelSoulution.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class ModuleServices : BaseServices<Module>, IModuleServices
    {
	
        IModuleRepository dal;
        public ModuleServices(IModuleRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
       
    }
}
