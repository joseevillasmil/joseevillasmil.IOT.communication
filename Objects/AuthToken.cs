using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace joseevillasmil.IOT.Communication.Objects
{
    public class AuthToken
    {
        public DateTime startAt { get; set; }
        public DateTime endsAt { get; set; }
        public string role { get; set; }
        public string Signature { get; set; }

        public override string ToString()
        {
            return $"{role}|{endsAt.Ticks}|{endsAt.Ticks}";
        }
    }
}
