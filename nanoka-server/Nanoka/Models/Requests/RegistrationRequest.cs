using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Nanoka.Models.Requests
{
    public class RegistrationRequest
    {
        [JsonProperty("username"), Required]
        [RegularExpression(UserBase.UsernameRegex)]
        public string Username { get; set; }

        [JsonProperty("recaptcha"), Required]
        public string RecaptchaToken { get; set; }
    }

    public class RegistrationResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("secret")]
        public Guid Secret { get; set; }
    }
}