using System.Collections.Generic;
using Nanoka.Core;
using Nanoka.Core.Models;
using Nest;

namespace Nanoka.Web.Database
{
    // nested object of doujinshi
    public class DbDoujinshiVariant
    {
        [Keyword(Name = "upu")]
        public string UploaderId { get; set; }

        [Text(Name = "a")]
        public string[] Artist { get; set; }

        [Text(Name = "g")]
        public string[] Group { get; set; }

        [Text(Name = "p")]
        public string[] Parody { get; set; }

        [Text(Name = "c")]
        public string[] Character { get; set; }

        [Text(Name = "ca")]
        public string[] Category { get; set; }

        [Text(Name = "l")]
        public string[] Language { get; set; }

        [Text(Name = "t")]
        public string[] Tag { get; set; }

        [Text(Name = "co")]
        public string[] Convention { get; set; }

        [Text(Name = "src")]
        public string Source { get; set; }

        [Nested(Name = "page")]
        public List<DbDoujinshiPage> Pages { get; set; }

        public DbDoujinshiVariant Apply(DoujinshiVariant variant)
        {
            if (variant == null)
                return null;

            UploaderId = variant.UploaderId.ToShortString();
            Artist     = variant.Metas?.GetOrDefault(DoujinshiMeta.Artist) ?? Artist;
            Group      = variant.Metas?.GetOrDefault(DoujinshiMeta.Group) ?? Group;
            Parody     = variant.Metas?.GetOrDefault(DoujinshiMeta.Parody) ?? Parody;
            Character  = variant.Metas?.GetOrDefault(DoujinshiMeta.Character) ?? Character;
            Category   = variant.Metas?.GetOrDefault(DoujinshiMeta.Category) ?? Category;
            Language   = variant.Metas?.GetOrDefault(DoujinshiMeta.Language) ?? Language;
            Tag        = variant.Metas?.GetOrDefault(DoujinshiMeta.Tag) ?? Tag;
            Convention = variant.Metas?.GetOrDefault(DoujinshiMeta.Convention) ?? Convention;
            Source     = variant.Source ?? Source;

            Pages = variant.Pages?.ToList(p => new DbDoujinshiPage().Apply(p)) ?? Pages;

            return this;
        }

        public DoujinshiVariant ApplyTo(DoujinshiVariant variant)
        {
            variant.UploaderId = UploaderId.ToGuid();

            variant.Metas = new Dictionary<DoujinshiMeta, string[]>
            {
                { DoujinshiMeta.Artist, Artist ?? variant.Metas.GetOrDefault(DoujinshiMeta.Artist) },
                { DoujinshiMeta.Group, Group ?? variant.Metas.GetOrDefault(DoujinshiMeta.Group) },
                { DoujinshiMeta.Parody, Parody ?? variant.Metas.GetOrDefault(DoujinshiMeta.Parody) },
                { DoujinshiMeta.Character, Character ?? variant.Metas.GetOrDefault(DoujinshiMeta.Character) },
                { DoujinshiMeta.Category, Category ?? variant.Metas.GetOrDefault(DoujinshiMeta.Category) },
                { DoujinshiMeta.Language, Language ?? variant.Metas.GetOrDefault(DoujinshiMeta.Language) },
                { DoujinshiMeta.Tag, Tag ?? variant.Metas.GetOrDefault(DoujinshiMeta.Tag) },
                { DoujinshiMeta.Convention, Convention ?? variant.Metas.GetOrDefault(DoujinshiMeta.Convention) }
            };

            variant.Source = Source ?? variant.Source;

            variant.Pages = Pages?.ToList(p => p.ApplyTo(new DoujinshiPage())) ?? variant.Pages;

            return variant;
        }
    }
}