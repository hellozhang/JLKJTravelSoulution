using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLKJTravelSoulution.IRepository;
using JLKJTravelSoulution.Model.Models;
using JLKJTravelSoulution.Repository.Base;

namespace JLKJTravelSoulution.Repository
{
    public class GuestbookRepository : BaseRepository<Guestbook>, IGuestbookRepository
    {
    }
}
