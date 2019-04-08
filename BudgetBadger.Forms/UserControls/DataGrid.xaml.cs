﻿using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetBadger.Forms.UserControls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DataGrid : SfDataGrid
    {
        public static BindableProperty FilterTextProperty = BindableProperty.Create(nameof(FilterText), typeof(string), typeof(DataGrid), propertyChanged: (bindable, oldVal, newVal) =>
        {
            ((DataGrid)bindable).UpdateFilter();
        });
        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        public static BindableProperty FilterProperty = BindableProperty.Create(nameof(Filter), typeof(Predicate<object>), typeof(DataGrid));
        public Predicate<object> Filter
        {
            get => (Predicate<object>)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public static BindableProperty SelectedCommandProperty = BindableProperty.Create(nameof(SelectedCommand), typeof(ICommand), typeof(ListView));
        public ICommand SelectedCommand
        {
            get => (ICommand)GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }

        public bool HasOtherTapGestureRecognizers { get; set; }

        public DataGrid ()
		{
			InitializeComponent ();    

            SelectionChanging += (sender, e) =>
            {
                if (HasOtherTapGestureRecognizers && (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.macOS))
                {
                    e.Cancel = true;
                }
            };
            SelectionChanged += (sender, e) =>
            {
                if (SelectedCommand != null)
                {
                    SelectedCommand.Execute(e.AddedItems.FirstOrDefault());
                }
            };
		}

        void UpdateFilter()
        {
            if (sfGrid != null && sfGrid.View != null && Filter != null)
            {
                sfGrid.View.Filter = Filter;
                sfGrid.View.RefreshFilter();
            }
        }
	}
}