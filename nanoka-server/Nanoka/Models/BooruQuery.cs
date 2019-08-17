using System;
using System.Collections.Generic;
using Nanoka.Models.Query;
using Newtonsoft.Json;

namespace Nanoka.Models
{
    public class BooruQuery : QueryWrapperBase<BooruQuery, BooruQuerySort>
    {
        [JsonProperty("upload")]
        public RangeQuery<DateTime> UploadTime { get; set; }

        [JsonProperty("update")]
        public RangeQuery<DateTime> UpdateTime { get; set; }

        [JsonProperty("tags")]
        public Dictionary<BooruTag, TextQuery> Tags { get; set; }

        [JsonProperty("rating")]
        public FilterQuery<BooruRating> Rating { get; set; }

        [JsonProperty("score")]
        public RangeQuery<int> Score { get; set; }

        [JsonProperty("source")]
        public TextQuery Source { get; set; }

        [JsonProperty("width")]
        public RangeQuery<int> Width { get; set; }

        [JsonProperty("height")]
        public RangeQuery<int> Height { get; set; }

        [JsonProperty("type")]
        public TextQuery MediaType { get; set; }

        public BooruQuery WithUploadTime(RangeQuery<DateTime> q) => Set(x => x.UploadTime = q);
        public BooruQuery WithUpdateTime(RangeQuery<DateTime> q) => Set(x => x.UpdateTime = q);
        public BooruQuery WithTag(BooruTag tag, TextQuery q) => Set(x => x.Tags[tag] = q);
        public BooruQuery WithRating(FilterQuery<BooruRating> q) => Set(x => x.Rating = q);
        public BooruQuery WithSource(TextQuery q) => Set(x => x.Source = q);
        public BooruQuery WithWidth(RangeQuery<int> q) => Set(x => x.Width = q);
        public BooruQuery WithHeight(RangeQuery<int> q) => Set(x => x.Height = q);
        public BooruQuery WithMediaType(TextQuery q) => Set(x => x.MediaType = q);
    }
}