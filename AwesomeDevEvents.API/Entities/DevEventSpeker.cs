using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata.Ecma335;

namespace AwesomeDevEvents.API.Entities
{
    public class DevEventSpeaker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TalkTitle { get; set; }
        public string TalkDescreption { get; set; }
        public string LinkedInProfile { get; set; }
        public Guid DevEventId { get; set; }
    }
}