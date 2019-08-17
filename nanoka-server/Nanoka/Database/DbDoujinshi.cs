using System;
using System.Collections.Generic;
using Nanoka.Models;
using Nest;
using Newtonsoft.Json;

namespace Nanoka.Database
{
    [ElasticsearchType(RelationName = nameof(Doujinshi), IdProperty = nameof(Id))]
    public class DbDoujinshi
    {
        [Keyword(Name = "id", Index = false), JsonProperty("id")]
        public string Id { get; set; }

        [Date(Name = "up"), JsonProperty("up")]
        public DateTime UploadTime { get; set; }

        [Date(Name = "ud"), JsonProperty("ud")]
        public DateTime UpdateTime { get; set; }

        // instead of using TextAttribute, let NEST configure multi-field for us without using fluent builder
        [PropertyName("on"), JsonProperty("on")]
        public string OriginalName { get; set; }

        [PropertyName("rn"), JsonProperty("rn")]
        public string RomanizedName { get; set; }

        [PropertyName("en"), JsonProperty("en")]
        public string EnglishName { get; set; }

        [Number(Name = "ca"), JsonProperty("ca")]
        public DoujinshiCategory Category { get; set; }

        [Number(Name = "sc"), JsonProperty("sc")]
        public int Score { get; set; }

        [Nested(Name = "var"), JsonProperty("var")]
        public List<DbDoujinshiVariant> Variants { get; set; }

        /// <summary>
        /// Cached values of the number of pages in each <see cref="Variants"/>.
        /// </summary>
        [Number(Name = "pg_n"), JsonProperty("pg_n")]
        public int[] PageCounts { get; set; }

        public DbDoujinshi Apply(Doujinshi doujinshi)
        {
            if (doujinshi == null)
                return null;

            Id            = doujinshi.Id.ToShortString();
            UploadTime    = doujinshi.UploadTime;
            UpdateTime    = doujinshi.UpdateTime;
            OriginalName  = doujinshi.OriginalName ?? OriginalName;
            RomanizedName = doujinshi.RomanizedName ?? RomanizedName;
            EnglishName   = doujinshi.EnglishName ?? EnglishName;
            Category      = doujinshi.Category;
            Score         = doujinshi.Score;

            Variants   = doujinshi.Variants?.ToList(v => new DbDoujinshiVariant().Apply(v)) ?? Variants;
            PageCounts = Variants.ToArray(v => v.PageCount);

            return this;
        }

        public Doujinshi ApplyTo(Doujinshi doujinshi)
        {
            doujinshi.Id            = Id.ToGuid();
            doujinshi.UploadTime    = UploadTime;
            doujinshi.UpdateTime    = UpdateTime;
            doujinshi.OriginalName  = OriginalName ?? doujinshi.OriginalName;
            doujinshi.RomanizedName = RomanizedName ?? doujinshi.RomanizedName;
            doujinshi.EnglishName   = EnglishName ?? doujinshi.EnglishName;
            doujinshi.Category      = Category;
            doujinshi.Score         = Score;

            doujinshi.Variants = Variants?.ToList(v => v.ApplyTo(new DoujinshiVariant())) ?? doujinshi.Variants;

            return doujinshi;
        }
    }
}