using System;

namespace ExtraMessenger.Hubs
{
    public class EditMessageDTO
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public string ChatInteractionId { get; set; }
    }
}