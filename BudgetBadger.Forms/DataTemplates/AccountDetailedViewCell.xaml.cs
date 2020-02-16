﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using BudgetBadger.Forms.UserControls;
using Xamarin.Forms;

namespace BudgetBadger.Forms.DataTemplates
{
    public partial class AccountDetailedViewCell : Grid
    {
        public static BindableProperty SaveCommandProperty = BindableProperty.Create(nameof(SaveCommand), typeof(ICommand), typeof(AccountDetailedViewCell));
        public ICommand SaveCommand
        {
            get => (ICommand)GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        public static BindableProperty RefreshItemCommandProperty = BindableProperty.Create(nameof(RefreshItemCommand), typeof(ICommand), typeof(AccountDetailedViewCell));
        public ICommand RefreshItemCommand
        {
            get => (ICommand)GetValue(RefreshItemCommandProperty);
            set => SetValue(RefreshItemCommandProperty, value);
        }

        public static BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(AccountDetailedViewCell));
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public AccountDetailedViewCell()
        {
            InitializeComponent();
        }

        void Handle_EditClicked(object sender, EventArgs e)
        {
            DescriptionControl.IsReadOnly = false;
            EditButton.IsVisible = false;
            SaveCancelContainer.IsVisible = true;
        }

        void Handle_SaveClicked(object sender, EventArgs e)
        {
            DescriptionControl.IsReadOnly = true;
            EditButton.IsVisible = true;
            SaveCancelContainer.IsVisible = false;
            if (SaveCommand?.CanExecute(BindingContext) ?? false)
            {
                SaveCommand?.Execute(BindingContext);
            }
        }

        void Handle_CancelClicked(object sender, EventArgs e)
        {
            DescriptionControl.IsReadOnly = true;
            EditButton.IsVisible = true;
            SaveCancelContainer.IsVisible = false;
            if (RefreshItemCommand?.CanExecute(BindingContext) ?? false)
            {
                RefreshItemCommand?.Execute(BindingContext);
            }
        }
    }
}
