using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLKJTravelSoulution.IRepository;
using JLKJTravelSoulution.IServices;
using JLKJTravelSoulution.Model.Models;
using JLKJTravelSoulution.Services.BASE;

namespace JLKJTravelSoulution.Services
{
    public partial class PasswordLibServices : BaseServices<PasswordLib>, IPasswordLibServices
    {
        IPasswordLibRepository dal;
        public PasswordLibServices(IPasswordLibRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }

    }
}
