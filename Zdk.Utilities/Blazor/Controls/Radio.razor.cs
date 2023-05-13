using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Zdk.Utilities.Blazor
{
    public partial class Radio<TItem>
    {
        [Parameter]
        public TItem Selected { get; set; }

        [Parameter]
        public EventCallback<TItem> SelectedChanged { get; set; }

        private TItem Value { get => Selected; set => SelectedChanged.InvokeAsync(value); }

        [Parameter]
        public IDictionary<string, TItem> Options { get; set; }

        private string IsSelected(TItem item)
        {
            return item.Equals(Value) ? StyleTexts.Selected : StyleTexts.NotSelected;
        }
    }
}