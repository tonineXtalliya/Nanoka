using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Nanoka.Models
{
    /// <summary>
    /// Represents an image in a booru.
    /// </summary>
    public class Image : ImageBase, IHasId, IHasScore
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("type")]
        public ImageMediaType MediaType { get; set; }
    }

    public class ImageBase : IHasEntityType, IValidatableObject
    {
        public const int MinimumTagCount = 5;

        [JsonProperty("tags"), Required]
        public Dictionary<ImageTag, string[]> Tags { get; set; }

        [JsonProperty("source")]
        public ExternalSource[] Sources { get; set; }

        [JsonProperty("rating")]
        public MaterialRating Rating { get; set; }

        [JsonProperty("notes")]
        public ImageNote[] Notes { get; set; }

#region Meta

        [JsonIgnore]
        NanokaEntity IHasEntityType.Type => NanokaEntity.Image;

#endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Tags.Values.SelectMany(x => x).Count() < MinimumTagCount)
                yield return new ValidationResult($"Must provide at least {MinimumTagCount} tags.");
        }
    }
}