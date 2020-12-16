﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SnomedTemplateService.Core.Domain
{
    public class TemplateData
    {
        private IList<TemplateAuthor> authors;
        private IDictionary<string, string> itemTitles;
        private IDictionary<string, string> itemDescriptions;
        private string title;

        public TemplateData(string id, string timestamp, string snomedVersion, string snomedBranch, string etl, IEnumerable<string> tags)
        {
            if (string.IsNullOrEmpty(snomedVersion))
            {
                throw new ArgumentException($"'{nameof(snomedVersion)}' cannot be null or empty", nameof(snomedVersion));
            }

            if (string.IsNullOrEmpty(snomedBranch))
            {
                throw new ArgumentException($"'{nameof(snomedBranch)}' cannot be null or empty", nameof(snomedBranch));
            }

            if (string.IsNullOrEmpty(etl))
            {
                throw new ArgumentException($"'{nameof(etl)}' cannot be null or empty", nameof(etl));
            }
            
            if(tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            Tags = tags.ToList();
            
            if (Tags.Count == 0)
            {
                throw new ArgumentException("Template should have at least one tag");
            }

            Id = id;
            Title = $"Template[id={id}]";
            TimeStamp = timestamp;
            SnomedVersion = snomedVersion.Trim();
            SnomedBranch = snomedBranch.Trim();
            Etl = etl.Trim();
            Authors = new List<TemplateAuthor>();
            itemTitles = new Dictionary<string, string>();
            itemDescriptions = new Dictionary<string, string>();
        }

        public string Id { get; }
        public string TimeStamp { get; }
        public IList<TemplateAuthor> Authors
        {
            get => authors;
            set => authors = new List<TemplateAuthor>(value ?? throw new ArgumentNullException(nameof(Authors)));
        }
        public string Title { get => title; set => title = value ?? throw new ArgumentNullException(nameof(Title)); }
        public string Description { get; set; }
        public string SnomedVersion { get; }
        public string SnomedBranch { get; }
        public string StringFormat { get; set; }
        public string Etl { get; }
        public ICollection<string> Tags { get; }
        public IDictionary<string, string> ItemTitles
        {
            get => itemTitles;
            set => itemTitles = new Dictionary<string, string>(value ?? throw new ArgumentNullException(nameof(ItemTitles)));
        }
        public IDictionary<string, string> ItemDescriptions
        {
            get => itemDescriptions;
            set => itemDescriptions = new Dictionary<string, string>(value ?? throw new ArgumentNullException(nameof(ItemDescriptions)));
        }

    }

    public class TemplateAuthor
    {
        public string Name { get; }

        public TemplateAuthor(string name)
        {
            Name = name;
        }

        public string Contact { get; set; }
    }
}
