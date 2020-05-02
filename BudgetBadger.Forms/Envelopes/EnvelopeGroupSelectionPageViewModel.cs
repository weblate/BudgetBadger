﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BudgetBadger.Core.Logic;
using BudgetBadger.Models;
using BudgetBadger.Forms.Enums;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using Prism.Mvvm;
using BudgetBadger.Core.Sync;
using BudgetBadger.Models.Extensions;
using BudgetBadger.Core.LocalizedResources;
using Prism.Events;
using BudgetBadger.Forms.Events;

namespace BudgetBadger.Forms.Envelopes
{
    public class EnvelopeGroupSelectionPageViewModel : BindableBase, INavigationAware, IInitializeAsync
    {
        readonly IResourceContainer _resourceContainer;
        readonly IEnvelopeLogic _envelopeLogic;
        readonly INavigationService _navigationService;
        readonly IPageDialogService _dialogService;
        readonly ISyncFactory _syncFactory;
        readonly IEventAggregator _eventAggregator;

        public ICommand BackCommand { get => new DelegateCommand(async () => await _navigationService.GoBackAsync()); }
        public ICommand SelectedCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SaveCommand { get; set; }
		public ICommand AddCommand { get; set; }
        public ICommand ManageGroupsCommand { get => new DelegateCommand(async () => await _navigationService.NavigateAsync(PageName.EnvelopeGroupsPage)); }
        public Predicate<object> Filter { get => (env) => _envelopeLogic.FilterEnvelopeGroup((EnvelopeGroup)env, SearchText); }

        bool _needToSync;

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        EnvelopeGroup _selectedEnvelopeGroup;
        public EnvelopeGroup SelectedEnvelopeGroup
        {
            get => _selectedEnvelopeGroup;
            set => SetProperty(ref _selectedEnvelopeGroup, value);
        }

        ObservableList<EnvelopeGroup> _envelopeGroups;
        public ObservableList<EnvelopeGroup> EnvelopeGroups
        {
            get => _envelopeGroups;
            set => SetProperty(ref _envelopeGroups, value);
        }

