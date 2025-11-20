using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.DTOs.User
{
    public class UserUpdateDto
    {
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
