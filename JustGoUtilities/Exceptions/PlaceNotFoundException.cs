using System;
using System.Runtime.Serialization;
using JustGoModels.Models.Edit;

namespace JustGoUtilities.Exceptions
{
    [Serializable]
    internal class PlaceNotFoundException : Exception
    {
        public PlaceEditModel Place { get; }

        public PlaceNotFoundException()
        {
        }

        public PlaceNotFoundException(PlaceEditModel place)
        {
            this.Place = place;
        }

        public PlaceNotFoundException(string message) : base(message)
        {
        }

        public PlaceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PlaceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}