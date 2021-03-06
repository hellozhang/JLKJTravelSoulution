﻿using JLKJTravelSoulution.IServices;
using JLKJTravelSoulution.IRepository;
using JLKJTravelSoulution.Services.BASE;
using JLKJTravelSoulution.Model.Models;
using System.Threading.Tasks;
using System.Linq;

namespace JLKJTravelSoulution.Services
{	
	/// <summary>
	/// RoleServices
	/// </summary>	
	public class RoleServices : BaseServices<Role>, IRoleServices
    {
	
        IRoleRepository dal;
        public RoleServices(IRoleRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="roleName"></param>
       /// <returns></returns>
        public async Task<Role> SaveRole(string roleName)
        {
            Role role = new Role(roleName);
            Role model = new Role();
            var userList = await dal.Query(a => a.Name == role.Name && a.Enabled);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await dal.Add(role);
                model = await dal.QueryByID(id);
            }

            return model;

        }
    }
}
