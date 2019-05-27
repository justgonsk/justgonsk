using System.Collections.Generic;
using System.Linq;
using JustGoModels.Interfaces;
using JustGoModels.Models.View;
using Microsoft.AspNetCore.Identity;

namespace JustGoModels.Models.Auth
{
    public class JustGoUser : IdentityUser, IConvertibleToViewModel<JustGoUserViewModel>
    {
        public virtual ICollection<Event> AddedEvents { get; set; }

        public JustGoUserViewModel ToViewModel()
        {
            return new JustGoUserViewModel
            {
                UserName = UserName,
                Email = Email,
                AddedEvents = AddedEvents.ToList()
            };
        }
    }
}