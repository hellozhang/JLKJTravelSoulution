using JLKJTravelSoulution.IServices.BASE;
using JLKJTravelSoulution.Model.Models;
using JLKJTravelSoulution.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLKJTravelSoulution.IServices
{
    public interface IBlogArticleServices :IBaseServices<BlogArticle>
    {
        Task<List<BlogArticle>> getBlogs();
        Task<BlogViewModels> getBlogDetails(int id);

    }

}