        string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); RaisePropertyChanged(nameof(HasSearchText)); }
        }

        public bool HasSearchText { get => !string.IsNullOrWhiteSpace(SearchText); }

        bool _noEnvelopeGroups;
        public bool NoEnvelopeGroups
        {
            get => _noEnvelopeGroups;
            set => SetProperty(ref _noEnvelopeGroups, value);
        }

        public EnvelopeGroupSelectionPageViewModel(IResourceContainer resourceContainer,
                                           INavigationService navigationService,
                                           IPageDialogService dialogService,
                                           IEnvelopeLogic envelopeLogic,
                                           ISyncFactory syncFactory,
                                           IEventAggregator eventAggregator)
        {
            _resourceContainer = resourceContainer;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _envelopeLogic = envelopeLogic;
            _syncFactory = syncFactory;
            _eventAggregator = eventAggregator;

            SelectedEnvelopeGroup = null;
            EnvelopeGroups = new ObservableList<EnvelopeGroup>();

            SelectedCommand = new DelegateCommand<EnvelopeGroup>(async eg => await ExecuteSelectedCommand(eg));
            RefreshCommand = new DelegateCommand(async () => await FullRefresh());
            SaveCommand = new DelegateCommand(async () => await ExecuteSaveCommand());
			AddCommand = new DelegateCommand(async () => await ExecuteAddCommand());

            _eventAggregator.GetEvent<EnvelopeGroupSavedEvent>().Subscribe(RefreshEnvelopeGroup);
            _eventAggregator.GetEvent<EnvelopeGroupDeletedEvent>().Subscribe(RefreshEnvelopeGroup);
            _eventAggregator.GetEvent<EnvelopeGroupHiddenEvent>().Subscribe(RefreshEnvelopeGroup);
            _eventAggregator.GetEvent<EnvelopeGroupUnhiddenEvent>().Subscribe(RefreshEnvelopeGroup);
            _eventAggregator.GetEvent<TransactionSavedEvent>().Subscribe(async t => await RefreshEnvelopeGroupFromTransaction(t));
        }

        public async void OnNavigatedFrom(INavigationParameters parameters)
        {
            SelectedEnvelopeGroup = null;

            if (_needToSync)
            {
                var syncService = _syncFactory.GetSyncService();
                var syncResult = await syncService.FullSync();

                if (syncResult.Success)
                {
                    await _syncFactory.SetLastSyncDateTime(DateTime.Now);
                    _needToSync = false;
                }
            }
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
			var envelopeGroup = parameters.GetValue<EnvelopeGroup>(PageParameter.EnvelopeGroup);
            if (envelopeGroup != null)
            {
                await _navigationService.GoBackAsync(parameters);
                return;
            }

            if (parameters.GetNavigationMode() == NavigationMode.Back)
            {
                await InitializeAsync(parameters);
            }
        }

        public async Task InitializeAsync(INavigationParameters parameters)
        {
            var countResult = await _envelopeLogic.GetEnvelopeGroupsCountAsync();
            if (countResult.Success)
            {
                if (countResult.Data <=  3) //3 because of the built in system, debt, income groups
                {
                    // add some 
                    var montlhyBills = new EnvelopeGroup{ Id = new Guid("{f3d90935-bb10-4cf7-ae4b-fa7ca041a6b1}"), Description = _resourceContainer.GetResourceString("EnvelopeGroupMonthlyBills") };
                    var result1 = await _envelopeLogic.SaveEnvelopeGroupAsync(montlhyBills);
                    _eventAggregator.GetEvent<EnvelopeGroupSavedEvent>().Publish(result1.Data);

                    var everydayExpenses = new EnvelopeGroup { Id = new Guid("{ce3bc99c-610b-413c-a06a-5888ef596cf1}"), Description = _resourceContainer.GetResourceString("EnvelopeGroupEverydayExpenses") };
                    var result2 = await _envelopeLogic.SaveEnvelopeGroupAsync(everydayExpenses);
                    _eventAggregator.GetEvent<EnvelopeGroupSavedEvent>().Publish(result2.Data);

                    var savingsGoals = new EnvelopeGroup { Id = new Guid("{0f3e250f-db63-4c5e-8090-f1dca882fb53}"), Description = _resourceContainer.GetResourceString("EnvelopeGroupSavingsGoals") };
                    var result3 = await _envelopeLogic.SaveEnvelopeGroupAsync(savingsGoals);
                    _eventAggregator.GetEvent<EnvelopeGroupSavedEvent>().Publish(result3.Data);

                    // show message
                    await _dialogService.DisplayAlertAsync(_resourceContainer.GetResourceString("AlertSuggestedEnvelopeGroups"),
                        _resourceContainer.GetResourceString("AlertMessageSuggestedEnvelopeGroups"),
                        _resourceContainer.GetResourceString("AlertOk"));
                }
            }

            await FullRefresh();
        }

        public async Task FullRefresh()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _envelopeLogic.GetEnvelopeGroupsForSelectionAsync();

                if (result.Success)
                {
                    EnvelopeGroups.ReplaceRange(result.Data);
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(_resourceContainer.GetResourceString("AlertRefreshUnsuccessful"), result.Message, _resourceContainer.GetResourceString("AlertOk"));
                }

                NoEnvelopeGroups = (EnvelopeGroups?.Count ?? 0) == 0;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteSelectedCommand(EnvelopeGroup envelopeGroup)
        {
            if (envelopeGroup == null)
            {
                return;
            }

            var parameters = new NavigationParameters
            {
                { PageParameter.EnvelopeGroup, envelopeGroup }
            };

            await _navigationService.GoBackAsync(parameters);
        }

        public async Task ExecuteSaveCommand()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var newEnvelopeGroup = new EnvelopeGroup
                {
                    Description = SearchText
                };

                var result = await _envelopeLogic.SaveEnvelopeGroupAsync(newEnvelopeGroup);

                if (result.Success)
                {
                    _needToSync = true;

                    _eventAggregator.GetEvent<EnvelopeGroupSavedEvent>().Publish(result.Data);

                    await _navigationService.GoBackAsync();
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(_resourceContainer.GetResourceString("AlertSaveUnsuccessful"), result.Message, _resourceContainer.GetResourceString("AlertOk"));
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

		public async Task ExecuteAddCommand()
        {
            await _navigationService.NavigateAsync(PageName.EnvelopeGroupEditPage);
        }

        public void RefreshEnvelopeGroup(EnvelopeGroup envelopeGroup)
        {
            var envelopeGroups = EnvelopeGroups.Where(a => a.Id != envelopeGroup.Id).ToList();

            if (envelopeGroup != null && envelopeGroup.IsActive)
            {
                envelopeGroups.Add(envelopeGroup);
            }

            EnvelopeGroups.ReplaceRange(envelopeGroups);
        }

        public async Task RefreshEnvelopeGroupFromTransaction(Transaction transaction)
        {
            if (transaction != null && transaction.Envelope != null)
            {
                var updatedGroupResult = await _envelopeLogic.GetEnvelopeGroupAsync(transaction.Envelope.Group.Id);
                if (updatedGroupResult.Success)
                {
                    RefreshEnvelopeGroup(updatedGroupResult.Data);
                }
            }
        }
    }
}
