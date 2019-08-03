using System;
using System.Collections.Generic;
using Nanoka.Core;
using Nanoka.Core.Models;
using Nest;

namespace Nanoka.Web.Database
{
    [ElasticsearchType(RelationName = nameof(Doujinshi), IdProperty = nameof(Id))]
    public class DbDoujinshi
    {
        public string Id { get; set; }

        [Date(Name = "up")]
        public DateTime UploadTime { get; set; }

        [Date(Name = "ud")]
        public DateTime UpdateTime { get; set; }

        [Text(Name = "on")]
        public string OriginalName { get; set; }

        [Text(Name = "rn")]
        public string RomanizedName { get; set; }

        [Text(Name = "en")]
        public string EnglishName { get; set; }

        [Number(Name = "sc")]
        public int Score { get; set; }

        [Nested(Name = "var")]
        public List<DbDoujinshiVariant> Variations { get; set; }

        public void Apply(Doujinshi doujinshi)
        {
            Id            = doujinshi.Id.ToShortString();
            UploadTime    = doujinshi.UploadTime;
            UpdateTime    = doujinshi.UpdateTime;
            OriginalName  = doujinshi.OriginalName ?? OriginalName;
            RomanizedName = doujinshi.RomanizedName ?? RomanizedName;
            EnglishName   = doujinshi.EnglishName ?? EnglishName;
            Score         = doujinshi.Score;

            Variations = doujinshi.Variations?.ToList(v =>
            {
                var variant = new DbDoujinshiVariant();
                variant.Apply(v);

                return variant;
            }) ?? Variations;
        }

        public void ApplyTo(Doujinshi doujinshi)
        {
            doujinshi.Id            = Id.ToGuid();
            doujinshi.UploadTime    = UploadTime;
            doujinshi.UpdateTime    = UpdateTime;
            doujinshi.OriginalName  = OriginalName ?? doujinshi.OriginalName;
            doujinshi.RomanizedName = RomanizedName ?? doujinshi.RomanizedName;
            doujinshi.EnglishName   = EnglishName ?? doujinshi.EnglishName;
            doujinshi.Score         = Score;

            doujinshi.Variations = Variations?.ToArray(v =>
            {
                var variant = new DoujinshiVariant();
                v.ApplyTo(variant);

                return variant;
            }) ?? doujinshi.Variations;
        }
    }
}