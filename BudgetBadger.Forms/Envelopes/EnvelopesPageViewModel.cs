﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using Prism.Commands;
using BudgetBadger.Forms.Navigation;
using BudgetBadger.Models;
using BudgetBadger.Core.Logic;
using BudgetBadger.Forms.ViewModels;

namespace BudgetBadger.Forms.Envelopes
{
    public class EnvelopesPageViewModel : BaseViewModel, INavigationAware
    {
        readonly IEnvelopeLogic EnvelopeLogic;
        readonly INavigationService NavigationService;

        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public BudgetSchedule Schedule { get; set; }
        public ObservableCollection<Budget> Budgets { get; set; }
        public Budget SelectedBudget { get; set; }
        public ObservableCollection<GroupedList<Budget>> GroupedBudgets { get; set; }

        public decimal Previous { get { return Budgets.Sum(b => b.PreviousAmount + b.PreviousActivity); }}
        public decimal Current { get { return Budgets.Sum(b => b.Amount + b.Activity); }}
        public decimal AvailableToBudget { get { return Budgets.Sum(b => b.Remaining); }}

        public bool SelectorMode { get; set; }
        public bool NormalMode { get { return !SelectorMode; }}

        public string SearchText { get; set; }
        public void OnSearchTextChanged()
        {
            ExecuteSearchCommand();
        }

        public EnvelopesPageViewModel(INavigationService navigationService, IEnvelopeLogic envelopeLogic)
        {
            Title = "Envelopes";

            EnvelopeLogic = envelopeLogic;
            NavigationService = navigationService;

            var scheduleResult = EnvelopeLogic.GetCurrentBudgetScheduleAsync(DateTime.Now).Result;
            Schedule = scheduleResult.Data;
            Budgets = new ObservableCollection<Budget>();

            RefreshCommand = new DelegateCommand(async () => await ExecuteRefreshCommand());
            NextCommand = new DelegateCommand(async () => await ExecuteNextCommand());
            PreviousCommand = new DelegateCommand(async () => await ExecutePreviousCommand());
            SelectedCommand = new DelegateCommand(async () => await ExecuteSelectedCommand());
            NewCommand = new DelegateCommand(async () => await ExecuteNewCommand());
            SearchCommand = new DelegateCommand(ExecuteSearchCommand);
        }

        public async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var budgetResult = await EnvelopeLogic.GetBudgetsAsync(Schedule);

                if (budgetResult.Success)
                {
                    Budgets = new ObservableCollection<Budget>(budgetResult.Data);
                    GroupedBudgets = new ObservableCollection<GroupedList<Budget>>(EnvelopeLogic.GroupBudgets(Budgets));
                }
                else
                {
                    //show error
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteNextCommand()
        {
            var scheduleResult = await EnvelopeLogic.GetNextBudgetScheduleAsync(Schedule);
            if (scheduleResult.Success)
            {
                Schedule = scheduleResult.Data;
                await ExecuteRefreshCommand();
            }
            else
            {
                //show error
            }
        }

        public async Task ExecutePreviousCommand()
        {
            var scheduleResult = await EnvelopeLogic.GetPreviousBudgetScheduleAsync(Schedule);
            if (scheduleResult.Success)
            {
                Schedule = scheduleResult.Data;
                await ExecuteRefreshCommand();
            }
            else
            {
                //show error
            }
        }

        public async Task ExecuteSelectedCommand()
        {
            if (SelectedBudget == null)
            {
                return;
            }

            if (SelectorMode)
            {
                var parameters = new NavigationParameters
                {
                    { NavigationParameterType.Envelope, SelectedBudget.Envelope }
                };
                await NavigationService.GoBackAsync(parameters);
            }
            else
            {
                var parameters = new NavigationParameters
                {
                    { NavigationParameterType.Budget, SelectedBudget }
                };
                await NavigationService.NavigateAsync(NavigationPageName.EnvelopeInfoPage, parameters);
            }

            SelectedBudget = null;
        }

        public async Task ExecuteNewCommand()
        {
            var parameters = new NavigationParameters
            {
                { NavigationParameterType.BudgetSchedule, Schedule }
            };

            await NavigationService.NavigateAsync(NavigationPageName.EnvelopeEditPage, parameters);

            SelectedBudget = null;
        }

        public void ExecuteSearchCommand()
        {
            GroupedBudgets = new ObservableCollection<GroupedList<Budget>>(EnvelopeLogic.GroupBudgets(EnvelopeLogic.SearchBudgets(Budgets, SearchText)));
        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            await ExecuteRefreshCommand();
        }

        public override async void OnNavigatingTo(NavigationParameters parameters)
        {
            // returns default bool if none present
            SelectorMode = parameters.GetValue<bool>(NavigationParameterType.SelectorMode);

            await ExecuteRefreshCommand();
        }
    }
}
