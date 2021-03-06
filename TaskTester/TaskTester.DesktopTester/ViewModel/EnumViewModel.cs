﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TaskTester.DesktopTester.ViewModel
{
    public sealed class EnumViewModel<TEnum>
    {
        public static implicit operator EnumViewModel<TEnum>(TEnum value) =>
            new EnumViewModel<TEnum>(value);

        private static readonly TEnum[] Values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToArray();
        private static readonly string[] NamesStatic = Values.Select(x => x.ToString()).ToArray();

        [JsonIgnore]
        public IEnumerable<string> Names => NamesStatic;

        [JsonIgnore]
        public int SelectedIndex { get; set; }

        [JsonProperty]
        public TEnum SelectedValue
        {
            get => Values[SelectedIndex];
            set => SelectedIndex = Array.FindIndex<TEnum>(Values, x => x.Equals(value));
        }

        public EnumViewModel() : this(default(TEnum)) { }
        public EnumViewModel(TEnum value) => SelectedValue = value;
    }
}
