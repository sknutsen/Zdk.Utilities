using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Zdk.Utilities.Blazor
{
    public partial class Checkbox
    {
        [Parameter]
        public bool Checked { get; set; }

        [Parameter]
        public EventCallback<bool> CheckedChanged { get; set; }

        private bool Value { get => Checked; set => CheckedChanged.InvokeAsync(value); }

        [Parameter]
        public string Text { get; set; } = "";
        [Parameter]
        public EventCallback<MouseEventArgs> Click { get; set; }

        private string IsSelected()
        {
            return Checked ? StyleTexts.Selected : StyleTexts.NotSelected;
        }
    }
}