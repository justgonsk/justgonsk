using System.Collections.Generic;

namespace JustGoModels.Models.View
{
    public class JustGoUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Event> AddedEvents { get; set; }
    }
}