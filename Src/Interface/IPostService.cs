using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Src.Dtos.Post;

namespace Src.Interface
{
    public interface IPostService
    {
         Task<IEnumerable<PostDto>> GetAllPostsAsync();
    }
}